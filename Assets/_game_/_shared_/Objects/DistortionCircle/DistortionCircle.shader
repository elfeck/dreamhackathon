Shader "stillalive-studios/Effects/Distortion Circle"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white"
		params ("Parameter: waveVector, frequency, damping, color amplitude", vector) = (20, 2, 3, 0)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}
	
		//grabPass - we grab the back-buffer
		GrabPass {}
		
		Pass
		{
			Cull Off
			ZWrite Off
			Fog {Mode Off}
		
			CGPROGRAM
			
			#pragma vertex VS
			#pragma fragment PS
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			
			//params
			float4 params; //x=wavevektor, y=frequency, z=damping, w=amplitude
			float4 distortionAmplitude; //xy distortion amplitude in xy direction
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _GrabTexture;
			
			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float4 uvgrab : TEXCOORD0;
				float2 tex0 : TEXCOORD1;
			};
			
			VS_OUTPUT VS(appdata_base v)
			{
				VS_OUTPUT output = (VS_OUTPUT)(0);
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				//this seems to be neccessary!
				#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
				#else
				float scale = 1.0;
				#endif
				
				//we perform half the calculation for the correct texture-coords here, and half in the PS.
				//IMPORTANT: perform the nonlinear transformation (projection!) after the linear interpolation --> therefore in the PS
				output.uvgrab.xy = (float2(output.pos.x, scale*output.pos.y) + output.pos.w) * 0.5f;
				output.uvgrab.w = output.pos.w;
				output.tex0 = TRANSFORM_TEX(v.texcoord, _MainTex);
				return output;
			}
			
			float4 PS(VS_OUTPUT input): COLOR
			{
				input.uvgrab.xy /= input.uvgrab.w;
				float2 dir = input.tex0 - float2(0.5f, 0.5f);
				float r = length(dir);
				clip(0.5f - r);

				//distort
				dir = normalize(dir);
				float2 distort = distortionAmplitude * dir * abs(sin(r * params.x - _Time * params.y)) * exp(-r * params.z);
				input.uvgrab.xy += distort;

				return tex2D(_GrabTexture, input.uvgrab.xy) + 500f * float4(1f, 1f, 1f, 0f) * 
						clamp(sin(r * params.x - _Time * params.y) - 0.8f, 0f, 1f) * exp(-r * params.z * 3f) * params.w;
			}
			
			ENDCG
		
		}
	} 
	//FallBack "Diffuse"
}

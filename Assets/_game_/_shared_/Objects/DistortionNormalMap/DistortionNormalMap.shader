Shader "stillalive-studios/Effects/Distortion Normal Map"
{
	Properties
	{
		_TintColor ("Multiplicative Color", color) = (1,1,1,1)
		_ColorIntensity ("Color strength on distortion", float) = 10
		_DistortionMap ("Distortion Normal Map", 2D) = "bump" {}
		amplitude ("Parameter: amplitudeX, amplitudeY, amplitude factor", vector) = (1, 1, 0.1, 0)
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
			float4 amplitude; //x=wavevektor, y=frequency, z=damping, w=amplitude
			
			float4 _TintColor;
			float _ColorIntensity;
			sampler2D _GrabTexture;
			sampler2D _DistortionMap;
			float4 _DistortionMap_ST;
			
			struct VS_INPUT
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float4 uvgrab : TEXCOORD0;
				float2 tex0 : TEXCOORD1;
				float4 color : COLOR;
			};
			
			VS_OUTPUT VS(VS_INPUT v)
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
				output.tex0 = TRANSFORM_TEX(v.texcoord, _DistortionMap);
				output.color = v.color;
				return output;
			}
			
			float4 PS(VS_OUTPUT input): COLOR
			{
				float2 dir = UnpackNormal(tex2D(_DistortionMap, input.tex0)).xy;
				input.uvgrab.xy /= input.uvgrab.w;
				float2 distort = amplitude.xy * dir * amplitude.z * _TintColor.a * input.color.a;
				input.uvgrab.xy += distort;
				return tex2D(_GrabTexture, input.uvgrab.xy)
					* lerp(float4(1,1,1,1), float4(_TintColor.rgb * input.color.rgb, 1), length(distort.xy) * _ColorIntensity);
			}
			
			ENDCG
		
		}
	} 
	//FallBack "Diffuse"
}

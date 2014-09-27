Shader "Hidden/TextureBlender"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white"
		blendingTexture ("Texture to blend with", 2D) = "white"
		params ("Parameter: blendFactor", vector) = (0.5,0,0,0)
		blendColor ("Color to blend to", vector) = (1,1,1,1)
	}
	SubShader
	{
		Pass
		{
			Cull Off
			ZWrite Off
			ZTest Always
			Fog {Mode off}
		
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment PS
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			sampler2D blendingTexture;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 params;
			float4 blendColor;
			
			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
			};
			
			VS_OUTPUT VS(appdata_base v)
			{
				VS_OUTPUT output = (VS_OUTPUT)0;
				output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				output.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
				return output;
			}
			
			float4 PS(VS_OUTPUT input) : COLOR
			{
				float4 texColor = tex2D(blendingTexture, input.tex);
				return lerp(tex2D(_MainTex, input.tex), blendColor * texColor, params.x * texColor.a);
			}
	
			ENDCG
		
		}
	} 
	Fallback off
}

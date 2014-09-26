Shader "stillalive-studios/Hard-Surfaces/Emissive Diffuse"
{
	Properties
	{
		_TintColor ("Color (RGB)", color) = (1,1,1,1)
		_MainTex ("Diffuse Texture", 2D) = "white" {}
		colorIntensity ("Color intensity (HDR)", float) = 1
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		
		Pass
		{
		
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment PS
			#include "UnityCG.cginc"
	
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float colorIntensity;
			float4 _TintColor;
		
			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float2 tex : TEXCOORD0;
			};
	
			VS_OUTPUT VS(appdata_base input)
			{
				VS_OUTPUT output = (VS_OUTPUT)0;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.tex = TRANSFORM_TEX(input.texcoord, _MainTex);
				return output;
			}
	
			float4 PS(VS_OUTPUT input) : COLOR
			{
				return tex2D(_MainTex, input.tex) * _TintColor * colorIntensity;
			}
			
			ENDCG
		}
	}
	Fallback "Diffuse"
}
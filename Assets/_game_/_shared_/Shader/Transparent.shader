Shader "stillalive-studios/Transparent/Simplest Transparent"
{
	Properties
	{
		_MainTex("Base Quad Texture", 2D) = "white" {}
		_TintColor("Color (multiplicative)", Color) = (1,1,1,1)
		alphaMultiplier("alpha Multiplier", float) = 1
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "RenderType"="Transparent"}
		Pass
		{			
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Fog {Mode Off}
		
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment PS
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float alphaMultiplier;

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
				float4 output = tex2D(_MainTex, input.tex) * _TintColor;
				output.a = saturate(output.a * alphaMultiplier);
				return output;
			}
			
			ENDCG
		
		}
	} 
	FallBack "Diffuse"
}

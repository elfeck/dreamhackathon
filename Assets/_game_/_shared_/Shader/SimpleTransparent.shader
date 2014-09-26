Shader "stillalive-studios/Transparent/Transparent with Edge-Fading"
{
	Properties
	{
		_MainTex("Base Quad Texture", 2D) = "white" {}
		quadColor("Color (multiplicative)", Color) = (1,1,1,1)
		alphaMultiplier("Alpha multiplier", float) = 1
		fadingParam("fading params: x left, x right, y up, y down", vector) = (1000,1000,1000,1000)
	}
	SubShader
	{
		Tags {"Queue" = "Transparent"}
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
			float4 quadColor;
			float alphaMultiplier;
			float4 fadingParam;

			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float4 texTransformedAndWhole : TEXCOORD0;
			};

			VS_OUTPUT VS(appdata_base input)
			{
				VS_OUTPUT output = (VS_OUTPUT)0;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.texTransformedAndWhole.xy = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.texTransformedAndWhole.zw = input.texcoord;
				return output;
			}

			float4 PS(VS_OUTPUT input) : COLOR
			{
				float4 output = tex2D(_MainTex, input.texTransformedAndWhole.xy) * quadColor;
				output.a *= alphaMultiplier 
					* (1f - exp(-input.texTransformedAndWhole.z * fadingParam.x))
					* (1f - exp(-input.texTransformedAndWhole.w * fadingParam.z))
					* (1f - exp(-(1f - input.texTransformedAndWhole.z) * fadingParam.y))
					* (1f - exp(-(1f - input.texTransformedAndWhole.w) * fadingParam.w));
				output.a = saturate(output.a);
				return output;
			}
			
			ENDCG
		
		}
	} 
	FallBack "Diffuse"
}

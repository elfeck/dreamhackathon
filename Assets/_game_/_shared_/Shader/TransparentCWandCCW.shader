Shader "stillalive-studios/Transparent/Transparent 2-pass CW and CCW"
{
	Properties
	{
		_MainTex("Base Quad Texture", 2D) = "white" {}
		_TintColor("Color (multiplicative)", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {"Queue" = "Transparent"}
		
		Pass
		{			
			Cull Front
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

			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float2 tex0 : TEXCOORD0;
			};

			VS_OUTPUT VS(appdata_base input)
			{
				VS_OUTPUT output = (VS_OUTPUT)0;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.tex0 = TRANSFORM_TEX(input.texcoord, _MainTex);
				return output;
			}

			float4 PS(VS_OUTPUT input) : COLOR
			{
				float4 output = tex2D(_MainTex, input.tex0) * _TintColor;
				return output;
			}
			
			ENDCG
		
		}
		
		Pass
		{			
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
		
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment PS
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 quadColor;

			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float2 tex0 : TEXCOORD0;
			};

			VS_OUTPUT VS(appdata_base input)
			{
				VS_OUTPUT output = (VS_OUTPUT)0;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.tex0 = TRANSFORM_TEX(input.texcoord, _MainTex);
				return output;
			}

			float4 PS(VS_OUTPUT input) : COLOR
			{
				float4 output = tex2D(_MainTex, input.tex0) * quadColor;
				return output;
			}
			
			ENDCG
		
		}
	} 
	FallBack "Diffuse"
}

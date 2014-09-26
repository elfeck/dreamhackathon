Shader "stillalive-studios/Special Purpose/OverDraw Shader (No Z-Test)"
{
	Properties
	{
		baseTex("Base Quad Texture", 2D) = "white" {}
		quadColor("Color (multiplicative)", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {"Queue" = "Transparent+1"}
		Pass
		{			
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Off
		
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment PS
			#include "UnityCG.cginc"
			
			sampler2D baseTex;
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
				output.tex0 = input.texcoord;
				return output;
			}
			
			float4 PS(VS_OUTPUT input) : COLOR
			{
				float4 output = tex2D(baseTex, input.tex0) * quadColor;
				return output;
			}
			
			ENDCG
		
		}
	} 
	FallBack "Diffuse"
}

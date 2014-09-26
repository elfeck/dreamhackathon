Shader "stillalive-studios/Particle/Particle Additive Shine Through"
{
	Properties
	{
		_MainTex("Base Quad Texture", 2D) = "white" {}
		_TintColor("Color (multiplicative)", Color) = (1,1,1,1)
		shinethroughFactor("Shine-through factor", Range(0,1)) = 0.5
		
	}
	SubShader
	{
		Tags {"Queue" = "Transparent"  "RenderType"="Transparent"}
		Pass
		{			
			Cull Off
			Blend SrcAlpha One
			ZWrite off
			ZTest Always
			Fog {Mode Off}
		
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment PS
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			sampler2D _CameraDepthTexture;
			float4 _CameraDepthTexture_ST;
			float shinethroughFactor;
			
			struct VS_INPUT
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			struct VS_OUTPUT
			{
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float4 color : COLOR;
				float2 tex0 : TEXCOORD0;
				float2 linZ : TEXCOORD2;
			};

			VS_OUTPUT VS(VS_INPUT input)
			{
				VS_OUTPUT output = (VS_OUTPUT)0;
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				output.tex0 = TRANSFORM_TEX(input.texcoord, _MainTex);
				output.color = input.color;
				output.screenPos = ComputeGrabScreenPos(output.pos);
				output.linZ = output.pos.zw;				
				return output;
			}

			float4 PS(VS_OUTPUT input) : COLOR
			{
				input.screenPos.xy /= input.screenPos.w;
				float depth = tex2D(_CameraDepthTexture, input.screenPos.xy);
				float4 output = tex2D(_MainTex, input.tex0) * _TintColor * input.color;
				output.a *= ((1-shinethroughFactor) * step(input.linZ.x, LinearEyeDepth(depth)) + shinethroughFactor);
				return output;
			}
			
			ENDCG
		
		}
	} 
	FallBack "Particles/Additive"
}

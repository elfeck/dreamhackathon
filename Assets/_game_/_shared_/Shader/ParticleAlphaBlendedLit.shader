Shader "stillalive-studios/Particle/Particle Alpha blended Lit"
{
	Properties
	{
		_MainTex("Base Quad Texture", 2D) = "white" {}
		_TintColor("Color (multiplicative)", Color) = (1,1,1,1)
		softnessFactor("Softness", float) = 1
		
	}
	SubShader
	{
		Tags {"Queue" = "Transparent"  "RenderType"="Transparent"}
		
//		Pass {
//
//                Tags {
//
//                    "LightMode"="ForwardBase"
//
//                }
//
//           
//
//                Lighting Off
//
//                Blend SrcAlpha OneMinusSrcAlpha
//
//               
//
//                BindChannels {
//
//                    Bind "Vertex", vertex
//
//                    Bind "Texcoord", texcoord
//
//                    Bind "Color", color
//
//                }
//
// 
//
//                SetTexture [_MainTex] {
//
//                    combine primary * texture
//
//                }
//
//            }
		
		
		Pass
		{
			Tags {"LightMode"="ForwardBase"}
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite off
			Lighting on
			Fog {Mode Off}
		
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment PS
			#pragma multi_compile_fwdbase
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			sampler2D _CameraDepthTexture;
			float4 _CameraDepthTexture_ST;
			float softnessFactor;
			
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
				float3 light : TEXCOORD3;
				LIGHTING_COORDS(4,5)
			};
			
			float3 ShadeVertexLightsBillboard(float4 vertex)
			{
				float3 viewpos = mul (UNITY_MATRIX_MV, vertex).xyz;
				float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
				for (int i = 0; i < 4; i++)
				{
					float3 toLight = unity_LightPosition[i].xyz - viewpos.xyz * unity_LightPosition[i].w;
					float lengthSq = dot(toLight, toLight);
					float atten = 1.0 / (1.0 + lengthSq * unity_LightAtten[i].z);
					lightColor += unity_LightColor[i].rgb * atten;
				}
				return lightColor;
			}

			VS_OUTPUT VS(VS_INPUT v)
			{
				VS_OUTPUT o = (VS_OUTPUT)0;
				o.light = ShadeVertexLightsBillboard(v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.tex0 = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				o.screenPos = ComputeGrabScreenPos(o.pos);
				o.linZ = o.pos.zw;
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}

			float4 PS(VS_OUTPUT input) : COLOR
			{
//				float3 light = ShadeVertexLightsBillboard(input.objectPos);
				input.screenPos.xy /= input.screenPos.w;
				float depth = tex2D(_CameraDepthTexture, input.screenPos.xy);
				float4 output = tex2D(_MainTex, input.tex0) * _TintColor * input.color;
				output.a *= saturate((LinearEyeDepth(depth) - input.linZ.x) * softnessFactor);
				return output * float4(input.light.rgb * LIGHT_ATTENUATION(input), 1);
			}
			
			ENDCG
		
		}
	} 
	FallBack "Particles/Alpha Blended"
}
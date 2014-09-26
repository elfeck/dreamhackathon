Shader "stillalive-studios/Transparent/Bumped Specular-Color-Map with Alpha" {
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Texture", 2D) = "white" {}
	_SpecMap ("SpecMap", 2D) = "white" {}
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader
{
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	Cull Off

CGPROGRAM
#pragma surface surf BlinnPhongSpecularMap alpha
#pragma target 3.0

#include "BlinnPhongSpecularMap.cginc"

//---------------------------------------------------------------------------------//
 
 
struct Input
{
	float2 uv_MainTex;
	float2 uv_SpecMap;
	float2 uv_BumpMap;
};

sampler2D _MainTex;
sampler2D _SpecMap;
sampler2D _BumpMap;
float _Shininess;
float4 _Color;

void surf(Input IN, inout SpecularMapSurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Alpha = tex.a * _Color.a;
	half4 spec = tex2D(_SpecMap, IN.uv_SpecMap);
	o.GlossColor = spec.rgb;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}


ENDCG
}
Fallback "Diffuse"
}
Shader "stillalive-studios/Hard-Surfaces/Bumped Specular with Detail Map and Glow"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_bumpPower ("Strength of main normal map", float) = 1
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_DiffuseTileable ("Diffuse Tileable Detail (multiplicative)", 2D) = "white" {}
	_GlowMap ("Glow Map (RGB)", 2D) = "black" {}
	_GlowFactor ("Glow Multiplication Factor", float) = 1
}
SubShader
{ 
	Tags { "RenderType"="Opaque" }
	
CGPROGRAM
#pragma target 3.0
#pragma surface surf BlinnPhong

sampler2D _MainTex;
sampler2D _DiffuseTileable;
sampler2D _BumpMap;
sampler2D _GlowMap;
float _GlowFactor;
fixed4 _Color;
half _Shininess;
half _bumpPower;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float2 uv_DiffuseTileable;
	float2 uv_GlowMap;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 detail = tex2D(_DiffuseTileable, IN.uv_DiffuseTileable);
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb * detail.rgb;
	o.Gloss = tex.a * detail.a;
	o.Alpha = tex.a * _Color.a * detail.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * float3(_bumpPower, _bumpPower, 1);
	o.Emission = tex2D(_GlowMap, IN.uv_GlowMap) * _GlowFactor;
}
ENDCG

}
FallBack "Specular"
}

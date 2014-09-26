Shader "stillalive-studios/Transparent/Bumped Diffuse Cutout with Glow"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_GlowMap ("Glow Map (RGB)", 2D) = "black" {}
	_GlowFactor ("Glow Multiplication Factor", float) = 1
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader
{
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	Offset -1,-1
	
CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _GlowMap;
float _GlowFactor;
fixed4 _Color;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float2 uv_GlowMap;
};

void surf(Input IN, inout SurfaceOutput o)
{	
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Alpha = tex.a * _Color.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
//	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * float3(_bumpPower, _bumpPower, 1);
	o.Emission = tex2D(_GlowMap, IN.uv_GlowMap) * _GlowFactor;
}
ENDCG
}

FallBack "Decal/Cutout Diffuse"
}

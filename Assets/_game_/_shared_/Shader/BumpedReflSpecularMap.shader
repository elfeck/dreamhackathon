Shader "stillalive-studios/Special Purpose/Bumped Reflection Based Specular Mapping (Bug-Shader)"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_SpecularColorCube ("Cubemap defining Specular Color", Cube) = "" {}
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	
	_BlendMap ("Blendmap (RGB) Blend Gloss (A)", 2D) = "white" {}
	_BlendBumpMap ("Normalmap of Blendmap", 2D) = "bump" {}
	_OverlayMap ("Overlay-Map (grayscale)", 2D) = "black" {}
	_OverlayProgress ("Overlay Progress", Range(0,1)) = 0
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
CGPROGRAM
#pragma surface surf BlinnPhongSpecularMap
#pragma target 3.0
#pragma glsl

#include "BlinnPhongSpecularMap.cginc"


sampler2D _MainTex;
sampler2D _BumpMap;
samplerCUBE _SpecularColorCube;
fixed4 _Color;
half _Shininess;

sampler2D _BlendMap;
sampler2D _BlendBumpMap;
sampler2D _OverlayMap;
float _OverlayProgress;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float3 worldRefl; INTERNAL_DATA
	float2 uv_BlendMap;
	float2 uv_OverlayMap;
};

void applyBlending(Input IN, inout SpecularMapSurfaceOutput o)
{
	half overlayValue = tex2D(_OverlayMap, IN.uv_OverlayMap).x * 0.99;
	half factor = step(overlayValue, _OverlayProgress); //0 when value lower than progress, 1 otherwise

	float4 tex = lerp(float4(o.Albedo, 1), tex2D(_BlendMap, IN.uv_BlendMap), factor);
	o.Albedo = tex.rgb;
	o.GlossColor = lerp(o.GlossColor, float3(1,1,1) * tex.a, factor);
	//o.Normal = normalize(lerp(o.Normal, UnpackNormal(tex2D(_BlendBumpMap, IN.uv_BlendMap)), factor));
}

void surf(Input IN, inout SpecularMapSurfaceOutput o) 
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	float3 refl = WorldReflectionVector(IN, o.Normal);
	o.GlossColor = texCUBE(_SpecularColorCube, refl).rgb * tex.a;
	
	applyBlending(IN, o);
}
ENDCG
}

FallBack "Specular"
}

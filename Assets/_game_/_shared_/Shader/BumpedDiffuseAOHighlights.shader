Shader "stillalive-studios/Hard-Surfaces/Bumped Diffuse AO + Highlighting" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TileableTex ("Tilable Texture", 2D) = "white" {}
	_AOTex ("Ambient Occlusion (RGB), Highlighting Channel (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	hightlightStrength ("Highlight strength", Range(0,3)) = 1
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 300

CGPROGRAM
#pragma surface surf Lambert

sampler2D _AOTex;
sampler2D _TileableTex;
sampler2D _BumpMap;
fixed4 _Color;
float hightlightStrength;

struct Input
{
	float2 uv_AOTex;
	float2 uv_TileableTex;
	float2 uv_BumpMap;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 ao = tex2D(_AOTex, IN.uv_AOTex);
	fixed4 c =  tex2D(_TileableTex, IN.uv_TileableTex) * (float4(ao.rgb,1) + float4(ao.a,ao.a,ao.a,0) * hightlightStrength);
	o.Albedo = c.rgb * _Color;
	o.Alpha = c.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG  
}

FallBack "Diffuse"
}

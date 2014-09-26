Shader "stillalive-studios/Hard-Surfaces/Bumped Diffuse + Detail Mapping" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_TileableTex ("Diffuse Texture (tilable)", 2D) = "white" {}
	_AOTex ("AO (RGB), Highlight Channel (A)", 2D) = "white" {}
	_DetailAOTex ("Detail AO (RGB) (tilable)", 2D) = "white" {}
	hightlightStrength ("Highlight strength", Range(0,3)) = 1
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_DetailBumpMap ("Detail Normalmap (tileable)", 2D) = "bump" {}
	normalMapBlendFactor ("Blendfactor between Normalmaps", Range(0,1)) = 0.5
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 300

CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0

sampler2D _TileableTex;
sampler2D _AOTex;
sampler2D _DetailAOTex;
sampler2D _BumpMap;
sampler2D _DetailBumpMap;
fixed4 _Color;
float hightlightStrength;
float normalMapBlendFactor;

struct Input
{
	float2 uv_TileableTex;
	float2 uv_AOTex;
	float2 uv_DetailAOTex;
	float2 uv_BumpMap;
	float2 uv_DetailBumpMap;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 ao = tex2D(_AOTex, IN.uv_AOTex) * float4(tex2D(_DetailAOTex, IN.uv_DetailAOTex).rgb, 1);
	fixed4 c =  tex2D(_TileableTex, IN.uv_TileableTex) * (float4(ao.rgb,1) + float4(ao.a,ao.a,ao.a,0) * hightlightStrength);
	o.Albedo = c.rgb * _Color;
	o.Alpha = c.a;
	o.Normal = normalize(normalMapBlendFactor * UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap))
		+ (1-normalMapBlendFactor) * UnpackNormal(tex2D(_DetailBumpMap, IN.uv_DetailBumpMap)));
	
}
ENDCG  
}

FallBack "Diffuse"
}

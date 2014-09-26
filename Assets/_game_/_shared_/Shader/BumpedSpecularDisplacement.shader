Shader "stillalive-studios/Hard-Surfaces/Bumped Specular Vertex Displacement" {
Properties {
	_TintColor ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_BumpMapStrength ("Strength of Normalmap", float) = 1
	_HeightFactor ("Height Displacement Factor", float) = 1
	_HeightOffset ("Height Offset (Shifts the Displacement)", Range(-1,0)) = 0
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_HeightMap ("Heightmap (grayscale)", 2D) = "black" {}
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	Cull Off
	
CGPROGRAM
#pragma surface surf BlinnPhong vertex:vert addshadow
#pragma target 3.0
#pragma glsl

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _HeightMap;
float4 _HeightMap_ST;
fixed4 _TintColor;
half _Shininess;
float _BumpMapStrength;
float _HeightFactor;
float _HeightOffset;

void vert(inout appdata_full v, out Input o)
{
	//apply displacement mapping
	float2 heightTexCoord = TRANSFORM_TEX(v.texcoord, _HeightMap);
	float height = (tex2Dlod(_HeightMap, float4(heightTexCoord.xy, 0, 0)).r + _HeightOffset) * _HeightFactor;
	v.vertex.xyz += v.normal.xyz * height;
}

inline float3 applyStrengthFactor2Bump(float3 bumpVector, float strength)
{
	return normalize(bumpVector * float3(strength, strength, 1));
}

void surf(Input IN, inout SurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _TintColor.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _TintColor.a;
	o.Specular = _Shininess;
	o.Normal = applyStrengthFactor2Bump(UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)), _BumpMapStrength);
}
ENDCG
}

FallBack "BumpedSpecular"
}

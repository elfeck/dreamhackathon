Shader "stillalive-studios/Hard-Surfaces/Bumped Specular (advanced)" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_BumpMapStrength ("Strength of Normalmap", float) = 1
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	Cull Off
	
CGPROGRAM
#pragma surface surf BlinnPhong


sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float _BumpMapStrength;

struct Input {
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

inline float3 applyStrengthFactor2Bump(float3 bumpVector, float strength)
{
	return normalize(bumpVector * float3(strength, strength, 1));
}

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = applyStrengthFactor2Bump(UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)), _BumpMapStrength);
}
ENDCG
}

FallBack "Specular"
}

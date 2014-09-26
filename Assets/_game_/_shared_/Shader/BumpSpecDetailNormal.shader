Shader "stillalive-studios/Hard-Surfaces/Bumped Specular with Detail Normalmap"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_bump1Power ("Strength of main normal map", float) = 1
	_bump2Power ("Strength of detail normal map", float) = 1
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_DetailNormalMap ("Detail Normalmap", 2D) = "bump" {}
}
SubShader
{ 
	Tags { "RenderType"="Opaque" }
	LOD 400
	
CGPROGRAM
#pragma target 3.0
#pragma surface surf BlinnPhong

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _DetailNormalMap;
fixed4 _Color;
half _Shininess;
float _bump1Power;
float _bump2Power;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float2 uv_DetailNormalMap;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	
	//combine the two normal-maps together
	float3 n1 = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * float3(_bump1Power, _bump1Power, 1);
	float3 n2 = UnpackNormal(tex2D(_DetailNormalMap, IN.uv_DetailNormalMap)) * float3(_bump2Power, _bump2Power, 0);
	o.Normal = normalize(n1 + n2);
}
ENDCG
}

FallBack "Specular"
}

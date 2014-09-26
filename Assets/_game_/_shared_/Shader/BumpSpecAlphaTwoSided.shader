Shader "stillalive-studios/Transparent/Bumped Specular Alpha TwoSided" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color RGB, Gloss A", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 5)) = 0.078125
	_MainTex ("Base (RGBA)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_SpecularMap ("Specular-Map (Greyscale)", 2D) = "white" {}
	_CutOff ("Alpha Cutoff", Range(0,1)) = 0.5
}
SubShader { 
Tags {"Queue"="AlphaTest" "IgnoreProjector"="True"}

//--------------------------------------------------------------------------------//
//PASS 1

Cull off
Blend off
ZWrite on
	
CGPROGRAM
#pragma surface surf BlinnPhong alphatest:_CutOff vertex:vert
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _SpecularMap;
fixed4 _Color;
half _Shininess;

void vert (inout appdata_full v)
{
	//v.vertex.xyz += v.normal * _SinTime.y;
	float4 pos = mul(_Object2World, v.vertex);
	//flip normal to always look into the hemisphere where the camera is
	v.normal *= sign(dot(_WorldSpaceCameraPos-pos.xyz, mul((float3x3)_Object2World, SCALED_NORMAL)));
}

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 spec = tex2D(_SpecularMap, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = spec.r;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG

//--------------------------------------------------------------------------------//
//PASS 2

Cull off
Zwrite off
Fog {Mode Off}
Blend SrcAlpha OneMinusSrcAlpha
AlphaTest Less [_CutOff]
	
CGPROGRAM
#pragma surface surf BlinnPhong alpha vertex:vert

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _SpecularMap;
fixed4 _Color;
half _Shininess;

void vert (inout appdata_full v)
{
	//v.vertex.xyz += v.normal * _SinTime.y;
	float4 pos = mul(_Object2World, v.vertex);
	//flip normal to always look into the hemisphere where the camera is
	v.normal *= sign(dot(_WorldSpaceCameraPos-pos.xyz, mul((float3x3)_Object2World, SCALED_NORMAL)));
}

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 spec = tex2D(_SpecularMap, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = spec.r;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG
}

FallBack "Specular"
}

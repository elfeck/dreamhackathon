Shader "stillalive-studios/Vertex Paint/Bumped Specular with Vertex Colors" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_colorBlendingFactor ("Vertex Color Blending Factor", Range (0, 1)) = 1
	_Shininess ("Shininess", Range (0.01, 5)) = 0.078125
	_MainTex ("Base (RGB), Specular (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader { 

Cull back
Blend off
ZWrite on
	
CGPROGRAM
#pragma surface surf BlinnPhong

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float _colorBlendingFactor;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb * lerp(float3(1,1,1), IN.color.rgb, _colorBlendingFactor);
	o.Gloss = tex.a;
	o.Alpha = _Color.a * IN.color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG

}

FallBack "Bumped Specular"
}

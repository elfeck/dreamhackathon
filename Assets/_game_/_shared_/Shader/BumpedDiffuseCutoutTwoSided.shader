Shader "stillalive-studios/Transparent/Bumped Diffuse Cutout TwoSided"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader
{
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 300
	Cull Off
	
CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert


sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

void vert (inout appdata_full v)
{
	//v.vertex.xyz += v.normal * _SinTime.y;
	float4 pos = mul(_Object2World, v.vertex);
	//flip normal to always look into the hemisphere where the camera is
	v.normal *= sign(dot(_WorldSpaceCameraPos-pos.xyz, mul((float3x3)_Object2World, SCALED_NORMAL)));
}

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}
ENDCG
}

FallBack "Transparent/Cutout/Diffuse"
}

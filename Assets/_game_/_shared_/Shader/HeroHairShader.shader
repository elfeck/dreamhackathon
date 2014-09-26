//extended from: Shader "stillalive-studios/Transparent/Bumped Diffuse Cutout TwoSided"
Shader "stillalive-studios/Special Purpose/Hero Hair Shader"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	
	_BlendMap ("Blendmap (RGB) Blend Gloss (A)", 2D) = "white" {}
	_BlendBumpMap ("Normalmap of Blendmap", 2D) = "bump" {}
	_OverlayMap ("Overlay-Map (grayscale)", 2D) = "black" {}
	_OverlayProgress ("Overlay Progress", Range(0,1)) = 0
}

SubShader
{
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 300
	Cull Off
	
CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert
#pragma target 3.0


sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;

sampler2D _BlendMap;
sampler2D _BlendBumpMap;
sampler2D _OverlayMap;
float _OverlayProgress;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float2 uv_BlendMap;
	float2 uv_OverlayMap;
};

void applyBlending(Input IN, inout SurfaceOutput o)
{
	half overlayValue = tex2D(_OverlayMap, IN.uv_OverlayMap).x * 0.99;
	half factor = step(overlayValue, _OverlayProgress); //0 when value lower than progress, 1 otherwise

	float4 tex = lerp(float4(o.Albedo, o.Gloss), tex2D(_BlendMap, IN.uv_BlendMap), factor);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	//o.Normal = normalize(lerp(o.Normal, UnpackNormal(tex2D(_BlendBumpMap, IN.uv_BlendMap)), factor));
}

void vert(inout appdata_full v)
{
	//v.vertex.xyz += v.normal * _SinTime.y;
	float4 pos = mul(_Object2World, v.vertex);
	//flip normal to always look into the hemisphere where the camera is
	v.normal *= sign(dot(_WorldSpaceCameraPos-pos.xyz, mul((float3x3)_Object2World, SCALED_NORMAL)));
}

void surf(Input IN, inout SurfaceOutput o)
{
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	
	
	applyBlending(IN, o);
//	o.Normal = float3(0,1,0);
}
ENDCG
}

FallBack "stillalive-studios/Transparent/Bumped Diffuse Cutout TwoSided"
}

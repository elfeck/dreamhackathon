Shader "stillalive-studios/Special Purpose/Color Customizable Bumped Specular Detail Overlay" {
Properties {
	_TintColor ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_ColorizableMask ("Colorizable-Mask (RGB)", 2D) = "black" {}
	_UserColorR ("User Color 1 (R)", color) = (1,1,1,1)
	_UserColorG ("User Color 2 (G)", color) = (1,1,1,1)
	_UserColorB ("User Color 3 (B)", color) = (1,1,1,1)
	colorIntensity ("Color intensity (HDR)", float) = 1
	_BlendMap ("Blendmap (RGB) Blend Gloss (A)", 2D) = "white" {}
	_BlendBumpMap ("Normalmap of Blendmap", 2D) = "bump" {}
	_OverlayMap ("Overlay-Map (grayscale)", 2D) = "black" {}
	_OverlayProgress ("Overlay Progress", Range(0,1)) = 0
}
SubShader { 
	Tags { "RenderType"="Opaque" }
	LOD 400
	Cull Off
	
CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _ColorizableMask;
fixed4 _UserColorR;
fixed4 _UserColorG;
fixed4 _UserColorB;
fixed4 _TintColor;
half _Shininess;
float colorIntensity;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float2 uv_BlendMap;
	float2 uv_OverlayMap;
};


sampler2D _BlendMap;
sampler2D _BlendBumpMap;
sampler2D _OverlayMap;
half _OverlayProgress;

void applyBlending(Input IN, inout SurfaceOutput o)
{
	half overlayValue = tex2D(_OverlayMap, IN.uv_OverlayMap).x * 0.99;
	half factor = step(overlayValue, _OverlayProgress); //0 when value lower than progress, 1 otherwise

	float4 tex = lerp(float4(o.Albedo, o.Gloss), tex2D(_BlendMap, IN.uv_BlendMap) * _TintColor, factor);
	o.Albedo = tex.rgb;
	o.Gloss = tex.a;
	//o.Normal = normalize(lerp(o.Normal, UnpackNormal(tex2D(_BlendBumpMap, IN.uv_BlendMap)), factor));
}

void surf(Input IN, inout SurfaceOutput o)
{
	float4 mask = tex2D(_ColorizableMask, IN.uv_MainTex);
	float4 userColor = mask.r * _UserColorR + mask.g * _UserColorG + mask.b * _UserColorB;
	//make userColor white, if it is black (since it should have no effect upon multiplication!)
	userColor += float4(1,1,1,1) * step(-0.01, -(mask.r+mask.g+mask.b));

	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * saturate(userColor.rgb) * colorIntensity * _TintColor.rgb;
	o.Gloss = tex.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

	applyBlending(IN, o);
}
ENDCG
}

FallBack "Specular"
}

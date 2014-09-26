//(Color Customizable Bumped Specular Detail Overlay)
Shader "stillalive-studios/Special Purpose/Hero Body Shader"
{
Properties
{
	_TintColor ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_ColorizableMask ("Colorizable-Mask (RGB)", 2D) = "black" {}
	_hueShiftR ("User Hue Shift 1 (R)", float) = 0
	_hueShiftG ("User Hue Shift 2 (G)", float) = 0
	_hueShiftB ("User Hue Shift 3 (B)", float) = 0
	_saturationFactor ("Saturation of the colors", float) = 1
	_valueFactor ("Value factor of the colors", float) = 1
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
float _hueShiftR;
float _hueShiftG;
float _hueShiftB;
float _saturationFactor;
float _valueFactor;
float _OverlayProgress;
fixed4 _TintColor;
half _Shininess;
float colorIntensity;
sampler2D _BlendMap;
sampler2D _BlendBumpMap;
sampler2D _OverlayMap;
half _ShadowBrightness;
half _ShadowFactor;
half _MaxGlowBrightness;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
	float2 uv_BlendMap;
	float2 uv_OverlayMap;
};


float3 rgb2hsv(float3 c)
{
	float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
	float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv2rgb(float3 c)
{
	float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void applyShadowEffect(inout SurfaceOutput o)
{
	if(_ShadowFactor >= 0)
	{
		const half Y = _ShadowBrightness * dot(o.Albedo, half3(0.2126729, 0.7151522, 0.0721750));
		o.Albedo = lerp(o.Albedo, half3(Y, Y, Y), _ShadowFactor);
	}
	else
	{
		o.Emission = o.Albedo * _MaxGlowBrightness * (-_ShadowFactor);
	}
}

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
	float hueShift = mask.r * _hueShiftR + mask.g * _hueShiftG + mask.b * _hueShiftB;

	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * colorIntensity * _TintColor.rgb;
	
	//apply hue shift according to input
	if(dot(mask.rgb, float3(1,1,1)) > 0.05)
	{
		float3 hsv = rgb2hsv(o.Albedo.rgb);
		hsv.x = fmod(hsv.x + hueShift, 1);
		hsv.y *= _saturationFactor;
		hsv.z *= _valueFactor;
		o.Albedo.rgb = hsv2rgb(hsv);
	}
	
	o.Gloss = tex.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

	applyBlending(IN, o);
	applyShadowEffect(o);
}
ENDCG
}

FallBack "Specular"
}

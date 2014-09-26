//(Bumped Specular Detail Overlay TwoSided)
Shader "stillalive-studios/Special Purpose/Hero Head Shader"
{
Properties
{
	_TintColor ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_BlendMap ("Blendmap (RGB) Blend Gloss (A)", 2D) = "white" {}
	_BlendBumpMap ("Normalmap of Blendmap", 2D) = "bump" {}
	_OverlayMap ("Overlay-Map (grayscale)", 2D) = "black" {}
	_OverlayProgress ("Overlay Progress", Range(0,1)) = 0
}
SubShader
{
	Tags { "RenderType"="Opaque" }
	Cull Back
	
CGPROGRAM
#pragma surface surf BlinnPhong
#pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _TintColor;
half _Shininess;

struct Input
{
	float2 uv_MainTex;
	float2 uv_BlendMap;
	float2 uv_OverlayMap;
};

sampler2D _BlendMap;
sampler2D _BlendBumpMap;
sampler2D _OverlayMap;
half _OverlayProgress;
half _ShadowBrightness;
half _ShadowFactor;
half _MaxGlowBrightness;

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
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = _TintColor.rgb * tex.rgb;
	o.Gloss = tex.a;
	o.Alpha = tex.a * _TintColor.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

	applyBlending(IN, o);
	applyShadowEffect(o);
}
ENDCG
}

FallBack "Specular"
}

Shader "stillalive-studios/Masked Lighting/Bumped"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Diffuse2 ("Second Diffuse", 2D) = "white" {}
		_Diffuse2BlendFactor ("Diffuse 2 Additional Blend Factor", Range(0,1)) = 1
		_uvScale("Diffuse 2 UV Scale", Float) = 1
		_Ramp ("Toon Ramp (RGB)", 2D) = "white" {} 
		_blendExponent ("Blend Exponent", Range(0.5,20) ) = 2
		_lightingStrength ("Lighting Strength Factor", float ) = 1
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		
//---------------------------------------------------------------------------------//
		
CGPROGRAM
#pragma surface surf ToonRamp vertex:vert addshadow
#pragma target 3.0

sampler2D _Ramp;
sampler2D _Diffuse2;
sampler2D _BumpMap;
float _Diffuse2BlendFactor;
sampler2D _MainTex;
float4 _Color;
float _uvScale;
float _blendExponent;
float _lightingStrength;

struct MySurfaceOutput
{
	fixed3 Albedo;
	fixed3 Albedo2;
	fixed2 phiH;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Gloss;
	fixed Alpha;
};

// custom lighting function that uses a texture ramp based
// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonRamp (MySurfaceOutput s, half3 lightDir, half atten)
{
	#ifndef USING_DIRECTIONAL_LIGHT
	lightDir = normalize(lightDir);
	#endif
	
	
	fixed diff = saturate(max(0, dot (s.Normal, lightDir)) * _lightingStrength) ;
	
	half d = dot (s.Normal, lightDir)*0.5 + 0.5;
	half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	
	half4 c;
	c.rgb = s.Albedo * lerp(float3(1,1,1), s.Albedo2, _Diffuse2BlendFactor * (1-ramp.x)) * (diff * atten * 2) * _LightColor0.rgb;
	c.a = 0;
	return c;
}


struct Input
{
	float3 worldPos;
	float3 worldNrml;
	float2 uv_MainTex : TEXCOORD0;
	float2 uv_BumpMap : TEXCOORD2;
};

float3 calcTriPlanarBlendFactors(float3 worldNrml)
{
	float3 b = pow(abs(worldNrml), _blendExponent);
	b /= dot(b, float3(1,1,1));
	return b;
}

void vert(inout appdata_full v, out Input o)
{
	o = (Input)0;
	o.worldNrml = normalize( mul( (float3x3)_Object2World, SCALED_NORMAL ) );
}

void sampleTex(out float3 colDiff, in float2 UVs, in sampler2D texDiff)
{
    float4 rgbHeight = tex2D (texDiff, UVs );
  	colDiff = rgbHeight.rgb;
}

void surf (Input IN, inout MySurfaceOutput o) 
{
	//triplanar projection for the masking diffuse
	IN.worldNrml = normalize( IN.worldNrml );
	float3x3 colorMatrix;
	
	float2 uv = float2( IN.worldPos.x,-IN.worldPos.z ) * _uvScale;
	sampleTex(colorMatrix[1], uv, _Diffuse2);
	uv = float2( IN.worldPos.z,-IN.worldPos.y ) * _uvScale;
	sampleTex(colorMatrix[0], uv, _Diffuse2);
	uv = float2( IN.worldPos.x,-IN.worldPos.y ) * _uvScale;
	sampleTex(colorMatrix[2], uv, _Diffuse2);
	
	//blend together
	o.Albedo2 = mul(calcTriPlanarBlendFactors(IN.worldNrml), colorMatrix).rgb;
	
	//======================================================//
	
	half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}

ENDCG

//---------------------------------------------------------------------------------//

	} Fallback "Bumped"
}

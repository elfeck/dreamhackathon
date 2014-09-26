Shader "stillalive-studios/Masked Lighting/Bumped Specular"
{
	Properties
	{
		_Color ("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex ("Base (RGB), Specularity (A)", 2D) = "white" {}
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Diffuse2 ("Second Diffuse", 2D) = "white" {}
		_Diffuse2BlendFactor ("Diffuse 2 Additional Blend Factor", Range(0,1)) = 1
		_Ramp ("Toon Ramp (RGB)", 2D) = "white" {} 
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
float4 _Diffuse2_ST;
sampler2D _BumpMap;
float _Diffuse2BlendFactor;
sampler2D _MainTex;
float4 _Color;
float _uvScale;
float _blendExponent;
float _lightingStrength;
float _Shininess;

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
	float4 screenPos;
	float2 uv_MainTex : TEXCOORD0;
	float2 uv_BumpMap : TEXCOORD1;
};

void vert(inout appdata_full v, out Input o)
{
	//calc stuff to get pixel coords
	o.screenPos = ComputeGrabScreenPos(mul(UNITY_MATRIX_MVP, v.vertex));
}

void surf(Input IN, inout MySurfaceOutput o) 
{
	float2 screenPos = IN.screenPos.xy / IN.screenPos.w;
	screenPos = TRANSFORM_TEX(screenPos, _Diffuse2);
	
	//blend together
	o.Albedo2 = tex2D(_Diffuse2, screenPos);
	
	//======================================================//
	
	half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	half4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
	o.Gloss = tex.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}

ENDCG

//---------------------------------------------------------------------------------//

	} Fallback "Bumped"
}

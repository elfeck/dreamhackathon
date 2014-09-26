Shader "stillalive-studios/Special Purpose/Double Diffuse Glow + Vertex Animation"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_GlowColor ("Glow Color", Color) = (1,1,1,1)
	_GlowIntensity ("Glow Intensity", float) = 1
	_Diffuse0 ("Diffuse 1 (RGB), Glow 1 (A)", 2D) = "white" {}
	_Diffuse1 ("Diffuse 2 (RGB), Glow 2 (A)", 2D) = "white" {}
	blendFactor ("Blend Factor between 1 and 2", Range(0,1)) = 0.5
	
	//animation
	_AnimPowerFreq ("Animation Power (XYZ), Frequency (W)", vector) = (0,0.1,0,1)
	_AnimOffsetSpeed ("Animation Offset (XYZ), Speed (W)", vector) = (10,0,0,10)
}

SubShader
{
	Tags { "RenderType"="Opaque" }

CGPROGRAM
#pragma surface surf Lambert vertex:vert

sampler2D _Diffuse0;
sampler2D _Diffuse1;
fixed4 _Color;
fixed4 _GlowColor;
float _GlowIntensity;
float blendFactor;
//animation
half4 _AnimPowerFreq;
half4 _AnimOffsetSpeed;

struct Input
{
	float2 uv_Diffuse0;
	float2 uv_Diffuse1;
};

void vert(inout appdata_full v)
{
	//animation
	half3 animOffset = _AnimOffsetSpeed.xyz * v.vertex.xyz;
	v.vertex.xyz += sin(_Time.y * _AnimOffsetSpeed.w + (animOffset.x+animOffset.y+animOffset.z) * _AnimPowerFreq.w) * _AnimPowerFreq.xyz;
}

void surf(Input IN, inout SurfaceOutput o)
{
	fixed4 c0 = tex2D(_Diffuse0, IN.uv_Diffuse0);
	fixed4 c1 = tex2D(_Diffuse1, IN.uv_Diffuse1);
	fixed4 c = lerp(c0, c1, blendFactor);
	o.Albedo = c.rgb * _Color.rgb;
	o.Alpha = 1;
	o.Emission = _GlowColor * _GlowIntensity * c.a;
}
ENDCG  
}

FallBack "Diffuse"
}

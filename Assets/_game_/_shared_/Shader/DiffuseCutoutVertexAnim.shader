Shader "stillalive-studios/Special Purpose/Diffuse Cutout TwoSided + Vertex Animation"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_GlowColor ("Glow Color", Color) = (1,1,1,1)
	_GlowIntensity ("Glow Intensity", float) = 1
	_MainTex ("Diffuse (RGB), Alpha (A)", 2D) = "white" {}
	_CutOff ("Alpha Cutoff", Range(0,1)) = 0.5
	
	//animation
	_AnimPowerFreq ("Animation Power (XYZ), Frequency (W)", vector) = (0,0.1,0,1)
	_AnimOffsetSpeed ("Animation Offset (XYZ), Speed (W)", vector) = (10,0,0,10)
}

SubShader
{
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	Cull Off

CGPROGRAM
#pragma surface surf Lambert alphatest:_CutOff vertex:vert

sampler2D _MainTex;
fixed4 _Color;
fixed4 _GlowColor;
float _GlowIntensity;
//animation
half4 _AnimPowerFreq;
half4 _AnimOffsetSpeed;

struct Input
{
	float2 uv_MainTex;
};

void vert(inout appdata_full v)
{
	//animation
	half3 animOffset = _AnimOffsetSpeed.xyz * v.vertex.xyz;
	v.vertex.xyz += sin(_Time.y * _AnimOffsetSpeed.w + (animOffset.x+animOffset.y+animOffset.z) * _AnimPowerFreq.w) * _AnimPowerFreq.xyz;
	
	float4 pos = mul(_Object2World, v.vertex);
	//flip normal to always look into the hemisphere where the camera is
	v.normal *= sign(dot(_WorldSpaceCameraPos-pos.xyz, mul((float3x3)_Object2World, SCALED_NORMAL)));
}

void surf(Input IN, inout SurfaceOutput o)
{
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = c.rgb * _Color.rgb;
	o.Alpha = c.a * _Color.a;
	o.Emission = _GlowColor * _GlowIntensity;
}
ENDCG  
}

FallBack "Diffuse"
}

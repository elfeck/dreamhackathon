Shader "stillalive-studios/Special Purpose/Bumped Specular Alpha Vegetation (Animated)"
{
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color RGB, Gloss A", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	_amplitudes ("Amplitudes of the different animation waves", vector) = (0.1, 0.03, 0.03, 0.01)
	_frequencies ("Frequencies for the different animation waves", vector) = (1.5, 4.1, 6.3, 10.05)
	_phaseShifts ("Additional phase shifts for the harmonics", vector) = (0,0,0,0)
	_windDirection ("Main direction of the wind", vector) = (1,0,0,0)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
}
SubShader
{ 
	Tags { "Queue" = "Transparent" }
	LOD 400
	Cull off
	
CGPROGRAM
#pragma surface surf Lambert alpha vertex:vert addshadow

sampler2D _MainTex;
sampler2D _BumpMap;
fixed4 _Color;
half _Shininess;
float4 _amplitudes;
float4 _frequencies;
float4 _windDirection;
float4 _phaseShifts;

//inline float getRandomFromData(appdata_full v)
//{
//	//return frac(frac(dot(v.vertex.xyz, float3(256f, 391f, 219f))) + frac(dot(v.normal, float3(182f, 884f, 384f))) 
//	//	+ frac(dot(v.texcoord.xy, float2(8347f, 48539f))));
//	return frac(dot(v.normal, float3(182f, 884f, 384f)));
//}

void vert (inout appdata_full v)
{
	//move the vertices according to the weight in the color alpha channel
	float3 dir = float3(1,1,1);
	dir = normalize(dot(dir, v.normal) * v.normal + dot(dir, v.tangent.xyz) * v.tangent.xyz);
	dir = dot(dir, _windDirection.xyz) * _windDirection.xyz;
	v.vertex.xyz += v.color.a * dir * (_amplitudes.x * sin(_frequencies.x * _Time.y + _phaseShifts.x) 
		+ _amplitudes.y * sin(_frequencies.y * _Time.y + _phaseShifts.y)
		+ _amplitudes.z * sin(_frequencies.z * _Time.y + _phaseShifts.z) 
		+ _amplitudes.w * sin(_frequencies.w * _Time.y + _phaseShifts.w));
	
	float4 pos = mul(_Object2World, v.vertex);
	//flip normal to always look into the hemisphere where the camera is
	v.normal *= sign(dot(_WorldSpaceCameraPos-pos.xyz, mul((float3x3)_Object2World, SCALED_NORMAL)));
}

struct Input
{
	float2 uv_MainTex;
	float2 uv_BumpMap;
};

void surf (Input IN, inout SurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = _SpecColor.a;
	o.Alpha = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
}

ENDCG
	}
}

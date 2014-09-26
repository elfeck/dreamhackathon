Shader "stillalive-studios/Special Purpose/Uniform Diffuse (no Normals)" {
Properties
{
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Texture", 2D) = "white" {}
}
SubShader
{
	Tags { "RenderType" = "Opaque" }

CGPROGRAM
#pragma surface surf UniformLightingModel

struct SpecularMapSurfaceOutput
{
    half3 Albedo;
    half Alpha;
    
    //unused
    half3 Normal;
    half3 Emission;
    half3 Specular;
};

inline half4 LightingUniformLightingModel(SpecularMapSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb) * (atten * 2);
	c.a = s.Alpha;
	return c;
}
 
inline half4 LightingUniformLightingModel_PrePass(SpecularMapSurfaceOutput s, half4 light)
{
	half4 c;
	c.rgb = (s.Albedo * light.rgb);
	c.a = s.Alpha;
	return c;
}

inline half4 LightingUniformLightingModel_DirLightmap(SpecularMapSurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, 
	bool surfFuncWritesNormal, out half3 specColor)
{
	half3 lm = DecodeLightmap(color);
	specColor = lm;
	return half4(lm, 0);
}

//---------------------------------------------------------------------------------//

 
struct Input
{
	float2 uv_MainTex;
};

sampler2D _MainTex;
float4 _Color;

void surf(Input IN, inout SpecularMapSurfaceOutput o)
{
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
}


ENDCG
}

}
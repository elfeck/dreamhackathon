///This shader implements a BlinnPhong light model, but with support for a specular color!

struct SpecularMapSurfaceOutput
{
    half3 Albedo;
    half3 Normal;
    half3 Emission;
    half Specular;
    half3 GlossColor;
    half Alpha;
};

inline half4 LightingBlinnPhongSpecularMap(SpecularMapSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	half diff = max (0, dot (s.Normal, lightDir));
	
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular*128.0);
	half3 specCol = spec * s.GlossColor;
	
	half4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * specCol) * (atten * 2);
	c.a = s.Alpha;
	return c;
}
 
inline half4 LightingBlinnPhongSpecularMap_PrePass(SpecularMapSurfaceOutput s, half4 light)
{
	half3 spec = light.a * s.GlossColor;
	
	half4 c;
	c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
	c.a = s.Alpha + light.a * _SpecColor.a;
	return c;
}

inline half4 LightingBlinnPhongSpecularMap_DirLightmap(SpecularMapSurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, 
	bool surfFuncWritesNormal, out half3 specColor)
{
	UNITY_DIRBASIS
	half3 scalePerBasisVector;
	
	half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
	
	half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
	half3 h = normalize (lightDir + viewDir);

	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular * 128.0);
	half3 sc = spec * s.GlossColor;
	
	// specColor used outside in the forward path, compiled out in prepass
	specColor = lm * sc.rgb * spec;
	
	// spec from the alpha component is used to calculate specular
	// in the Lighting*_Prepass function, it's not used in forward
	return half4(lm, spec);
}
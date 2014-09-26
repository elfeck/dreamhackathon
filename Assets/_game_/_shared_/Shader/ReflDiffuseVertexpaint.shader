Shader "stillalive-studios/Reflective Surface/Diffuse Vertex-Paint" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_colorBlendingFactor ("Vertex Color Blending Factor", Range (0, 1)) = 1
	_ReflectColor ("Reflection Color", Color) = (1,1,1,1)
	reflectionStrength ("Strength of the Reflection", Range(0,1)) = 1
	_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
}
SubShader {
	LOD 200
	Tags { "RenderType"="Opaque" }
	
CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
samplerCUBE _Cube;

fixed4 _Color;
float _colorBlendingFactor;
fixed4 _ReflectColor;
float reflectionStrength;

struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
	float4 color : COLOR;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color * lerp(float4(1,1,1,1), IN.color, _colorBlendingFactor);
	o.Albedo = c.rgb;
	
	fixed4 reflcol = texCUBE (_Cube, IN.worldRefl);
	reflcol *= IN.color.r * reflectionStrength;
	o.Emission = reflcol.rgb * _ReflectColor.rgb;
	o.Alpha = tex.a;
}
ENDCG
}
	
FallBack "Reflective/VertexLit"
} 

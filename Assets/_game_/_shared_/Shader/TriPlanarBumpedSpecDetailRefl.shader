Shader "stillalive-studios/Triplanar/Triplanar Bumped Specular Detail Reflection"
{
	Properties
	{
		_MainColor_Top ("Top Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Top("Top: UV Scale", Float) = 1
		_TexDiff_Top ("Top: Diffuse (RGB) + Spec (A)", 2D) = "white" {}
		_TexBump_Top ("Top: Normal Map (RGB), Reflective (A)", 2D) = "bump" {}

		_MainColor_Side ("Side Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Side("Side: UV Scale", Float) = 1
		_TexDiff_Side ("Side: Diffuse (RGB) + Spec (A)", 2D) = "white" {}
		_TexBump_Side ("Side: Normal Map (RGB), Reflective (A)", 2D) = "bump" {}

		_blendExponent ( "Blend Exponent", Range(0.5,20) ) = 2
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_ReflectionStrength ("Reflection Strength", float) = 0.5
		_ReflectionColor ( "Reflection Color", Color ) = (1,1,1,1)
		_ReflectionMap ("Reflection Cubemap", Cube) = "white" {}
		
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
		#pragma exclude_renderers gles
		#pragma surface surf BlinnPhong vertex:vert
		#pragma target 3.0
		#pragma glsl
		
		//###############################################################//
		
		fixed4 _MainColor_Top;
		sampler2D _TexDiff_Top;
		sampler2D _TexBump_Top;
		sampler2D _TexSpec_Top;
		float _ScaleUV_Top;
		
		fixed4 _MainColor_Side;
		sampler2D _TexDiff_Side;
		sampler2D _TexBump_Side;
		sampler2D _TexSpec_Side;
		float _ScaleUV_Side;
		
		float _ReflectionStrength;
		float4 _ReflectionColor;
		samplerCUBE _ReflectionMap;
		float _Shininess;
		float _blendExponent;
		
		//###############################################################//
		
		struct Input
		{
			float3 worldPos;
			float3 worldNrml;
			float3 worldRefl; INTERNAL_DATA
		};
		
		//###############################################################//
		
		float3 CalcTriPlanarBlendFactors(float3 worldNrml)
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
		
		//###############################################################//
		
		void sampleTexs(out float3 colDiff, out float3 nrml, out float spec, out float refl,
			in float2 UVs, in sampler2D texDiff, in sampler2D texNrml)
		{
		    float4 rgbSpec = tex2D (texDiff, UVs );
		  	colDiff = rgbSpec.rgb;
		  	float4 bump = tex2D (texNrml, UVs );
		  	nrml = UnpackNormal( bump ).xyz;
		  	spec = rgbSpec.w;
		  	refl = bump.w;
		}
		
		void surf(Input IN, inout SurfaceOutput o) 
		{
			IN.worldNrml = normalize( IN.worldNrml );
			
			float3x3 colorMatrix;
			float3x3 nrmlMatrix;
			float3 specMatrix;
			float3 reflMatrix;
			
			//--------------------------------------------------------------------//
			//Sample TOP views
			float2 uvTop = float2( IN.worldPos.x,-IN.worldPos.z ) * _ScaleUV_Top;
			
			sampleTexs(colorMatrix[1], nrmlMatrix[1], specMatrix[1], reflMatrix[1], uvTop, _TexDiff_Top, _TexBump_Top );
			colorMatrix[1] *= _MainColor_Top.rgb;
			
			//--------------------------------------------------------------------//
			//Sample Side views			
			float2 uvRight = float2( IN.worldPos.z,-IN.worldPos.y ) * _ScaleUV_Side;

			sampleTexs(colorMatrix[0], nrmlMatrix[0], specMatrix[0], reflMatrix[0], uvRight, _TexDiff_Side, _TexBump_Side );
			colorMatrix[0] *= _MainColor_Side.rgb;
			
			//--------------------------------------------------------------------//
	 		//Sample Front views
			float2 uvFront = float2( IN.worldPos.x,-IN.worldPos.y ) * _ScaleUV_Side;
			
			sampleTexs(colorMatrix[2], nrmlMatrix[2], specMatrix[2], reflMatrix[2], uvFront, _TexDiff_Side, _TexBump_Side );
			colorMatrix[2] *= _MainColor_Side.rgb;
			
			//--------------------------------------------------------------------//
			//blend together
			float3 blendF = CalcTriPlanarBlendFactors(IN.worldNrml);
		
			float3 colFinal = mul( blendF, colorMatrix );
			o.Normal = normalize( mul( blendF, nrmlMatrix ) );
			o.Albedo = colFinal.rgb;
			
			float specFinal = mul( blendF, specMatrix );
			o.Specular = _Shininess;
			o.Gloss = specFinal; // specular level (multplicator)
			
			//reflection
			float reflFactor = mul(blendF, reflMatrix);
			float3 refl = WorldReflectionVector(IN, o.Normal);
			o.Emission = texCUBE(_ReflectionMap, refl).rgb * _ReflectionStrength * _ReflectionColor.rbg * reflFactor;
		}
		ENDCG
    } 
	FallBack "stillalive-studios/Triplanar/Triplanar Bumped Specular"
}

Shader "stillalive-studios/Triplanar/Triplanar Bumped Specular" {
	Properties {
		_MainColor_Top ("Top Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Top("Top: UV Scale", Float) = 1
		_TexDiff_Top ("Top: Diffuse (RGB) + Height (A)", 2D) = "white" {}
		_TexBump_Top ("Top: Normal Map", 2D) = "bump" {}
		_TexSpec_Top ("Top: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}

		_MainColor_Side ("Side Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Side("Side: UV Scale", Float) = 1
		_TexDiff_Side ("Side: Diffuse (RGB) + Height (A)", 2D) = "white" {}
		_TexBump_Side ("Side: Normal Map", 2D) = "bump" {}
		_TexSpec_Side ("Side: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}

		_blendExponent ( "Blend Exponent", Range(0.5,20) ) = 2
		_blendOffset ( "Blend Strength Offset", Range(0.01,1) ) = 0.1
		_SpecColor ( "Specular Color", Color ) = (0.5,0.5,0.5)
		
		
	}
	SubShader {
      Tags { "RenderType" = "Opaque" }
		CGPROGRAM
//		#pragma debug
		// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
		#pragma exclude_renderers gles
		#pragma surface surf BlinnPhong vertex:vert
		#pragma target 3.0
		
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
		
		float _blendExponent;
		float _blendOffset;
		
		//###############################################################//
		
		struct Input
		{
			float3 worldPos;
			float3 worldNrml;
			float3 viewDir_tangSpace;
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
		
		void SampleTexWithSpec(out float3 colDiff, out float3 nrml, out float2 spec,
					  in float2 UVs, in sampler2D texDiff, in sampler2D texNrml, in sampler2D texSpec)
		{
		    float4 rgbHeight = tex2D (texDiff, UVs );
		  	colDiff = rgbHeight.rgb;
		  	float4 bump = tex2D (texNrml, UVs );
		  	nrml = UnpackNormal( bump ).xyz;
		  	spec = tex2D ( texSpec, UVs ).wy;
		}
		
		void surf(Input IN, inout SurfaceOutput o) 
		{
			IN.worldNrml = normalize( IN.worldNrml );
			
			float3x3 colorMatrix;
			float3x3 nrmlMatrix;
			float3x2 specMatrix;
			
			//--------------------------------------------------------------------//
			//Sample TOP views
			
			float2 uvTop = float2( IN.worldPos.x,-IN.worldPos.z )* _ScaleUV_Top;
			
			SampleTexWithSpec(colorMatrix[1], nrmlMatrix[1], specMatrix[1],
				uvTop, _TexDiff_Top, _TexBump_Top, _TexSpec_Top );
			colorMatrix[1] *= _MainColor_Top.rgb;
			
			//--------------------------------------------------------------------//
			//Sample Side views			
			float2 uvRight = float2( IN.worldPos.z,-IN.worldPos.y )* _ScaleUV_Side;

			SampleTexWithSpec(colorMatrix[0], nrmlMatrix[0], specMatrix[0],	
				uvRight, _TexDiff_Side, _TexBump_Side, _TexSpec_Side );
			colorMatrix[0] *= _MainColor_Side.rgb;
			
			//--------------------------------------------------------------------//
	 		//Sample Front views
			float2 uvFront = float2( IN.worldPos.x,-IN.worldPos.y )* _ScaleUV_Side;
			
			SampleTexWithSpec(colorMatrix[2], nrmlMatrix[2], specMatrix[2],
				uvFront, _TexDiff_Side, _TexBump_Side, _TexSpec_Side );
			colorMatrix[2] *= _MainColor_Side.rgb;
			
			
			//--------------------------------------------------------------------//
			//blend together
			float3 blendF = CalcTriPlanarBlendFactors(IN.worldNrml);
		
			float3 colFinal = mul( blendF, colorMatrix );
			o.Normal = normalize( mul( blendF, nrmlMatrix ) );
			o.Albedo = colFinal.rgb;
			
			float2 specFinal = mul( blendF, specMatrix );
			o.Specular = clamp( specFinal.x, 0.05, 1);  // glossiness (exponent)
			o.Gloss = specFinal.y; // specular level (multplicator)
		}
		ENDCG
    } 
	FallBack "Diffuse"
}

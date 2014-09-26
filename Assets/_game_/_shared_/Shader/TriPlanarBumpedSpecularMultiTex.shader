Shader "stillalive-studios/Triplanar/Triplanar Bumped Specular Multi-Texture" {
	Properties
	{
		_ScaleUV_TopControl ("Top Control: UV Scale", Float) = 1
		_Control_Top ("Top Control: Greyscale", 2D) = "white" {}
	
		_MainColor_Top0 ("Top0 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Top0 ("Top0: UV Scale", Float) = 1
		_TexDiff_Top0 ("Top0: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Top0 ("Top0: Normal Map", 2D) = "bump" {}
		_TexSpec_Top0 ("Top0: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}
		
		_MainColor_Top1 ("Top1 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Top1 ("Top1: UV Scale", Float) = 1
		_TexDiff_Top1 ("Top1: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Top1 ("Top1: Normal Map", 2D) = "bump" {}
		_TexSpec_Top1 ("Top1: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}
		
		
		_ScaleUV_SideControl ("Side Control: UV Scale", Float) = 1
		_Control_Side ("Side Control: Greyscale", 2D) = "white" {}

		_MainColor_Side0 ("Side0 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Side0 ("Side0: UV Scale", Float) = 1
		_TexDiff_Side0 ("Side0: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Side0 ("Side0: Normal Map", 2D) = "bump" {}
		_TexSpec_Side0 ("Side0: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}
		
		_MainColor_Side1 ("Side1 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Side1 ("Side1: UV Scale", Float) = 1
		_TexDiff_Side1 ("Side1: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Side1 ("Side1: Normal Map", 2D) = "bump" {}
		_TexSpec_Side1 ("Side1: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}

		_blendExponent ( "Blend Exponent", Range(0.5,20) ) = 2
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

	
		CGPROGRAM
//		#pragma debug
		// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
		#pragma exclude_renderers gles
		#pragma surface surf BlinnPhong vertex:vert
		#pragma target 3.0
		
		//###############################################################//	
		
		sampler2D _Control_Top;
		float _ScaleUV_TopControl;
	
		fixed4 _MainColor_Top0;
		sampler2D _TexDiff_Top0;
		sampler2D _TexBump_Top0;
		sampler2D _TexSpec_Top0;
		float _ScaleUV_Top0;
		
		fixed4 _MainColor_Top1;
		sampler2D _TexDiff_Top1;
		sampler2D _TexBump_Top1;
		sampler2D _TexSpec_Top1;
		float _ScaleUV_Top1;
		
		
		sampler2D _Control_Side;
		float _ScaleUV_SideControl;
		
		fixed4 _MainColor_Side0;
		sampler2D _TexDiff_Side0;
		sampler2D _TexBump_Side0;
		sampler2D _TexSpec_Side0;
		float _ScaleUV_Side0;
		
		fixed4 _MainColor_Side1;
		sampler2D _TexDiff_Side1;
		sampler2D _TexBump_Side1;
		sampler2D _TexSpec_Side1;
		float _ScaleUV_Side1;

		float _blendExponent;
		float _Shininess;
		
		//###############################################################//
		
		struct Input
		{
			float3 worldPos;
			float3 worldNrml;
		};
		
		//###############################################################//
	      
		float3 CalcTriPlanarBlendFactors( float3 worldNrml )
		{
			float3 b = pow(abs(worldNrml), _blendExponent);
			b /= dot(b, float3(1,1,1)); //normalize: x + y + z == 1
			return b;
		}
		
		void vert (inout appdata_full v, out Input o) 
		{
			o = (Input)0;
			o.worldNrml = normalize( mul( (float3x3)_Object2World, SCALED_NORMAL ) );
		}
	
		//###############################################################//
		
		void SampleTexWithSpec(out float3 colDiff, out float3 nrml, out float spec,
					  in float2 UVs, in sampler2D texDiff, in sampler2D texNrml, in sampler2D texSpec)
		{
		    float4 rgbSpec = tex2D (texDiff, UVs);
		  	colDiff = rgbSpec.rgb;
		  	spec = rgbSpec.w;
		  	float4 bump = tex2D (texNrml, UVs);
		  	nrml = UnpackNormal(bump).xyz;
		}
		
		//###############################################################//
	
		void surf (Input IN, inout SurfaceOutput o) 
		{
	 		IN.worldNrml = normalize(IN.worldNrml);
	 		
	 		float2 uvProj;
			
			float3x3 colorMatrix;
	 		float3x3 nrmlMatrix;
			float3 specMatrix;
			
			float4 blending;
			float3 tmpDiff0;
			float3 tmpDiff1;
			float3 tmpNrm0;
			float3 tmpNrm1;
			float tmpSpec0;
			float tmpSpec1;
			
			//--------------------------------------------------------------------//
			//Sample TOP views
	
			uvProj = float2( IN.worldPos.x,-IN.worldPos.z )* _ScaleUV_Top0;
	 		SampleTexWithSpec(tmpDiff0, tmpNrm0, tmpSpec0, uvProj, _TexDiff_Top0, _TexBump_Top0, _TexSpec_Top0 );
			tmpDiff0 *= _MainColor_Top0.rgb;
			
			uvProj = float2( IN.worldPos.x,-IN.worldPos.z )* _ScaleUV_Top1;
	 		SampleTexWithSpec(tmpDiff1, tmpNrm1, tmpSpec1, uvProj, _TexDiff_Top1, _TexBump_Top1, _TexSpec_Top1 );
			tmpDiff1 *= _MainColor_Top1.rgb;
			
			
			uvProj = float2( IN.worldPos.x,-IN.worldPos.z )* _ScaleUV_TopControl;
			blending = tex2D(_Control_Top, uvProj);
			colorMatrix[1] = lerp(tmpDiff0, tmpDiff1, blending.r);
			nrmlMatrix[1] = lerp(tmpNrm0, tmpNrm1, blending.r);
			specMatrix[1] = lerp(tmpSpec0, tmpSpec1, blending.r);
			
			//--------------------------------------------------------------------//
			//Sample Side views
	
			uvProj = float2( IN.worldPos.z,-IN.worldPos.y )* _ScaleUV_Side0;
	 		SampleTexWithSpec(tmpDiff0, tmpNrm0, tmpSpec0, uvProj, _TexDiff_Side0, _TexBump_Side0, _TexSpec_Side0 );
	 		tmpDiff0 *= _MainColor_Side0.rgb;
	 		
	 		uvProj = float2( IN.worldPos.z,-IN.worldPos.y )* _ScaleUV_Side1;
	 		SampleTexWithSpec(tmpDiff1, tmpNrm1, tmpSpec1, uvProj, _TexDiff_Side1, _TexBump_Side1, _TexSpec_Side1 );
	 		tmpDiff1 *= _MainColor_Side1.rgb;
	 		
	 		
	 		uvProj = float2( IN.worldPos.z,-IN.worldPos.y ) * _ScaleUV_SideControl;
			blending = tex2D(_Control_Side, uvProj);
			colorMatrix[0] = lerp(tmpDiff0, tmpDiff1, blending.r);
			nrmlMatrix[0] = lerp(tmpNrm0, tmpNrm1, blending.r);
			specMatrix[0] = lerp(tmpSpec0, tmpSpec1, blending.r);
	 		
	 		//--------------------------------------------------------------------//
	 		//Sample Front views
	 		
	      	uvProj = float2( IN.worldPos.x,-IN.worldPos.y )* _ScaleUV_Side0;
	 		SampleTexWithSpec(tmpDiff0, tmpNrm0, tmpSpec0, uvProj, _TexDiff_Side0, _TexBump_Side0, _TexSpec_Side0 );
	 	 	tmpDiff0 *= _MainColor_Side0.rgb;
	 	 	
	 	 	uvProj = float2( IN.worldPos.x,-IN.worldPos.y )* _ScaleUV_Side1;
	 		SampleTexWithSpec(tmpDiff1, tmpNrm1, tmpSpec1, uvProj, _TexDiff_Side1, _TexBump_Side1, _TexSpec_Side1 );
	 	 	tmpDiff1 *= _MainColor_Side1.rgb;
	 	 	
	 	 	
	 	 	uvProj = float2( IN.worldPos.x,-IN.worldPos.y )* _ScaleUV_SideControl;
	 		blending = tex2D(_Control_Side, uvProj);
	 		colorMatrix[2] = lerp(tmpDiff0, tmpDiff1, blending.r);
			nrmlMatrix[2] = lerp(tmpNrm0, tmpNrm1, blending.r);
			specMatrix[2] = lerp(tmpSpec0, tmpSpec1, blending.r);
	      	
	      	
	      	
	      	//--------------------------------------------------------------------//
			//blend together
			float3 blendF = CalcTriPlanarBlendFactors(IN.worldNrml);
			
			o.Albedo = mul(blendF, colorMatrix);
			o.Normal = normalize(mul(blendF, nrmlMatrix));
			o.Specular = _Shininess;  // glossiness (exponent)
			o.Gloss = dot(blendF, specMatrix); // specular level (multplicator)
		}
		
		
		ENDCG    
    } 
	FallBack "stillalive-studios/Triplanar/Triplanar Bumped Specular"
}

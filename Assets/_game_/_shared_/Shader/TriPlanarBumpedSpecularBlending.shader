Shader "stillalive-studios/Triplanar/Triplanar Bumped Specular Blended" {
	Properties {
		_MainTex ("Main base Texture", 2D) = "white" {}
		_BumpMap ("Main base Normal Map", 2D) = "bump" {}
	
		_TexDiff_Top ("Top: Diffuse (RGB) + Height (A)", 2D) = "white" {}
		_TexBump_Top ("Top: Normal Map", 2D) = "bump" {}
		_TexSpec_Top ("Top: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}
		_ScaleUV_Top("Top: UV Scale", Float) = 1
		_ScaleHeight_Top ( "Top: Blend Scale", Range(0,100) ) = 50
		_ScaleParallax_Top( "Top: Parallax Scale", Range(0,0.2) ) = 0.1

		_TexDiff_Side ("Side: Diffuse (RGB) + Height (A)", 2D) = "white" {}
		_TexBump_Side ("Side: Normal Map", 2D) = "bump" {}
		_TexSpec_Side ("Side: Specularity Strength (G) + Glossiness (A)", 2D) = "black" {}
		_ScaleUV_Side("Side: UV Scale", Float) = 1
		_ScaleHeight_Side ( "Side: Blend Scale", Range(0,100) ) = 50
		_ScaleParallax_Side( "Side: Parallax Scale", Range(0,0.2) ) = 0.1

		_blendExponent ( "Blend Exponent", Range(0.5,20) ) = 2
		_blendOffset ( "Blend Strength Offset", Range(0.01,1) ) = 0.1
		_SpecColor ( "Specular Color", Color ) = (0.5,0.5,0.5)		
	}
	SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma debug
// Upgrade NOTE: excluded shader from OpenGL ES 2.0 because it uses non-square matrices
#pragma exclude_renderers gles
      #pragma surface surf BlinnPhong vertex:vert
      #pragma target 3.0
      struct Input {
        float3 worldPos;
        float3 worldNrml;
        float3 viewDir_tangSpace;
        float2 uv_MainTex;
        float2 uv_BumpMap;
        float4 color : COLOR;
      };

	  sampler2D _MainTex;
	  sampler2D _BumpMap;

      sampler2D _TexDiff_Top;
      sampler2D _TexBump_Top;
      sampler2D _TexSpec_Top;
      float _ScaleUV_Top;
      float _ScaleHeight_Top;
      float _ScaleParallax_Top;

	  sampler2D _TexDiff_Side;
      sampler2D _TexBump_Side;
      sampler2D _TexSpec_Side;
      float _ScaleUV_Side;
      float _ScaleHeight_Side;
      float _ScaleParallax_Side;
        
      float _blendExponent;
      float _blendOffset;
      
      float3 CalcTriPlanarBlendFactors( float3 worldNrml )
      {
      		float3 b = pow( abs( worldNrml ), _blendExponent );
      					   
      		b /= dot( b, float3(1,1,1) );
      		
      		return b;
      }
      
      float3 CalcTriPlanarBlendFactors( float3 worldNrml, float3 height ) // right, top, front (X,Y,Z)
      {
      		float3 b = pow( abs( worldNrml ), _blendExponent );
      					   
      		b *= ( height + _blendOffset );
      		b /= dot( b, float3(1,1,1) );
      		
      		return b;
      }
      
      void vert (inout appdata_full v, out Input o) 
      {
      	o = (Input)0;
      	o.worldNrml = normalize( mul( (float3x3)_Object2World, SCALED_NORMAL ) );
      	float3 blendF = CalcTriPlanarBlendFactors( o.worldNrml );
      	float3 binormalWorld = blendF.x * float3(0,-1,0) + blendF.y * float3(0,0,-1) + blendF.z * float3(0,-1,0);
      	float3 tangentWorld = normalize( cross( binormalWorld, o.worldNrml ) );
      	binormalWorld = cross( o.worldNrml, tangentWorld );
      	float3x3 world2tangentSpace =  float3x3( tangentWorld, binormalWorld, o.worldNrml );
      	v.tangent.xyz = mul( (float3x3)_World2Object, tangentWorld );
      	v.tangent.w = 1;
      	
      	float4 worldPos = mul(_Object2World, v.vertex);
      	worldPos.xyz /= worldPos.w;
      	float3 viewDir_World = _WorldSpaceCameraPos - worldPos.xyz;
      	
      	o.viewDir_tangSpace = mul(world2tangentSpace, viewDir_World );
      }
      
      inline float2 MyParallaxOffset( float h, float parallaxScale, float3 viewDir_unit )
	  {
			h = (h * parallaxScale - parallaxScale/2.0);
			//h /= viewDir_unit.z + 0.5; // more correct, but requires iterations
			return h * viewDir_unit.xy ;
	  }

	  void SampleTexWithSpec( uniform bool useParallax,
	  				  out float3 colDiff, out float3 nrml, out float2 spec, out float height,
	  				  in float2 UVs, in float parallaxScale, in float3 viewDir_tangSpace, in sampler2D texDiff, in sampler2D texNrml, in sampler2D texSpec )
	  {
	  		if( useParallax )
 			{
 				height = tex2D ( texDiff, UVs ).a; 
	        	UVs += MyParallaxOffset( height, parallaxScale, viewDir_tangSpace );
	        }
	        float4 rgbHeight = tex2D (texDiff, UVs );
	      	colDiff = rgbHeight.rgb;
	      	height = rgbHeight.a;
	      	float4 bump = tex2D (texNrml, UVs );
	      	nrml = UnpackNormal( bump ).xyz;
	      	spec = tex2D ( texSpec, UVs ).wy;
	  }

      void surf (Input IN, inout SurfaceOutput o) 
      {
 		IN.worldNrml = normalize( IN.worldNrml );
 		IN.viewDir_tangSpace = normalize( IN.viewDir_tangSpace );
 		
 		float3 height;
 		float3x3 colorMatrix;
 		float3x3 nrmlMatrix;
		float3x2 specMatrix;
		
		// TOP

		float2 uvTop = float2( IN.worldPos.x,-IN.worldPos.z )* _ScaleUV_Top;
 
 		SampleTexWithSpec( true,
 		            /* OUT */ colorMatrix[1], nrmlMatrix[1], specMatrix[1], height.y, 
 					/* IN */ uvTop, _ScaleParallax_Top, IN.viewDir_tangSpace, 
 					         _TexDiff_Top, _TexBump_Top, _TexSpec_Top );
		
		// SIDE

		float2 uvRight = float2( IN.worldPos.z,-IN.worldPos.y )* _ScaleUV_Side;

 
 		SampleTexWithSpec( true,
 		            /* OUT */ colorMatrix[0], nrmlMatrix[0], specMatrix[0], height.x, 
 					/* IN */ uvRight, _ScaleParallax_Side, IN.viewDir_tangSpace, 
 					         _TexDiff_Side, _TexBump_Side, _TexSpec_Side );
 		
		// FRONT
      	
      	float2 uvFront = float2( IN.worldPos.x,-IN.worldPos.y )* _ScaleUV_Side;
 
 		SampleTexWithSpec( true,
 					/* OUT */ colorMatrix[2], nrmlMatrix[2], specMatrix[2], height.z, 
 					/* IN */ uvFront, _ScaleParallax_Side, IN.viewDir_tangSpace, 
 					         _TexDiff_Side, _TexBump_Side, _TexSpec_Side );
 					         
      	
		// blend together
      	float3 blendF = CalcTriPlanarBlendFactors( IN.worldNrml, height * float3( _ScaleHeight_Side, _ScaleHeight_Top, _ScaleHeight_Side ) );
 		
        float3 colFinal;
        
        colFinal = mul( blendF, colorMatrix );
        o.Normal = lerp(normalize( mul( blendF, nrmlMatrix ) ) , UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)), IN.color.a);
        fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = lerp(colFinal.rgb, mainColor.rgb, IN.color.a);
        
        float2 specFinal = mul( blendF, specMatrix );
        o.Specular = lerp(clamp( specFinal.x, 0.05, 1), mainColor.a, IN.color.a);  // glossiness (exponent)
        o.Gloss = specFinal.y; // specular level (multplicator)
      }
      ENDCG
    } 
	FallBack "Diffuse"
}

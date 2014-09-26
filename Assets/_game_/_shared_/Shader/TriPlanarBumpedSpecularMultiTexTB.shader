Shader "stillalive-studios/Triplanar/Triplanar Bumped Specular Multi-Texture Top-Bottom"
{
	Properties
	{
		_ScaleUV_TopControl ("Top/Bottom Control: UV Scale", Float) = 1
		_Control_Top ("Top/Bottom Control: Greyscale", 2D) = "white" {}
	
		_MainColor_Top0 ("Top0 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Top0 ("Top0: UV Scale", Float) = 1
		_TexDiff_Top0 ("Top0: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Top0 ("Top0: Normal Map", 2D) = "bump" {}
		
		_MainColor_Top1 ("Top1 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Top1 ("Top1: UV Scale", Float) = 1
		_TexDiff_Top1 ("Top1: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Top1 ("Top1: Normal Map", 2D) = "bump" {}
		
		_MainColor_Bottom0 ("Bottom0 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Bottom0("Bottom0: UV Scale", Float) = 1
		_TexDiff_Bottom0 ("Bottom0: Diffuse (RGB) + Height (A)", 2D) = "white" {}
		_TexBump_Bottom0 ("Bottom0: Normal Map", 2D) = "bump" {}
		
//		_MainColor_Bottom1 ("Bottom1 Color (RGB)", color) = (1,1,1,1)
//		_ScaleUV_Bottom1("Bottom1: UV Scale", Float) = 1
//		_TexDiff_Bottom1 ("Bottom1: Diffuse (RGB) + Height (A)", 2D) = "white" {}
//		_TexBump_Bottom1 ("Bottom1: Normal Map", 2D) = "bump" {}
		
		_SideBaseNormalWeight ("Side Base Normal Map Weight", Range(0,1)) = 0.5
		_ScaleUV_SideBase ("Side Base UV Scale", Float) = 1
		_TexBump_SideBase ("Side Base Normal Map", 2D) = "bump" {}
		
		_ScaleUV_SideControl ("Side Control: UV Scale", Float) = 1
		_Control_Side ("Side Control: Greyscale", 2D) = "white" {}

		_MainColor_Side0 ("Side0 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Side0 ("Side0: UV Scale", Float) = 1
		_TexDiff_Side0 ("Side0: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Side0 ("Side0: Normal Map", 2D) = "bump" {}
		
		_MainColor_Side1 ("Side1 Color (RGB)", color) = (1,1,1,1)
		_ScaleUV_Side1 ("Side1: UV Scale", Float) = 1
		_TexDiff_Side1 ("Side1: Diffuse (RGB) + Gloss (A)", 2D) = "white" {}
		_TexBump_Side1 ("Side1: Normal Map", 2D) = "bump" {}

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
		float _ScaleUV_Top0;
		
		fixed4 _MainColor_Top1;
		sampler2D _TexDiff_Top1;
		sampler2D _TexBump_Top1;
		float _ScaleUV_Top1;
		
		fixed4 _MainColor_Bottom0;
		sampler2D _TexDiff_Bottom0;
		sampler2D _TexBump_Bottom0;
		float _ScaleUV_Bottom0;
		
//		fixed4 _MainColor_Bottom1;
//		sampler2D _TexDiff_Bottom1;
//		sampler2D _TexBump_Bottom1;
//		float _ScaleUV_Bottom1;
		
		float _ScaleUV_SideBase;
		sampler2D _TexBump_SideBase;
		float _SideBaseNormalWeight;
		
		sampler2D _Control_Side;
		float _ScaleUV_SideControl;
		
		fixed4 _MainColor_Side0;
		sampler2D _TexDiff_Side0;
		sampler2D _TexBump_Side0;
		float _ScaleUV_Side0;
		
		fixed4 _MainColor_Side1;
		sampler2D _TexDiff_Side1;
		sampler2D _TexBump_Side1;
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
					  in float2 UVs, in sampler2D texDiff, in sampler2D texNrml)
		{
		    float4 rgbSpec = tex2D(texDiff, UVs);
		  	colDiff = rgbSpec.rgb;
		  	spec = rgbSpec.w;
		  	float4 bump = tex2D(texNrml, UVs);
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
			
			float3 bottomDiff;
			float3 bottomNrml;
			float bottomSpec;
			float bottomTopLerp = step(IN.worldNrml.y, 0);
			
			//sample 1st bottom texture
			uvProj = float2(IN.worldPos.x,-IN.worldPos.z) * _ScaleUV_Bottom0;
	 		SampleTexWithSpec(bottomDiff, bottomNrml, bottomSpec, uvProj, _TexDiff_Bottom0, _TexBump_Bottom0);
			bottomDiff *= _MainColor_Bottom0.rgb;
			//sample 1st top textures
			uvProj = float2(IN.worldPos.x,-IN.worldPos.z) * _ScaleUV_Top0;
	 		SampleTexWithSpec(tmpDiff0, tmpNrm0, tmpSpec0, uvProj, _TexDiff_Top0, _TexBump_Top0);
			tmpDiff0 *= _MainColor_Top0.rgb;
			//now mix them together
			tmpDiff0 = lerp(tmpDiff0, bottomDiff, bottomTopLerp);
			tmpNrm0 = lerp(tmpNrm0, bottomNrml, bottomTopLerp);
			tmpSpec0 = lerp(tmpSpec0, bottomSpec, bottomTopLerp);
			
			//2nd bottom texture (TEXTURE SAMPLE LIMIT!!)
//			uvProj = float2(IN.worldPos.x,-IN.worldPos.z) * _ScaleUV_Bottom1;
//	 		SampleTexWithSpec(bottomDiff, bottomNrml, bottomSpec, uvProj, _TexDiff_Bottom1, _TexBump_Bottom1);
//			bottomDiff *= _MainColor_Bottom1.rgb;
			//and 2nd top texture
			uvProj = float2(IN.worldPos.x,-IN.worldPos.z) * _ScaleUV_Top1;
	 		SampleTexWithSpec(tmpDiff1, tmpNrm1, tmpSpec1, uvProj, _TexDiff_Top1, _TexBump_Top1);
			tmpDiff1 *= _MainColor_Top1.rgb;
			//now mix them together
			tmpDiff1 = lerp(tmpDiff1, bottomDiff, bottomTopLerp);
			tmpNrm1 = lerp(tmpNrm1, bottomNrml, bottomTopLerp);
			tmpSpec1 = lerp(tmpSpec1, bottomSpec, bottomTopLerp);
			
			
			uvProj = float2( IN.worldPos.x,-IN.worldPos.z ) * _ScaleUV_TopControl;
			blending = tex2D(_Control_Top, uvProj);
			colorMatrix[1] = lerp(tmpDiff0, tmpDiff1, blending.r);
			nrmlMatrix[1] = lerp(tmpNrm0, tmpNrm1, blending.r);
			specMatrix[1] = lerp(tmpSpec0, tmpSpec1, blending.r);
			
			//--------------------------------------------------------------------//
			//Sample Side views
			
			uvProj = float2( IN.worldPos.z,-IN.worldPos.y ) * _ScaleUV_SideBase;
			float3 baseNormalSide = UnpackNormal(tex2D(_TexBump_SideBase, uvProj)).xyz;
	
			uvProj = float2( IN.worldPos.z,-IN.worldPos.y ) * _ScaleUV_Side0;
	 		SampleTexWithSpec(tmpDiff0, tmpNrm0, tmpSpec0, uvProj, _TexDiff_Side0, _TexBump_Side0);
	 		tmpDiff0 *= _MainColor_Side0.rgb;
	 		
	 		uvProj = float2( IN.worldPos.z,-IN.worldPos.y ) * _ScaleUV_Side1;
	 		SampleTexWithSpec(tmpDiff1, tmpNrm1, tmpSpec1, uvProj, _TexDiff_Side1, _TexBump_Side1);
	 		tmpDiff1 *= _MainColor_Side1.rgb;
	 		
	 		
	 		uvProj = float2( IN.worldPos.z,-IN.worldPos.y ) * _ScaleUV_SideControl;
			blending = tex2D(_Control_Side, uvProj);
			colorMatrix[0] = lerp(tmpDiff0, tmpDiff1, blending.r);
			nrmlMatrix[0] = normalize(_SideBaseNormalWeight * baseNormalSide + (1-_SideBaseNormalWeight) * lerp(tmpNrm0, tmpNrm1, blending.r));
			specMatrix[0] = lerp(tmpSpec0, tmpSpec1, blending.r);
	 		
	 		//--------------------------------------------------------------------//
	 		//Sample Front views
	 		
	 		uvProj = float2( IN.worldPos.x,-IN.worldPos.y ) * _ScaleUV_SideBase;
			float3 baseNormalFront = UnpackNormal(tex2D(_TexBump_SideBase, uvProj)).xyz;
	 		
	      	uvProj = float2( IN.worldPos.x,-IN.worldPos.y ) * _ScaleUV_Side0;
	 		SampleTexWithSpec(tmpDiff0, tmpNrm0, tmpSpec0, uvProj, _TexDiff_Side0, _TexBump_Side0);
	 	 	tmpDiff0 *= _MainColor_Side0.rgb;
	 	 	
	 	 	uvProj = float2( IN.worldPos.x,-IN.worldPos.y ) * _ScaleUV_Side1;
	 		SampleTexWithSpec(tmpDiff1, tmpNrm1, tmpSpec1, uvProj, _TexDiff_Side1, _TexBump_Side1);
	 	 	tmpDiff1 *= _MainColor_Side1.rgb;
	 	 	
	 	 	
	 	 	uvProj = float2( IN.worldPos.x,-IN.worldPos.y ) * _ScaleUV_SideControl;
	 		blending = tex2D(_Control_Side, uvProj);
	 		colorMatrix[2] = lerp(tmpDiff0, tmpDiff1, blending.r);
			nrmlMatrix[2] = normalize(_SideBaseNormalWeight * baseNormalFront + (1-_SideBaseNormalWeight) * lerp(tmpNrm0, tmpNrm1, blending.r));
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
	FallBack "stillalive-studios/Triplanar/Triplanar Bumped Specular Multi-Texture"
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("Image Effects/Texture Blender")]
public class TextureBlender : ImageEffectBase
{
	public Texture2D blendTexture;
	public float blendFactor = 0.2f;
	public float blendFactorSmoothing = 0f;
	public Color blendingColor = Color.white;
	
	private float _currBlendFactor = 0f;
	
	void Awake()
	{
		_currBlendFactor = blendFactor;
	}

	protected override void Start()
	{
		if(shader == null) shader = Shader.Find("Hidden/TextureBlender");

		base.Start();
	}
	
	void FixedUpdate()
	{
		HelperFunctions.applyRecursiveFilter(ref _currBlendFactor, blendFactor, 1f - blendFactorSmoothing);
	}
	
	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//only do it, when it's visible!
		if(blendFactor == 0f && blendTexture == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		
		material.SetTexture("blendingTexture", blendTexture);
		material.SetColor("blendColor", blendingColor);
		material.SetVector("params", new Vector4(Mathf.Clamp01(blendFactor), 0f));
		Graphics.Blit(source, destination, material);
	}
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("stillalive-studios/Effects/Camera effects/Camera Fullscreen Blender")]
public class CameraFullScreenBlender : CamEffectRegisterBase
{
	/// <summary>
	/// The maximum blending factor at the central position
	/// </summary>
	public float maxBlendFactor = 1f;
	/// <summary>
	/// The range of the blend, outside of it no camera is going to be affected.
	/// </summary>
	public float range = 25f;
	/// <summary>
	/// The color of the blend.
	/// </summary>
	public Color blendColor = Color.white;
	/// <summary>
	/// The time it takes the effect to fade in
	/// </summary>
	public float fadeInTime = 0.5f;
	/// <summary>
	/// The time it takes the effect to fade out
	/// </summary>
	public float fadeOutTime = 0.5f;
	
	protected override void registerEffect()
	{
		CameraEffects.registerFullScreenBlend(transform.position, range, maxBlendFactor, duration, fadeInTime, fadeOutTime, blendColor, null);
	}
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("stillalive-studios/Effects/Camera effects/Camera Bluring")]
public class CameraBluring : CamEffectRegisterBase
{
	/// <summary>
	/// The strength of the shaking right at the originating position (epi centre)
	/// </summary>
	public float strength = 1f;
	/// <summary>
	/// The range of the shake, outside of it no camera is going to be affected.
	/// </summary>
	public float range = 25f;

	protected override void registerEffect()
	{
		CameraEffects.registerBlur(transform.position, strength, range, duration);
	}
}

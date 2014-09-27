using UnityEngine;
using System.Collections;

[AddComponentMenu("stillalive-studios/Effects/Camera effects/Camera Shaker")]
public class CameraShaker : CamEffectRegisterBase
{
	/// <summary>
	/// The strength of the shaking right at the originating position (epi centre)
	/// </summary>
	public float strength = 0.03f;
	/// <summary>
	/// The range of the shake, outside of it no camera is going to be affected.
	/// </summary>
	public float range = 25f;
	
	protected override void registerEffect()
	{
		CameraEffects.registerShaker(transform.position, strength, range, duration);
	}
}

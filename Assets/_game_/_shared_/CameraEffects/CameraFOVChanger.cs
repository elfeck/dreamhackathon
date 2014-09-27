using UnityEngine;
using System.Collections;

[AddComponentMenu("stillalive-studios/Effects/Camera effects/Camera FOV Changer")]
public class CameraFOVChanger : CamEffectRegisterBase
{
	public bool disable = false;
	public bool relativeFOVChange = true;
	/// <summary>
	/// The new target field of view value or relative FOV change
	/// </summary>
	public float targetFieldOfView = 5f;
	/// <summary>
	/// The time it takes the effect to fade in
	/// </summary>
	public float fadeInTime = 0.5f;
	/// <summary>
	/// The time it takes the effect to fade out
	/// </summary>
	public float fadeOutTime = 0.5f;

	private Camera _onlyVisibleTo = null;

	//---------------------------------------------------------------------------//

	public void setOnlyVisibleTo(Camera cam) { _onlyVisibleTo = cam; }

	//---------------------------------------------------------------------------//

	void Awake()
	{
		_onlyVisibleTo = null;
	}

	protected override void registerEffect()
	{
		if(disable) return;
		CameraEffects.registerFOVChange(targetFieldOfView, relativeFOVChange, duration, fadeInTime, fadeOutTime, _onlyVisibleTo);
	}
}

using UnityEngine;
using System.Collections;

public abstract class CamEffectRegisterBase : SASMonoBehaviour
{
	/// <summary>
	/// If true, the effect is going to be triggered on start.
	/// </summary>
	public bool applyOnStart = true;
	/// <summary>
	/// If set the on trigger enter event invokes the effect
	/// </summary>
	public TriggerDelegate applyOnTriggerEnter = null;
	/// <summary>
	/// The delay after it has been triggered before it shows any effect.
	/// -1 keeps the effect running forever.
	/// </summary>
	public float delay = 0f;
	/// <summary>
	/// The duration of the effect (it is also wearing off in that time!)
	/// </summary>
	public float duration = 1f;

	//---------------------------------------------------------------------------//

	protected virtual void Start()
	{
		if(applyOnStart) triggerEffect();
		if(applyOnTriggerEnter != null) applyOnTriggerEnter.registerOnTriggerEnter(triggerEffect);
	}
	
	public void triggerEffect()
	{
		Invoke(registerEffect, delay);
	}
	
	protected abstract void registerEffect();

	//---------------------------------------------------------------------------//
}

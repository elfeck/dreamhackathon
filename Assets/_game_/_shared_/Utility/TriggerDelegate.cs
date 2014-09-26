using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate bool ShouldTriggerCondition(Collider coll);

[AddComponentMenu("stillalive-studios/Utility/Trigger Delegate")]
public class TriggerDelegate : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	protected ShouldTriggerCondition _conditionCallback = null;

	private System.Action<Collider, TriggerDelegate> _onTriggerEnterSender = null;
	private System.Action<Collider> _onTriggerEnterParam = null;
	private System.Action _onTriggerEnter = null;
	private System.Action<Collider, TriggerDelegate> _onTriggerExitSender = null;
	private System.Action<Collider> _onTriggerExitParam = null;
	private System.Action _onTriggerExit = null;
	
//	public System.Action<Collider> onTriggerStay; //for performance reasons this one is not supported!
	private Collider _coll;

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{
		_coll = collider;
		if(!_coll.isTrigger)
			Debug.LogWarning(name + ": TriggerDelegate only works with Triggers! Fix this!", gameObject);
	}

	//---------------------------------------------------------------------------//

	public void registerOnTriggerEnter(System.Action<Collider, TriggerDelegate> del) {_onTriggerEnterSender += del;}
	public void unregisterOnTriggerEnter(System.Action<Collider, TriggerDelegate> del) {_onTriggerEnterSender -= del;}
	public void registerOnTriggerEnter(System.Action<Collider> del) {_onTriggerEnterParam += del;}
	public void unregisterOnTriggerEnter(System.Action<Collider> del) {_onTriggerEnterParam -= del;}
	public void registerOnTriggerEnter(System.Action del) {_onTriggerEnter += del;}
	public void unregisterOnTriggerEnter(System.Action del) {_onTriggerEnter -= del;}

	public void registerOnTriggerExit(System.Action<Collider, TriggerDelegate> del) {_onTriggerExitSender += del;}
	public void unregisterOnTriggerExit(System.Action<Collider, TriggerDelegate> del) {_onTriggerExitSender -= del;}
	public void registerOnTriggerExit(System.Action<Collider> del) {_onTriggerExitParam += del;}
	public void unregisterOnTriggerExit(System.Action<Collider> del) {_onTriggerExitParam -= del;}
	public void registerOnTriggerExit(System.Action del) {_onTriggerExit += del;}
	public void unregisterOnTriggerExit(System.Action del) {_onTriggerExit -= del;}

	//---------------------------------------------------------------------------//
	
	void OnTriggerEnter(Collider coll)
	{
		if(_conditionCallback != null && !_conditionCallback(coll)) return;

		if(_onTriggerEnterSender != null) _onTriggerEnterSender(coll, this);
		if(_onTriggerEnterParam != null) _onTriggerEnterParam(coll);
		if(_onTriggerEnter != null) _onTriggerEnter();
	}
	
	void OnTriggerExit(Collider coll)
	{
		if(_conditionCallback != null && !_conditionCallback(coll)) return;

		if(_onTriggerExitSender != null) _onTriggerExitSender(coll, this);
		if(_onTriggerExitParam != null) _onTriggerExitParam(coll);
		if(_onTriggerExit != null) _onTriggerExit();
	}
	
	public void setConditionCallback(ShouldTriggerCondition c) {_conditionCallback = c;}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Returns if a certain point is inside the BOUNDS of the trigger-volume!
	/// </summary>
	public bool isInsideBounds(Vector3 point)
	{
		return _coll.bounds.Contains(point);
	}

	//---------------------------------------------------------------------------//
}

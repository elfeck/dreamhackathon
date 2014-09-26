using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Trigger Delegate (incl. stay)")]
public class TriggerDelegateExt : TriggerDelegate
{
	//------------------------- Property declaration -----------------------------//
	
	private System.Action<Collider> _onTriggerStayParam = null;
	private System.Action<Collider, TriggerDelegateExt> _onTriggerStayParam2 = null;
	private System.Action _onTriggerStay = null;

	//-------------------------- Method declarations -----------------------------//
	
	public void registerOnTriggerStay(System.Action<Collider> del) {_onTriggerStayParam += del;}
	public void unregisterOnTriggerStay(System.Action<Collider> del) {_onTriggerStayParam -= del;}
	public void registerOnTriggerStay(System.Action del) {_onTriggerStay += del;}
	public void unregisterOnTriggerStay(System.Action del) {_onTriggerStay -= del;}
	public void registerOnTriggerStay(System.Action<Collider, TriggerDelegateExt> del) {_onTriggerStayParam2 += del;}
	public void unregisterOnTriggerStay(System.Action<Collider, TriggerDelegateExt> del) {_onTriggerStayParam2 -= del;}
	
	void OnTriggerStay(Collider coll)
	{
		if(_conditionCallback != null && !_conditionCallback(coll)) return;
		
		if(_onTriggerStayParam != null) _onTriggerStayParam(coll);
		if(_onTriggerStay != null) _onTriggerStay();
		if(_onTriggerStayParam2 != null) _onTriggerStayParam2(coll, this);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Collision Delegate")]
public class CollisionDelegate : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	private System.Action<Collision> _onCollisionEnterParam = null;
	private System.Action _onCollisionEnter = null;
	private System.Action<Collision> _onCollisionExitParam = null;
	private System.Action _onCollisionExit = null;

	//-------------------------- Method declarations -----------------------------//
	
	public void registerOnCollisionEnter(System.Action<Collision> del) {_onCollisionEnterParam += del;}
	public void unregisterOnCollisionEnter(System.Action<Collision> del) {_onCollisionEnterParam -= del;}
	public void registerOnCollisionEnter(System.Action del) {_onCollisionEnter += del;}
	public void unregisterOnCollisionEnter(System.Action del) {_onCollisionEnter -= del;}
	
	public void registerOnCollisionExit(System.Action<Collision> del) {_onCollisionExitParam += del;}
	public void unregisterOnCollisionExit(System.Action<Collision> del) {_onCollisionExitParam -= del;}
	public void registerOnCollisionExit(System.Action del) {_onCollisionExit += del;}
	public void unregisterOnCollisionExit(System.Action del) {_onCollisionExit -= del;}

	void Awake()
	{
		if(collider == null && rigidbody == null)
			Debug.LogWarning(name + ": CollisionDelegate needs a Collider or a Rigidbody!");
		else if(collider && collider.isTrigger)
			Debug.LogWarning(name + ": CollisionDelegate does not work with Triggers! Fix this!");
	}
	
	void OnCollisionEnter(Collision coll)
	{
		if(_onCollisionEnterParam != null) _onCollisionEnterParam(coll);
		if(_onCollisionEnter != null) _onCollisionEnter();
	}
	
	void OnCollisionExit(Collision coll)
	{
		if(_onCollisionExitParam != null) _onCollisionExitParam(coll);
		if(_onCollisionExit != null) _onCollisionExit();
	}
}

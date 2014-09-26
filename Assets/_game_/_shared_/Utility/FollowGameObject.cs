using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Follow Another GameObject (Attach to)")]
public class FollowGameObject : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	/// <summary>
	/// This script makes a certain it's gameobject follow the subject
	/// </summary>
	public Transform subject = null;
	/// <summary>
	/// Tells if also the rotation should be matched
	/// </summary>
	public bool followRotation = true;
	
	protected Transform _trans;
	
	//-------------------------- Method declarations -----------------------------//

	protected virtual void Awake()
	{
		_trans = transform;
	}

	protected virtual void Update()
	{
		updateSubject();
	}

	protected virtual void updateSubject()
	{
		if(subject == null) return;

		var rb = subject.rigidbody;
		_trans.position = rb == null ? subject.position : rb.worldCenterOfMass;
		if(followRotation) _trans.forward = subject.forward;
	}
}

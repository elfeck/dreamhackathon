using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("stillalive-studios/Utility/Rigidbody Fixator")]
public class RigidbodyFixator : SASMonoBehaviour
{
	public bool freezePositionX = false;
	public bool freezePositionY = false;
	public bool freezePositionZ = false;
	public bool freezeRotationX = false;
	public bool freezeRotationY = false;
	public bool freezeRotationZ = false;
	
	private RigidbodyConstraints _initConstraint;
	
	public RigidbodyConstraints getInitialConstraint() {return _initConstraint;}
	
	void Awake()
	{
		_initConstraint = RigidbodyConstraints.None;
		if(freezePositionX) _initConstraint |= RigidbodyConstraints.FreezePositionX;
		if(freezePositionY) _initConstraint |= RigidbodyConstraints.FreezePositionY;
		if(freezePositionZ) _initConstraint |= RigidbodyConstraints.FreezePositionZ;
		if(freezeRotationX) _initConstraint |= RigidbodyConstraints.FreezeRotationX;
		if(freezeRotationY) _initConstraint |= RigidbodyConstraints.FreezeRotationY;
		if(freezeRotationZ) _initConstraint |= RigidbodyConstraints.FreezeRotationZ;
		rigidbody.constraints = _initConstraint;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Mirror movements")]
public class MirrorMovement : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	public Transform observed;
	public bool local = true;
	public bool mirrorDeltaPosX = true;
	public bool mirrorDeltaPosY = true;
	public bool mirrorDeltaPosZ = true;
	public float movementScaleFactor = 1f;
	
	private Vector3 _lastPos;
	private Transform _trans;
	
	//-------------------------- Method declarations -----------------------------//
	
	private Vector3 getObjPosition() {return local ? observed.localPosition : observed.position;}
	
	void applyDelta(Vector3 delta)
	{
		if(local) _trans.localPosition += delta;
		else _trans.position += delta;
	}
	
	void Start()
	{
		_trans = transform;
		if(observed == null) Debug.LogWarning("The mirror movement script is not observing any object!", gameObject);
		else _lastPos = getObjPosition();
	}
	
	void FixedUpdate()
	{
		if(null == observed) return;
		
		Vector3 pos = getObjPosition();
		Vector3 delta = pos - _lastPos;
		delta = -delta * movementScaleFactor;
		if(!mirrorDeltaPosX) delta.x = 0f;
		if(!mirrorDeltaPosY) delta.y = 0f;
		if(!mirrorDeltaPosZ) delta.z = 0f;
		applyDelta(delta);
		_lastPos = pos;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Reset Transform On Event (position, rotation)")]
public class ResetTransformOnEvent : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	/// <summary>
	/// True if the position should be reset
	/// </summary>
	public bool resetPosition = true;
	/// <summary>
	/// True if the rotation should be reset
	/// </summary>
	public bool resetRotation = true;
	
	public enum ResetEvent {None, OnEnable, OnDisable};
	/// <summary>
	/// Select the event that triggers the reset. None if you want to do it manually only.
	/// </summary>
	public ResetEvent resetEvent = ResetEvent.None;
	
	private Vector3 _pos;
	private Quaternion _rot;
	private Transform _trans;

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{
		_trans = transform;
		_pos = _trans.position;
		_rot = _trans.rotation;
	}
	
	public void reset()
	{
		if(resetPosition) _trans.position = _pos;
		if(resetRotation) _trans.rotation = _rot;
	}
	
	void OnEnable()
	{
		if(resetEvent == ResetEvent.OnEnable) reset();
	}
	
	protected override void OnDisable()
	{
		base.OnDisable();
		if(resetEvent == ResetEvent.OnDisable) reset();
	}
}

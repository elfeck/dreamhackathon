using UnityEngine;
using System.Collections;

/// <summary>
/// Virtual transform child is used to increase performance. It will take a child of a deep hierarchy (the object
/// this is attached to) and move the child up in the hierarchy (e.g. to the root), but still moving as if it was
/// attached to the place it came from.
/// </summary>
[AddComponentMenu("stillalive-studios/Utility/Virtual Transform Child (virtual parenting)")]
public class VirtualTransformChild : SASMonoBehaviour
{
	/// <summary>
	/// If this is null, the object will be moved up to the root while still following the movement of the object
	/// it was attached to. But if this is != null, it will be parented to this object instead of the uppermost root.
	/// NOTE: This only works if moveToParent is (some) parent of the object itself
	/// </summary>
	public Transform moveToParent = null;
	
	private Transform _attachTo;
	private Transform _trans;
	private Transform _root;
	private Vector3 _localPos;
	private Vector3 _globalScale;
	private Quaternion _localRot;
	
	void Awake()
	{
		if(_attachTo == null)
		{
			_trans = transform;
			_attachTo = _trans.parent;
			_localPos = _trans.localPosition;
			_localRot = _trans.localRotation;
			
			//find root and move object up to the root
			_root = _trans;
			while(_root.parent != null && _root != moveToParent) _root = _root.parent;
			_trans.parent = _root;
		}
	}
	
	void LateUpdate()
	{
		if(_attachTo == null)
		{
			Destroy(this);
			return;
		}
		
		Quaternion rot = _attachTo.rotation;
//		_trans.localPosition = (_attachTo.position-_root.position) + rot * HelperFunctions.vector3Product(_localPos, _globalScale);
		_trans.localRotation = HelperFunctions.conjugateQuaternion(_root.rotation) * rot * _localRot;
		
		_trans.localPosition = _root.InverseTransformPoint(_attachTo.TransformPoint(_localPos));
	}
}

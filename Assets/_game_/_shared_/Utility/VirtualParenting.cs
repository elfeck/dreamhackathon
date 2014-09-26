using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VirtualParenting : SASMonoBehaviour
{
	/// <summary>
	/// The transform this object should be virtually attached to (parenting).
	/// </summary>
	public Transform attachTo;
	/// <summary>
	/// If VirtualPartenting was deactivated and gets activated, the position/rotation
	/// is updated with a maximum velocity, until the target position/rotation is reached.
	/// </summary>
	public bool smoothAfterActivation = false;
	/// <summary>
	/// The max relative speed in smooth mode.
	/// </summary>
	public float maxSmoothSpeed = 10f;
	/// <summary>
	/// The max relative rotation angle in smooth mode.
	/// </summary>
	public float maxSmoothRotationSpeed = 180f;
	
	private Transform _trans;
	private Transform _root;
	private Vector3 _localPos;
	private Quaternion _localRot;
	private bool _active = true;
	private bool _inSmoothMode = false;

	public void setActive(bool active)
	{
		_active = active;
		if(_active)
		{
			_inSmoothMode = true;
		}
	}

	void Awake()
	{
		_active = true;

		if(attachTo == null)
		{
			Debug.LogError("VirtualParenting: There was no Transform specified to attach to!", gameObject);
			Destroy(this);
			return;
		}
		
		_trans = transform;
		_root = _trans.parent;
		
		_localPos = attachTo.InverseTransformPoint(_trans.position);
		_localRot = HelperFunctions.conjugateQuaternion(attachTo.rotation) * _trans.rotation;
	}
	
	void LateUpdate()
	{
		if(attachTo == null)
		{
			Destroy(this);
			return;
		}
		if(!_active) return;

		Quaternion rot = attachTo.rotation;
		Vector3 tempLocalPosition;
		Quaternion tempLocalRotation;
		if(_root != null)
		{
			tempLocalRotation = HelperFunctions.conjugateQuaternion(_root.rotation) * rot * _localRot;
			tempLocalPosition = _root.InverseTransformPoint(attachTo.TransformPoint(_localPos));
		}
		else
		{
			tempLocalRotation = rot * _localRot;
			tempLocalPosition = attachTo.TransformPoint(_localPos);
		}

		if(_inSmoothMode)
		{
			float deltaAngle = maxSmoothRotationSpeed * Time.deltaTime;
			float deltaPos = maxSmoothSpeed * Time.deltaTime;

			bool rotationOff = Quaternion.Angle(tempLocalRotation, _trans.localRotation) > deltaAngle;
			bool positionOff = (tempLocalPosition - _trans.localPosition).sqrMagnitude > deltaPos*deltaPos;

			if(!positionOff && !rotationOff)
			{
				_inSmoothMode = false;
			}
			else
			{
				if(rotationOff)
					tempLocalRotation = Quaternion.RotateTowards(_trans.localRotation, tempLocalRotation, deltaAngle);

				if(positionOff)
					tempLocalPosition = Vector3.MoveTowards(_trans.localPosition, tempLocalPosition, deltaPos);
			}
		}

		_trans.localRotation = tempLocalRotation;
		_trans.localPosition = tempLocalPosition;
	}
}

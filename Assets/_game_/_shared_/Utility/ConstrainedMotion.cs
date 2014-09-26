using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConstrainedMotion : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//

	public enum ConstraintType {Pendulum}
	public ConstraintType type = ConstraintType.Pendulum;
	public Transform anchor;

	private Rigidbody _rb;
	private Transform _trans;
	private float _initDistance;

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{
		_rb = rigidbody;
		_trans = transform;

		_initDistance = Vector3.Distance(_rb.position, anchor.position);
	}

	void FixedUpdate()
	{
		if(_rb == null || anchor == null) return;

		switch(type)
		{
		case ConstraintType.Pendulum: updatePendulum(Time.fixedDeltaTime); break;
		}
	}

	//---------------------------------------------------------------------------//

	void updatePendulum(float dt)
	{
		//simulate gravitation
		_rb.useGravity = false;

		//snap to sphere surface
		Vector3 dir = Vector3.Normalize(_rb.position - anchor.position);
		_rb.MovePosition(anchor.position + dir.normalized * _initDistance);

		//apply simulated gravitation force
		Vector3 effGravity = Physics.gravity - Vector3.Dot(Physics.gravity, dir) * dir;
		_rb.AddForce(effGravity, ForceMode.Acceleration);

		//project current velocity onto tangent of sphere
		var tangent = Vector3.Cross(Vector3.Cross(dir, _rb.velocity), dir).normalized;
		if(tangent != Vector3.zero) _rb.velocity = tangent * _rb.velocity.magnitude;

		//update rotation
		_trans.up = -dir;
	}

	//---------------------------------------------------------------------------//
}

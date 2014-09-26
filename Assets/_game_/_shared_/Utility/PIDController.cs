using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is the base class for all PID Controllers.
/// NOTE: Make sure the controller is only called during FixedUpdate()
/// -> this will make it more stable and one can use fixedDeltaTime as time-step [internally]
/// </summary>
[System.Serializable]
public class BasePIDController
{
	public float p;
	public float i;
	public float d;
	
	public BasePIDController(float pp, float ii, float dd) {p = pp; i = ii; d = dd;}

	public void setPIDFactors(float pp, float ii, float dd) {p = pp; i = ii; d = dd;}
	public void setPIDFactors(BasePIDController other) {setPIDFactors(other.p, other.i, other.d);}
}

//---------------------------------------------------------------//

[System.Serializable]
public class PIDController : BasePIDController
{
	private float _lastError = 0f;
	private float _integral = 0f;
	private float _maxIntegralLength = 5f;
	
	public PIDController(PIDController copy) : base(copy.p, copy.i, copy.d)
	{
		_lastError = copy._lastError;
		_integral = copy._integral;
	}
	public PIDController(float pp, float ii, float dd) : base(pp, ii, dd) {}
	public void reset() {_lastError = 0f; _integral = 0f;}
	
	public float apply(float curr, float should, float dt)
	{
		float error = should - curr;
		_integral += dt * error;
		_integral = Mathf.Min(_integral, _maxIntegralLength);
		float diff = (error - _lastError) / dt;
		_lastError = error;
		return p * error + _integral * i + diff * d;
	}
}

//---------------------------------------------------------------//

[System.Serializable]
public class PositionController : BasePIDController
{
	private Vector3 _lastError = new Vector3(0f, 0f, 0f);
	private Vector3 _integral = new Vector3(0f, 0f, 0f);
	private Vector3 _diff = new Vector3(0f, 0f, 0f);
	private float _maxIntegralLength = 5f;
	
	public PositionController(PositionController copy) : base(copy.p, copy.i, copy.d)
	{
		_lastError = copy._lastError;
		_integral = copy._integral;
	}
	public PositionController(float pp, float ii, float dd) : base(pp, ii, dd) {}
	public void reset() {_lastError = new Vector3(0f, 0f, 0f); _integral = new Vector3(0f, 0f, 0f);}
	
	public Vector3 apply(Vector3 currPos, Vector3 shouldPos, float dt)
	{
		Vector3 error = shouldPos - currPos;
		_integral += dt * error;
		if(_integral.sqrMagnitude > _maxIntegralLength*_maxIntegralLength) _integral *= _maxIntegralLength / _integral.magnitude;
		_diff = (error - _lastError) / dt;
		_lastError = error;
		return p * error + _integral * i + _diff * d;
	}

	public Vector3 getCorrectionP() {return _lastError * p;}
	public Vector3 getCorrectionI() {return _integral * i;}
	public Vector3 getCorrectionD() {return _diff * d;}
}

//---------------------------------------------------------------//

[System.Serializable]
public class OrientationController : BasePIDController
{
	private Vector3 _lastError = new Vector3(0f, 0f, 0f);
	private Vector3 _integral = new Vector3(0f, 0f, 0f);
	
	public OrientationController(float pp, float ii, float dd) : base(pp, ii, dd) {}
	public void reset() {_lastError = new Vector3(0f, 0f, 0f); _integral = new Vector3(0f, 0f, 0f);}
	
	public Vector3 apply(Quaternion currRot, Quaternion shouldRot, float dt)
	{		
		//first compute the rotational difference between the two orientations
		Quaternion dq = shouldRot * HelperFunctions.conjugateQuaternion(currRot);
		
		//convert the difference rotation to an axis and an angle and transform this
		//afterwards to a angular-velocity (angle/time) as input-error to the controller.
		float angle = 0f;
		Vector3 axis = Vector3.zero;
		dq.ToAngleAxis(out angle, out axis);
		angle = Mathf.Deg2Rad * angle;
		angle = Mathf.Repeat(angle, Mathf.PI * 2);
		if(angle > Mathf.PI) angle = angle - Mathf.PI * 2;
		else if(angle < -Mathf.PI) angle = angle + Mathf.PI * 2;
		Vector3 w = axis * angle / dt;

		if(angle == 0f) w = Vector3.zero;
		
		//use omega (=w) as error
		_integral += dt * w;
		Vector3 diff = (w - _lastError) / dt;
		_lastError = w;
		return p * w + _integral * i + diff * d;
	}
}

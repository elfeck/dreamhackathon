using UnityEngine;
using System.Collections;

public enum BillboardFacingForwardAxis
{
	XAxis, YAxis, ZAxis
}

[AddComponentMenu("stillalive-studios/Other/Billboard facing behavior")]
public class BillboardFacing : SASMonoBehaviour
{
	public float positionOffset = 0f;
	public bool laserBillboard = false;
	public Vector3 fixedAxis = Vector3.forward;
	public BillboardFacingForwardAxis localAxisToAlignWithFixedAxis = BillboardFacingForwardAxis.ZAxis;
	private Transform _trans;
	
	void Awake()
	{
		_trans = transform;
	}
	
	void OnWillRenderObject()
	{
		Vector3 camDir = Camera.current.transform.forward;
		_trans.position += positionOffset * camDir;
		if(!laserBillboard)
			_trans.forward = Vector3.Normalize(Camera.current.transform.position - _trans.position);
		else
		{
			Vector3 axisZ = _trans.TransformDirection(fixedAxis);
			axisZ.Normalize();
			Vector3 axisY = -Vector3.Normalize(camDir - Vector3.Dot(axisZ, camDir) * axisZ);
			Vector3 axisX = Vector3.Normalize(Vector3.Cross(axisY, axisZ));
			
			if(localAxisToAlignWithFixedAxis == BillboardFacingForwardAxis.XAxis) _trans.LookAt(_trans.position + axisY, axisX);
			else if(localAxisToAlignWithFixedAxis == BillboardFacingForwardAxis.ZAxis) _trans.LookAt(_trans.position + axisZ, axisY);
			else if(localAxisToAlignWithFixedAxis == BillboardFacingForwardAxis.YAxis) _trans.LookAt(_trans.position + axisX, axisZ);
		}
	}
}

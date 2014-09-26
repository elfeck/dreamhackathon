using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FixInZPlane : SASMonoBehaviour
{
	void Awake()
	{
		rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
	}
}

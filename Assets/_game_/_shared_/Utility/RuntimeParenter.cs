using UnityEngine;
using System.Collections;

[AddComponentMenu("stillalive-studios/Other/Runtime parenter")]
public class RuntimeParenter : SASMonoBehaviour
{
	public Transform parent;
	public bool resetPosition = true;
	public bool resetRotation = false;
	
	void Awake()
	{
		if(parent)
		{
			transform.parent = parent;
			if(resetPosition) transform.localPosition = Vector3.zero;
			if(resetRotation) transform.localRotation = Quaternion.identity;
		}
	}
}

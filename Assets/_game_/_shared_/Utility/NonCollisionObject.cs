using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NonCollisionObject : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//

	/// <summary>
	/// The collider will be registered to this non-collision group.
	/// </summary>
	public string nonCollisionGroupName;
	/// <summary>
	/// If this is true, all colliders in childs will be added to the group too.
	/// </summary>
	public bool ignoreCollisionRecursively = false;

	private Collider _collider;

	//-------------------------- Method declarations -----------------------------//

	protected virtual void Awake()
	{
		_collider = collider;

		if(nonCollisionGroupName == null || nonCollisionGroupName == "")
		{
			Debug.LogError("goupName of NonCollisionObject must be assigned!");
		}
		else
		{
			if(_collider)
				NonCollisionGroups.registerObject(_collider, nonCollisionGroupName);
	
			if(ignoreCollisionRecursively)
			{
				foreach(var coll in GetComponentsInChildren<Collider>())
				{
					NonCollisionGroups.registerObject(coll, nonCollisionGroupName);
				}
			}
		}
	}

	protected virtual void OnDestroy()
	{
		if(nonCollisionGroupName == null || nonCollisionGroupName == "")
			return;

		if(_collider)
			NonCollisionGroups.unRegisterObject(_collider, nonCollisionGroupName);

		if(ignoreCollisionRecursively)
		{
			foreach(var coll in GetComponentsInChildren<Collider>())
			{
				NonCollisionGroups.unRegisterObject(coll, nonCollisionGroupName);
			}
		}
	}
}

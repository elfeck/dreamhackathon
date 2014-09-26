using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class NonCollisionGroups
{
	//------------------------- Property declaration -----------------------------//
	private static Dictionary<string,List<Collider>> _groups = new Dictionary<string, List<Collider>>();

	//-------------------------- Method declarations -----------------------------//

	public static void registerObject(Collider collider, string groupName)
	{
		if(!collider.gameObject.activeInHierarchy)
		{
			Debug.LogError("IgnoreCollision cannot be called if collider is disabled!");
			return;
		}

		if(_groups.ContainsKey(groupName) && _groups[groupName] != null)
		{
			if(_groups[groupName].Contains(collider))
			{
				Debug.LogWarning("Collider " + collider + " already registered in group <" + groupName + ">!");
				return;
			}

			foreach(var coll in _groups[groupName])
			{
				if(coll == null) continue;
				if(coll.gameObject.activeInHierarchy)
					Physics.IgnoreCollision(coll, collider);
				else
					Debug.LogError("IgnoreCollision cannot be called if collider is disabled!" + coll);
			}
		}
		else
			_groups[groupName] = new List<Collider>();

		//add collider after the IgnoreCollision calls
		_groups[groupName].Add(collider);
	}

	public static void unRegisterObject(Collider collider, string groupName)
	{
		if(!collider.gameObject.activeInHierarchy)
		{
			Debug.LogError("IgnoreCollision cannot be called if collider is disabled!");
			return;
		}

		if(_groups.ContainsKey(groupName) && _groups[groupName] != null)
		{
			if(!_groups[groupName].Contains(collider))
			{
				Debug.LogWarning("Collider " + collider + " not found in group <" + groupName + ">!");
				return;
			}

			//remove collider first
			_groups[groupName].Remove(collider);

			foreach(var coll in _groups[groupName])
			{
				if(coll.gameObject.activeInHierarchy)
					Physics.IgnoreCollision(coll, collider, false);
				else
					Debug.LogError("IgnoreCollision cannot be called if collider is disabled!" + coll);
			}
		}
		else
			Debug.LogError("Group <" + groupName + "> not found!");
	}
}

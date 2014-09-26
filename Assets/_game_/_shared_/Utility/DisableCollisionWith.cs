using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Disable Collider-Collision With")]
public class DisableCollisionWith : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	public List<Collider> otherColliders = new List<Collider>(1);
	public bool disableOnStart = true;
	public bool disableForChildColliderToo = false;

	//-------------------------- Method declarations -----------------------------//

	void Start()
	{
		changeCollisionIgnoreState(disableOnStart);
	}
	
	/// <summary>
	/// Enables/Disables the collisions between all colliders in "otherColliders" and the one of this gameobject
	/// </summary>
	public void changeCollisionIgnoreState(bool ignore)
	{
		Collider[] colliders = {collider};
		if(disableForChildColliderToo) colliders = GetComponentsInChildren<Collider>();

		foreach(Collider coll in colliders)
		{
			if(coll == null) continue;
			if(!coll.enabled || !coll.gameObject.activeInHierarchy) continue;
			
			foreach(var c in otherColliders)
			{
				if(c != null && c.enabled && c.gameObject.activeInHierarchy)
					Physics.IgnoreCollision(coll, c, ignore);
			}
		}
	}
}

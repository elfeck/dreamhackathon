using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Unparents a gameobject
/// \authors Julian Mautner
/// </summary>
public class Unparenter : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//

	public bool unparentOnStart = true;

	//-------------------------- Method declarations -----------------------------//

	void Start()
	{
		if(unparentOnStart) unparent();
	}

	public void unparent()
	{
		transform.parent = null;
		this.enabled = false;
	}
}

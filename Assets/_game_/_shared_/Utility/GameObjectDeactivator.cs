using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Activates and deactivates object on start
/// \authors Julian Mautner
/// </summary>
public class GameObjectDeactivator : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//

    /// <summary>
    /// The time delay after Start() before the object(s) are deactivated.
    /// </summary>
	public float delay = 0f;

	//-------------------------- Method declarations -----------------------------//

	void Start()
	{
		if(delay > 0f) Invoke(action, delay);
		else action();
	}

	protected virtual void action()
	{
        gameObject.SetActive(false);
	}
}

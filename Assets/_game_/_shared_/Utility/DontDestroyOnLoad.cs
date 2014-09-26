using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Don't Destroy On Load")]
public class DontDestroyOnLoad : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}

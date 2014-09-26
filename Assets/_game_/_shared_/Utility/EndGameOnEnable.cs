using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndGameOnEnable : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	public float delay = 2f;

	//-------------------------- Method declarations -----------------------------//

	void OnEnable()
	{
		Invoke(quit,delay);	
	}
	
	void quit()
	{
		Application.Quit();		
	}
}

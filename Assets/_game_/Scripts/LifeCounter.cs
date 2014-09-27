using UnityEngine;
using System.Collections;

public class LifeCounter : MonoBehaviour
{

	void Update ()
	{
		if(GameSession.inst.lifes > 0)
		{
			guiText.text = "Lifes: " + GameSession.inst.lifes;
			guiText.enabled = true;
		}
		else
			guiText.enabled = false;
	}
}

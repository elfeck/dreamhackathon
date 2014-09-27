using UnityEngine;
using System.Collections;

public class LifeCounter : MonoBehaviour
{
	public bool blue = true;

	void Update ()
	{
		var lifes = blue ? GameSession.inst.blueLifes : GameSession.inst.redLifes;

		if(lifes > 0 && GameSession.inst.allowGameOver)
		{
			guiText.text = lifes.ToString();
			guiText.enabled = true;
		}
		else
			guiText.enabled = false;
	}
}

using UnityEngine;
using System.Collections;

public class DeathCounter : MonoBehaviour
{

	void Update ()
	{
		if(!GameSession.inst.gameOver())
			guiText.text = "Death Count\n" + Entity.deathCount.ToString();
	}
}

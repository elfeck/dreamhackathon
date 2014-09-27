using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour
{

	void Update()
	{
		guiText.enabled = GameSession.inst.lifes <= 0;
	}
}

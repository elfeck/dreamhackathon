using UnityEngine;
using System.Collections;

public class GameOverScreen : MonoBehaviour
{

	void Update()
	{
		guiText.enabled = GameSession.inst.allowGameOver && (GameSession.inst.redLifes <= 0 || GameSession.inst.blueLifes <= 0);
	}
}

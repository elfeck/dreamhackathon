using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour
{
	IEnumerator Start ()
	{
		PlayerController.inst.cursors = 0;
		GameSession.inst.enableSpawnBalanceCurve = false;
		GameSession.inst.spawnRate = 0f;

		yield return StartCoroutine(showTextCo("TUTORIAL", 1.5f));
		yield return new WaitForSeconds(0.5f);

		GameSession.inst.setSpawnBalance(1f);
		GameSession.inst.spawnRate = 20f;

		yield return StartCoroutine(showTextCo("This is the RED army... happy!", 3f));

		yield return new WaitForSeconds(1f);

		PlayerController.inst.cursors = 1;
		yield return StartCoroutine(showTextCo("But then you show up...", 1.5f, true));

		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCo("... and change their loyalty! (MOVE the CURSOR over them)", 5f, true));

		//========================================//

		yield return new WaitForSeconds(6f);
		PlayerController.inst.cursors = 0;
		yield return StartCoroutine(showTextCo("That's enough chaos for now!", 2f));
		GameSession.inst.spawnRate = 0f;

		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCo("I know you like seeing things die...", 2.5f));

		yield return new WaitForSeconds(1.5f);
		GameSession.inst.setSpawnBalance(0.3f);
		GameSession.inst.spawnRate = 60f;
		yield return StartCoroutine(showTextCo("The BLUE army is coming! WATCH!", 2f));

		Time.timeScale = 3f;
		yield return new WaitForSeconds(3f * Time.timeScale);
		Time.timeScale = 0.25f;
		yield return StartCoroutine(showTextCo("Ohhh beautiful! All that slaughtering!", 3f * Time.timeScale));
		Time.timeScale = 2.5f;

		yield return new WaitForSeconds(4f * Time.timeScale);

		Time.timeScale = 0.25f;
		yield return StartCoroutine(showTextCo("No!! The blue army is too strong! The war will be over soon!", 3f * Time.timeScale));

		PlayerController.inst.cursors = 2;
		GameSession.inst.setSpawnBalance(0.5f);
		GameSession.inst.spawnRate = 50f;
		yield return StartCoroutine(showTextCo("You must take action now! The war needs to last forever!", 3f * Time.timeScale));
		Time.timeScale = 1f;

		//========================================//

		yield return new WaitForSeconds(9f);
		

		yield return StartCoroutine(showTextCo("Make sure neither army reaches the other side by converting them!", 4f, true));
		GameSession.inst.allowGameOver = true;
		yield return StartCoroutine(showTextCo("If one empire falls, the war is over...", 4f * Time.timeScale, true));
		Time.timeScale = 1f;
		yield return new WaitForSeconds(6f * Time.timeScale);


		GameSession.inst.spawnRate = 40f;
		Time.timeScale = 0.25f;
		yield return StartCoroutine(showTextCo("Well done! The war goes on!", 3f * Time.timeScale));

		yield return new WaitForSeconds(0.5f * Time.timeScale);
		yield return StartCoroutine(showTextCo("Now you're ready! May the pile of corpses grow!", 3f * Time.timeScale));

		Time.timeScale = 1f;
		Application.LoadLevel("Level01");
	}

	IEnumerator showTextCo(string text, float duration, bool pause = false)
	{
		float scale = Time.timeScale;
		if(pause)
		{
			Time.timeScale = 0.15f;
			duration *= Time.timeScale;
		}

		showText(text);
		yield return new WaitForSeconds(duration);
		showText("");

		if(pause) Time.timeScale = scale;
	}


	void showText(string txt)
	{
		guiText.enabled = txt != "";
		guiText.text = txt;
	}
}

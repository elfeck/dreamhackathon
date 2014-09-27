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
		yield return new WaitForSeconds(1f);

		GameSession.inst.setSpawnBalance(1f);
		GameSession.inst.spawnRate = 20f;

		yield return StartCoroutine(showTextCo("This is a rampaging army...", 3f));
		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCo("Sure what they fight for...", 2f));

		yield return new WaitForSeconds(1f);

		yield return StartCoroutine(showTextCo("But then you show up...", 1.5f));
		PlayerController.inst.cursors = 1;

		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCo("... and change their minds...", 3f));

		//========================================//

		yield return new WaitForSeconds(6f);
		PlayerController.inst.cursors = 0;
		yield return StartCoroutine(showTextCo("That's enough deads for now!", 3f));
		GameSession.inst.spawnRate = 0f;

		yield return new WaitForSeconds(0.5f);
		yield return StartCoroutine(showTextCo("I know you like seeing things die...", 3f));

		yield return new WaitForSeconds(2f);
		GameSession.inst.setSpawnBalance(0.3f);
		GameSession.inst.spawnRate = 60f;
		yield return StartCoroutine(showTextCo("Yeah! A 2nd army! WAR!", 3f));

		Time.timeScale = 2f;
		yield return new WaitForSeconds(3f * Time.timeScale);
		Time.timeScale = 0.25f;
		yield return StartCoroutine(showTextCo("Ahhh beautiful! All the slaughtering!", 3f * Time.timeScale));
		Time.timeScale = 2f;

		yield return new WaitForSeconds(8f * Time.timeScale);

		Time.timeScale = 0.25f;
		yield return StartCoroutine(showTextCo("No!! The blue army is stronger! The war will be over soon!", 3f * Time.timeScale));

		PlayerController.inst.cursors = 2;
		GameSession.inst.setSpawnBalance(0.5f);
		GameSession.inst.spawnRate = 50f;
		yield return StartCoroutine(showTextCo("You must take action now! The war needs to last forever!", 3f * Time.timeScale));
		Time.timeScale = 1f;

		//========================================//

		yield return new WaitForSeconds(12f);

		PlayerController.inst.cursors = 0;
		GameSession.inst.spawnRate = 40f;

		Time.timeScale = 0.25f;
		yield return StartCoroutine(showTextCo("Well done! The war goes on!", 3f * Time.timeScale));

		yield return new WaitForSeconds(0.5f * Time.timeScale);
		yield return StartCoroutine(showTextCo("Now you're ready! May the pile of corpses grow!", 3f * Time.timeScale));

		Time.timeScale = 1f;
		Application.LoadLevel("Level01");
	}

	IEnumerator showTextCo(string text, float duration)
	{
		showText(text);
		yield return new WaitForSeconds(duration);
		showText("");
	}


	void showText(string txt)
	{
		guiText.enabled = txt != "";
		guiText.text = txt;
	}
}

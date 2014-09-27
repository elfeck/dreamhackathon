using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{
	public Rect window = new Rect(0f, 0f, 200f, 200f);
    public Texture2D background;

	void OnGUI()
	{
		Screen.showCursor = true;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.ScaleAndCrop);

		window.x = (Screen.width - window.width) * 0.5f;
		window.y = (Screen.height - window.height) * 0.73f;

		GUI.Window(0, window, windowFunc, "");
	}

	void windowFunc(int id)
	{
		if(GUILayout.Button("Tutorial"))
			Application.LoadLevel("Tutorial");

		GUILayout.Space(10f);

		if(GUILayout.Button("Easy"))
			Application.LoadLevel("Level01");
		if(GUILayout.Button("Normal"))
			Application.LoadLevel("Level02");
		if(GUILayout.Button("Hard"))
			Application.LoadLevel("Level03");

		GUILayout.Space(15f);

		if(GUILayout.Button("Exit"))
			Application.Quit();
	}
}

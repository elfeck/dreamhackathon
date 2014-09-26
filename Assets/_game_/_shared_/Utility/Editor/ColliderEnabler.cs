using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class ColliderEnabler : EditorWindow
{
	
	[MenuItem("stillalive-studios/Tools/Collider Enabler-Disabler")]
	public static void Init()
	{
		ColliderEnabler window = EditorWindow.GetWindow<ColliderEnabler>("Collider Enabler");
		window.Show();
		window.Focus();
	}
	
	void OnGUI()
	{
		GUILayout.Label("Enables or Disables all children of the selected\nobjects with the following names:");
		GUILayout.Label("  - Collider_Cube");
		GUILayout.Label("  - Collider_Sphere");
		GUILayout.Label("  - Collider_Capsule");
		GUILayout.Label("  - Collider_Mesh");
		GUILayout.Space(10.0f);
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Enable")) setColliderActive(true);
		if(GUILayout.Button("Disable")) setColliderActive(false);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(10.0f);
		if(_objCounter > 0)
			GUILayout.Label(" --> " + _objCounter + " objects have been updated!");
	}
	
	private bool _tmpActive;
	private int _objCounter = 0;
	private void setColliderActive(bool active)
	{
		_objCounter = 0;
		_tmpActive = active;
		foreach(Transform t in Selection.transforms)
		{
			setColliderActiveRec(t);
		}
		
	}
	
	private void setColliderActiveRec(Transform t)
	{
		if(t.name == "Collider_Cube" || t.name == "Collider_Sphere" || t.name == "Collider_Capsule" || t.name == "Collider_Mesh")
		{
			t.gameObject.SetActive(_tmpActive);
			_objCounter++;
		}
		else
		{
			foreach(Transform child in t)
				setColliderActiveRec(child);
		}
	}
}

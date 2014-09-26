using UnityEngine;
using UnityEditor;
using System;

public class CreateAssetFromScript : Editor
{

	[MenuItem("Assets/Create Asset From Script")]
	public static void CreateManagerValidate()
	{
		if(Selection.activeObject.GetType() != typeof(MonoScript))
			return;
		MonoScript script = (MonoScript)Selection.activeObject;
		var scriptClass = script.GetClass();
		if(scriptClass == null)
			return;

		ScriptableObject asset = ScriptableObject.CreateInstance(Selection.activeObject.name);
		AssetDatabase.CreateAsset(asset, String.Format("Assets/{0}.asset", Selection.activeObject.name));
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}
}

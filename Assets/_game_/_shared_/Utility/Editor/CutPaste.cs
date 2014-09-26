using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CutPaste : EditorWindow
{
	private List<GameObject> _selectionList = new List<GameObject>();
	private GameObject _pasteInto = null;
	private Vector2 _scrollPos = Vector2.zero;
	
	[MenuItem("stillalive-studios/Tools/Cut'n'Paste GameObjects")]
	public static void Init()
	{
		var window = EditorWindow.GetWindow<CutPaste>("Cut & Paste GameObjects");
		window.Show();
		window.Focus();
	}
	
	void OnGUI()
	{
		
		EditorGUILayout.LabelField("1) Select GameObjects to cut out:");
		_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
		for(int i = 0; i < _selectionList.Count; ++i)
		{
			_selectionList[i] = EditorGUILayout.ObjectField(_selectionList[i], typeof(GameObject), true) as GameObject;
		}
		EditorGUILayout.EndScrollView();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Clear"))
			_selectionList.Clear();
		if(GUILayout.Button("Add Selection"))
			_selectionList.AddRange(Selection.gameObjects);
		if(GUILayout.Button("Add Empty Item"))
			_selectionList.Add(null);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("2) Select GameObject to paste them into (null for root):");

		EditorGUILayout.BeginHorizontal();
		_pasteInto = EditorGUILayout.ObjectField(_pasteInto, typeof(GameObject), true) as GameObject;
		if(GUILayout.Button("Put Selection", GUILayout.Width(90f)))
		{
			if(Selection.activeGameObject != null && Selection.gameObjects.Length == 1)
				_pasteInto = Selection.activeGameObject;
		}
		EditorGUILayout.EndHorizontal();

		if(GUILayout.Button("Cut & Paste"))
			cutAndPaste();
	}

	void cutAndPaste()
	{
		foreach(var go in _selectionList)
		{
			if(go == null) continue;
			Undo.SetTransformParent(go.transform, _pasteInto == null ? null : _pasteInto.transform, "CutPaste+"+go.name);
		}
	}
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RandomizeObject : EditorWindow
{
	
	private Vector3 _minPos = -Vector3.one;
	private Vector3 _maxPos = Vector3.one;
	private bool _randomizePositionX = true;
	private bool _randomizePositionY = true;
	private bool _randomizePositionZ = true;

	private Vector3 _minRot = -Vector3.one;
	private Vector3 _maxRot = Vector3.one;
	private bool _randomizeRotationX = true;
	private bool _randomizeRotationY = true;
	private bool _randomizeRotationZ = true;	
	
	private Vector2 _minMaxUniformScale = new Vector2(-1f,1f);
	private Vector3 _minScale = -Vector3.one;
	private Vector3 _maxScale = Vector3.one;
	private bool _randomizeScaleX = true;
	private bool _randomizeScaleY = true;
	private bool _randomizeScaleZ = true;
	private bool _uniformScale = true;
	
	private float _min = -10.0f;
	private float _max = 10.0f;
	
	private Dictionary<GameObject, Triple<Vector3, Vector3, Vector3> > _lastRandom = 
		new Dictionary<GameObject, Triple<Vector3, Vector3, Vector3> >();
	
	[MenuItem("stillalive-studios/Tools/Randomize Object")]
	public static void Init()
	{
		RandomizeObject window = EditorWindow.GetWindow<RandomizeObject>("Randomize Object");
		window.Show();
		window.Focus();
	}
	
	void OnGUI()
	{
		GUILayout.Label("Randomize:");
		GUILayout.Space(10.0f);
		
		// Utility buttons
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Select all")) {
			_randomizePositionX = _randomizePositionY = _randomizePositionZ = 
				_randomizeRotationX = _randomizeRotationY = _randomizeRotationZ =
					_randomizeScaleX = _randomizeScaleY = _randomizeScaleZ = true;
		} else if (GUILayout.Button("Unselect all")) {
			_randomizePositionX = _randomizePositionY = _randomizePositionZ = 
				_randomizeRotationX = _randomizeRotationY = _randomizeRotationZ =
					_randomizeScaleX = _randomizeScaleY = _randomizeScaleZ = false;
		} else if (GUILayout.Button("Reset values")) {
			_min = -10.0f;
			_max = 10.0f;
			
			_minPos = -Vector3.one;
			_maxPos = Vector3.one;
			
			_minRot = -Vector3.one;
			_maxRot = Vector3.one;
			
			_minScale = Vector3.zero;
			_maxScale = Vector3.one;
			_minMaxUniformScale = new Vector2(0f,1f);
		} else if (GUILayout.Button("Undo last")) {
			undoLastRandom();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5.0f);
		
		// Min/Max values
		GUILayout.BeginHorizontal();
		_min = EditorGUILayout.FloatField("Minimum value:", _min);
		_max = EditorGUILayout.FloatField("Maximum value:", _max);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(5.0f);
		
		// Options
		GUILayout.BeginVertical();
		GUILayout.Label("\tPosition:");
		
		GUILayout.BeginHorizontal();
		_randomizePositionX = GUILayout.Toggle(_randomizePositionX, "x:");
		EditorGUI.BeginDisabledGroup(!_randomizePositionX);
		EditorGUILayout.MinMaxSlider(ref _minPos.x, ref _maxPos.x, _min, _max);
		GUILayout.Label("Min: ");
		_minPos.x = EditorGUILayout.FloatField(_minPos.x);
		GUILayout.Label("Max: ");
		_maxPos.x = EditorGUILayout.FloatField(_maxPos.x);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		_randomizePositionY = GUILayout.Toggle(_randomizePositionY, "y:");
		EditorGUI.BeginDisabledGroup(!_randomizePositionY);
		EditorGUILayout.MinMaxSlider(ref _minPos.y, ref _maxPos.y, _min, _max);
		GUILayout.Label("Min: ");
		_minPos.y = EditorGUILayout.FloatField(_minPos.y);
		GUILayout.Label("Max: ");
		_maxPos.y = EditorGUILayout.FloatField(_maxPos.y);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		_randomizePositionZ = GUILayout.Toggle(_randomizePositionZ, "z:");
		EditorGUI.BeginDisabledGroup(!_randomizePositionZ);
		EditorGUILayout.MinMaxSlider(ref _minPos.z, ref _maxPos.z, _min, _max);
		GUILayout.Label("Min: ");
		_minPos.z = EditorGUILayout.FloatField(_minPos.z);
		GUILayout.Label("Max: ");
		_maxPos.z = EditorGUILayout.FloatField(_maxPos.z);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.Space(10.0f);
		
		GUILayout.BeginVertical();
		GUILayout.Label("\tRotation:");
		
		GUILayout.BeginHorizontal();
		_randomizeRotationX = GUILayout.Toggle(_randomizeRotationX, "x:");
		EditorGUI.BeginDisabledGroup(!_randomizeRotationX);
		EditorGUILayout.MinMaxSlider(ref _minRot.x, ref _maxRot.x, _min, _max);
		GUILayout.Label("Min: ");
		_minRot.x = EditorGUILayout.FloatField(_minRot.x);
		GUILayout.Label("Max: ");
		_maxRot.x = EditorGUILayout.FloatField(_maxRot.x);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		_randomizeRotationY = GUILayout.Toggle(_randomizeRotationY, "y:");
		EditorGUI.BeginDisabledGroup(!_randomizeRotationY);
		EditorGUILayout.MinMaxSlider(ref _minRot.y, ref _maxRot.y, _min, _max);
		GUILayout.Label("Min: ");
		_minRot.y = EditorGUILayout.FloatField(_minRot.y);
		GUILayout.Label("Max: ");
		_maxRot.y = EditorGUILayout.FloatField(_maxRot.y);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		_randomizeRotationZ = GUILayout.Toggle(_randomizeRotationZ, "z:");
		EditorGUI.BeginDisabledGroup(!_randomizeRotationZ);
		EditorGUILayout.MinMaxSlider(ref _minRot.z, ref _maxRot.z, _min, _max);
		GUILayout.Label("Min: ");
		_minRot.z = EditorGUILayout.FloatField(_minRot.z);
		GUILayout.Label("Max: ");
		_maxRot.z = EditorGUILayout.FloatField(_maxRot.z);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.Space(10.0f);
		
		
		GUILayout.BeginVertical();
		GUILayout.Label("\tScale:");
		float minScale = Mathf.Max(_min, 0f);
		
		//uniform-scale
		GUILayout.BeginHorizontal();
		_uniformScale = GUILayout.Toggle(_uniformScale, "Uniform Scale:");
		EditorGUI.BeginDisabledGroup(!_uniformScale); //uniform scale
		EditorGUILayout.MinMaxSlider(ref _minMaxUniformScale.x, ref _minMaxUniformScale.y, minScale, _max);
		GUILayout.Label("Min: ");
		_minMaxUniformScale.x = EditorGUILayout.FloatField(_minMaxUniformScale.x);
		GUILayout.Label("Max: ");
		_minMaxUniformScale.y = EditorGUILayout.FloatField(_minMaxUniformScale.y);
		EditorGUI.EndDisabledGroup(); //uniform scale
		GUILayout.EndHorizontal();
		
		EditorGUI.BeginDisabledGroup(_uniformScale); //non-uniform scale
		
		GUILayout.BeginHorizontal();
		_randomizeScaleX = GUILayout.Toggle(_randomizeScaleX, "x:");
		EditorGUI.BeginDisabledGroup(!_randomizeScaleX);
		EditorGUILayout.MinMaxSlider(ref _minScale.x, ref _maxScale.x, minScale, _max);
		GUILayout.Label("Min: ");
		_minScale.x = EditorGUILayout.FloatField(_minScale.x);
		GUILayout.Label("Max: ");
		_maxScale.x = EditorGUILayout.FloatField(_maxScale.x);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		_randomizeScaleY = GUILayout.Toggle(_randomizeScaleY, "y:");
		EditorGUI.BeginDisabledGroup(!_randomizeScaleY);
		EditorGUILayout.MinMaxSlider(ref _minScale.y, ref _maxScale.y, minScale, _max);
		GUILayout.Label("Min: ");
		_minScale.y = EditorGUILayout.FloatField(_minScale.y);
		GUILayout.Label("Max: ");
		_maxScale.y = EditorGUILayout.FloatField(_maxScale.y);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		_randomizeScaleZ = GUILayout.Toggle(_randomizeScaleZ, "z:");
		EditorGUI.BeginDisabledGroup(!_randomizeScaleZ);
		EditorGUILayout.MinMaxSlider(ref _minScale.z, ref _maxScale.z, minScale, _max);
		GUILayout.Label("Min: ");
		_minScale.z = EditorGUILayout.FloatField(_minScale.z);
		GUILayout.Label("Max: ");
		_maxScale.z = EditorGUILayout.FloatField(_maxScale.z);
		EditorGUI.EndDisabledGroup();
		GUILayout.EndHorizontal();
		
		EditorGUI.EndDisabledGroup(); //non-uniform scale
		
		GUILayout.EndVertical();
		
		GUILayout.Space(20.0f);
		
		// Randomize!
		if (GUILayout.Button("Randomize")) {
			randomizeObjects();
		}
	}
	
	private void randomizeObjects()
	{
		_lastRandom.Clear();
		
		// Apply the changes locally on each selected objects
		foreach (Transform obj in Selection.transforms)
		{
			_lastRandom.Add(
				obj.gameObject, 
				new Triple<Vector3, Vector3, Vector3> (
				obj.localPosition,
				obj.eulerAngles,
				obj.localScale));
			
			Vector3 pos = obj.localPosition;
			obj.localPosition = new Vector3(
				pos.x + (_randomizePositionX ? Random.Range(_minPos.x, _maxPos.x) : 0.0f),
				pos.y + (_randomizePositionY ? Random.Range(_minPos.y, _maxPos.y) : 0.0f),
				pos.z + (_randomizePositionZ ? Random.Range(_minPos.z, _maxPos.z) : 0.0f));
			
			if (_randomizeRotationX || _randomizeRotationY || _randomizeRotationZ)
			{
				Vector3 angles = obj.localEulerAngles;
				obj.localEulerAngles = new Vector3(
					(_randomizeRotationX ? Random.Range(_minRot.x, _maxRot.x) : angles.x),
					(_randomizeRotationY ? Random.Range(_minRot.y, _maxRot.y) : angles.y),
					(_randomizeRotationZ ? Random.Range(_minRot.z, _maxRot.z) : angles.z));
			}
			
			if(_uniformScale)
			{
				float uniformScaleChange = Random.Range(_minMaxUniformScale.x, _minMaxUniformScale.y);
				obj.localScale = new Vector3(uniformScaleChange, uniformScaleChange, uniformScaleChange);
			}
			else
			{
				if (_randomizeScaleX || _randomizeScaleY || _randomizeScaleZ)
				{
					Vector3 scale = obj.localScale;
					obj.localScale = new Vector3(
						(_randomizeScaleX ? Random.Range(_minScale.x, _maxScale.x) : scale.x),
						(_randomizeScaleY ? Random.Range(_minScale.y, _maxScale.y) : scale.y),
						(_randomizeScaleZ ? Random.Range(_minScale.z, _maxScale.z) : scale.z));
				}
			}
		}
	}
	
	private void undoLastRandom()
	{
		foreach (KeyValuePair<GameObject, Triple<Vector3, Vector3, Vector3> > kv in _lastRandom)
		{
			kv.Key.transform.localPosition = kv.Value.first;
			kv.Key.transform.eulerAngles = kv.Value.second;
			kv.Key.transform.localScale = kv.Value.third;
		}
	}
}

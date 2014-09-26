using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Tobii.EyeX.Client;
using Tobii.EyeX.Framework;
using Rect = UnityEngine.Rect;

public class EyeXController : SASSingleton<EyeXController>
{
	//------------------------- Property declaration -----------------------------//

	/// <summary>
	/// The gaze point data lifetime determines how long a gaze-point is considered up to date.
	/// Meaning after this time the system reports that there are no gaze points available
	/// </summary>
	public float gazePointDataLifetime = 0.5f;
	public bool showDebugInformation = true;
	/// <summary>
	/// Choice of gaze point data stream to be visualized.
	/// </summary>
	public GazePointDataMode gazePointType = GazePointDataMode.LightlyFiltered;
	
	
	private Vector2 _gazePoint = Vector2.zero; //screen coords
	private Vector2 _gameWindowPosition = Vector2.zero; //screen cords
	private float _lastGazeUpdate = -1f;
	private float _currentTimeStamp = 0f;
	private bool _enableEyeTrackingSupport = true;
	private bool _initialized = false;
	private IEyeXDataProvider<EyeXGazePoint> _gazePointProvider;

	//-------------------------- Method declarations -----------------------------//

	/// <summary>
	/// Returns the filtered gaze point relative to the game windows position (window-screen-coords).
	/// </summary>
	public Vector2 getGazePointPixelCoords() {return _gazePoint - _gameWindowPosition;}
	public Vector2 getGazePointScreenCoords()
	{
		//transform gaze pos from pixel-coordinates into screen-coordinates
		Vector2 coords = getGazePointPixelCoords();
		coords.y = Screen.height - coords.y;
		return coords;
	}
	public Vector3 getGazePointScreenCoords3D()
	{
		var v2 = getGazePointScreenCoords();
		return new Vector3(v2.x, v2.y, 0f);
	}
	public bool isDataAvailable() {return _lastGazeUpdate >= _currentTimeStamp - gazePointDataLifetime;}
	//requires to at least once have received the eye x gaze data
	public bool isEyeXActive() {return _lastGazeUpdate > 0f && _enableEyeTrackingSupport && _initialized;}

	//DEBUG
	//private GUIStyle _labelStyle = new GUIStyle();
	//void OnGUI()
	//{
	//	if(!_enableEyeTrackingSupport) return;

	//	if(isDataAvailable())
	//	{
	//		GUI.color = Color.green;
	//		GUI.Label(new Rect(getGazePointPixelCoords().x, getGazePointPixelCoords().y, 20f, 20f), "X");
	//	}
	//	else
	//	{
	//		_labelStyle.alignment = TextAnchor.LowerRight;
	//		_labelStyle.normal.textColor = Color.white;

	//		GUI.color = Color.red;
	//		const float w = 100f;
	//		const float h = 30f;
	//		GUI.Label(new Rect(Screen.width-w, Screen.height-h, w, h), "No eye signal!");
	//	}
	//}

	//---------------------------------------------------------------------------//

	void Start()
	{
		initialize();
	}

	public void reinitialize()
	{
		if(_initialized) 
		{
			shutdown();
			EyeXHost.GetInstance().ShutdownEyeX();
		}
		initialize();
	}

	public void initialize()
	{
		//check if engine is running
		_enableEyeTrackingSupport = isEyeXEngineRunning();

		if(!_enableEyeTrackingSupport) return;
		if(_initialized) return;

		if(!EyeXHost.GetInstance().IsInitialized) EyeXHost.GetInstance().InitializeEyeX();

		_gazePointProvider = EyeXHost.GetInstance().GetGazePointDataProvider(gazePointType);
		_gazePointProvider.Start();

		_initialized = true;
	}

	//---------------------------------------------------------------------------//

	public override void OnDestroy()
	{
		shutdown();

		base.OnDestroy();
	}

	void shutdown()
	{
		if(!_initialized) return;
		_gazePointProvider.Stop();
		_initialized = false;
	}

	//---------------------------------------------------------------------------//

	void Update()
	{
		if(!_enableEyeTrackingSupport) return;
		if(!_initialized) return;

		_currentTimeStamp = Time.realtimeSinceStartup;

		var point = _gazePointProvider.Last;
		if(point.IsValid && point.IsWithinScreenBounds)
		{
			_gazePoint = point.GUI;
			_lastGazeUpdate = _currentTimeStamp;
		}
	}

	//---------------------------------------------------------------------------//

	/// <summary>
	/// Returns true if the eyeX engine is even running
	/// </summary>
	static public bool isEyeXEngineRunning()
	{
		System.Diagnostics.Process[] running = System.Diagnostics.Process.GetProcesses();
		foreach(System.Diagnostics.Process process in running)
		{
			try
			{
				if(!process.HasExited && process.ProcessName == "Tobii.EyeX.Engine")
					return true;
			}
			catch(System.InvalidOperationException)
			{
				//do nothing
			}
		}
		return false;
	}

	//---------------------------------------------------------------------------//
}

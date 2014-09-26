using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class BuildInfo
{
	static private string _version = null;
	/// <summary>
	/// Gets the version number in the following format: major.minor.build:revision
	/// For instance: 0.8.2:1834
	/// </summary>
	static public string version
	{
		get
		{
			if(_version == null)
			{
				var obj = Resources.Load("version") as TextAsset;
				if(obj != null) _version = obj.text;
				else _version = "0.0.0:0";
			}
			return _version;
		}
	}
	
	/// <summary>
	/// Gets the name of the process (thus usually the name of the executable)
	/// </summary>
	static public string processName
	{
		get
		{
			return System.Diagnostics.Process.GetCurrentProcess().ProcessName;
		}
	}
	
	static public string getBuildLevelInfo()
	{
		return processName + ", Level: " + Application.loadedLevelName + ", v" + version;
	}
}

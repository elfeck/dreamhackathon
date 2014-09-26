using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SASSingleton<T> : SASMonoBehaviour where T : SASSingleton<T>
{
	//------------------------- Property declaration -----------------------------//

	static public T inst 
	{
		get
		{
			if(!Application.isPlaying && _inst == null)
			{
				//in editor when the application is not playing the asking for a singleton
				//instance will search the entire scene for one, and if none is found it will add a new
				//gameobject containing that component
				_inst = FindObjectOfType<T>();
				if(_inst == null)
				{
					//create new gameobject and add this component to it
					var go = new GameObject(typeof(T).ToString() + "_Singleton");
					_inst = go.AddComponent<T>();
				}
			}
			return _inst;
		}
	}
	static private T _inst = null;

	//-------------------------- Method declarations -----------------------------//

	public virtual void Awake()
	{
		if(_inst == null || _inst == (T)(this)) _inst = (T)(this);
		else
		{
			Debug.LogWarning("There is only one singleton allowed of this type! (" + typeof(T).Name + ")", this);
			Destroy(this);
		}
	}
	
	public virtual void OnDestroy()
	{
		if(_inst == this) _inst = null;
	}
}

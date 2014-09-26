using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The SASFriendSingleton is a restricted singleton class, that may only be accessed "globally" if the corresponding
/// class within it is accessed is inherting from <classnameOfsingleton>.Friend. It is actually an empty interface, but
/// marks that class as friend and thus provides access to the only instance (this happens via compile time checks!).
/// It should therefore be used whenever you need a class to have at most one instance, but you do not want to make that
/// instance globally available apart from some very specific classes (reduce unexpected dependencies!).
/// This is to circumvent hidden dependencies when searching for the correct reference via FindObjectOfType and the like!
/// NOTE: Though prettier than a singleton it is still a singleton. So dont spam it all over the place.
/// </summary>
public class SASFriendSingleton<T> : SASMonoBehaviour where T : SASFriendSingleton<T>
{
	//------------------------- Property declaration -----------------------------//

	public interface Friend {}

	static private T _inst = null;

	//-------------------------- Method declarations -----------------------------//

	/// <summary>
	/// Use this method to get access to the one and only instance of this class. Only friends of this singleton
	/// (namely classes who inherit from <classnameOfsingleton>.Friend) are allowed to call this method!
	/// NOTE: This method should most always be called like this: <classnameOfsingleton>.instance(this), where this
	/// will be the object out of which the method is called.
	/// </summary>
	/// <param name="caller">Caller.</param>
	/// <typeparam name="K">The 1st type parameter.</typeparam>
	static public T instance<K>(K caller) where K : SASFriendSingleton<T>.Friend
	{
		return getInstance();
	}

	static protected T inst
	{
		get{return getInstance();}
	}

	static private T getInstance()
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

	//---------------------------------------------------------------------------//

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

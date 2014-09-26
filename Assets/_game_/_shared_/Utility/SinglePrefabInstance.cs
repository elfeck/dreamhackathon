using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Takes care of just allowing one instance of a given gameObject (name actually). As soon as an object with
/// this component and the same name is awoken (Awake()) it is immediately destroyed.
/// </summary>
public class SinglePrefabInstance : SASMonoBehaviour
{
	static private Dictionary<string, SinglePrefabInstance> _instances = new Dictionary<string, SinglePrefabInstance>();

	void Awake()
	{
		SinglePrefabInstance inst = null;
		if(_instances.TryGetValue(gameObject.name, out inst) && inst != null)
		{
			//there is already such an object instantiated. Thus delete this one
			DestroyImmediate(gameObject);
			return;
		}

		_instances[gameObject.name] = this;
	}

	void OnDestroy()
	{
		SinglePrefabInstance inst = null;
		if(_instances.TryGetValue(gameObject.name, out inst) && inst == this)
			_instances.Remove(gameObject.name);
	}
}

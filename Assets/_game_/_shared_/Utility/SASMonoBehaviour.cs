using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// This class extents the basic monobehaviour provided by Unity, by several new methods and properties
/// </summary>
public class SASMonoBehaviour : 
#if ULINK_SUPPORT
	uLink.MonoBehaviour
#else
	MonoBehaviour
#endif
{
//	public uLink.NetworkView uNetwork { get {return GetComponent<uLink.NetworkView>();} }

	//---------------------------------------------------------------------------//
	
	/// <summary>
	/// Invoke the specified task and time (Type-Safe).
	/// </summary>
	/// <param name='task'>
	/// Signature for the method: public void InvokeTask();
	/// </param>
	public void Invoke(System.Action task, float time)
	{
		Invoke(task.Method.Name, time);
	}
	
	/// <summary>
	/// Invoke the specified task and time (Type-Safe).
	/// </summary>
	/// <param name='task'>
	/// Signature for the method: public void InvokeTask();
	/// </param>
	public void InvokeRepeating(System.Action task, float time, float interval)
	{
		//WORKAROUND for weird unity bug with InvokeRepeating not working when delay (=time) equals zero!
		if(time == 0f) time = 0.001f;
		InvokeRepeating(task.Method.Name, time, interval);
	}

	/// <summary>
	/// Invoke the specified task and time (Type-Safe).
	/// The start delay is randomized between 0.001f and interval.
	/// </summary>
	public void InvokeRepeating(System.Action task, float interval)
	{
		InvokeRepeating(task, Random.Range(0.001f, interval), interval);
	}
	
	/// <summary>
	/// Cancels the invoke of a specific task (Type-Safe).
	/// </summary>
	/// <param name='task'>
	/// Signature for the method: public void InvokeTask();
	/// </param>
	public void CancelInvoke(System.Action task)
	{
		CancelInvoke(task.Method.Name);
	}

	/// <summary>
	/// Since Unity still calls all invokes also IF the object is simply deactivated, we wanted to
	/// not have this behavior and thus cancel ALL invokes OnDisable, which is heavily used for all poolable objects for instance.
	/// </summary>
	protected virtual void OnDisable()
	{
		//cancels ALL invokes
		CancelInvoke();
	}

	//---------------------------------------------------------------------------//
	
	/// <summary>
	/// Gets the first component implementing the passed interface.
	/// </summary>
	public I GetInterfaceComponent<I>() where I : class
	{
		return GetComponent(typeof(I)) as I;
	}
	
	/// <summary>
	/// Finds all object implementing a certain interface I. Note that this method is quite slow!
	/// </summary>
	public static List<I> FindObjectsOfInterface<I>() where I : class
	{
		MonoBehaviour[] monoBehaviours = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];
		List<I> list = new List<I>();
		foreach(MonoBehaviour behaviour in monoBehaviours)
		{
			I component = behaviour.GetComponent(typeof(I)) as I;
			if(component != null) list.Add(component);
		}
		return list;
	}
	
	/// <summary>
	/// Traverses the hierarchy from this object to the root and searches in each object for the component asked.
	/// The first occurence is returned immediately. If nothing can be found null is returned.
	/// </summary>
	/// <returns>
	/// A reference to the component found in parents.
	/// </returns>
	public T getComponentInParents<T>() where T : Component
	{
		Transform curr = this.transform;
		while(curr)
		{
			T tmp = curr.GetComponent<T>();
			if(tmp) return tmp;
			curr = curr.parent;
		}
		return default(T);
	}
	
#if ULINK_SUPPORT
	private bool _networkViewInitialized = false;
	private uLink.NetworkView _networkViewCached = null;
	protected uLink.NetworkView _networkView
	{
		get
		{
			if(!_networkViewInitialized)
			{
				_networkViewCached = GetComponent<uLink.NetworkView>();
				#if DEBUG_BUILD
				if(_networkViewCached == null)
				{
					if(GetComponent<UnityEngine.NetworkView>() != null)
						Debug.LogError(name + " has a Unity.NetworkView attached, but we must use uLink.NetworkView! This message is displayed only once per object.", gameObject);
					else
						Debug.LogError(name + " cannot invoke RPCs, it has no NetworkView! This message is displayed only once per object.", gameObject);
				}
				#endif
				_networkViewInitialized = true;
			}
			return _networkViewCached;
		}

		set
		{
			_networkViewCached = value;
			_networkViewInitialized = true;
		}
	}

	public void syncedInvoke(uLink.NetworkFlags flags, string rpc, uLink.NetworkPlayer player, params object[] paramList)
	{
#if !DEBUG_BUILD
		if(Registry.isLocalGame)
		{
			_localInvoke(rpc, paramList);
		}
		else
#endif
#if DEBUG_BUILD
		if(_networkView != null)
#endif
		{
			try
			{
				_networkView.RPC(flags, rpc, player, paramList);
			}
			catch
			{
				throw; //rethrowing here improves the log output
			}
		}
	}
	/// <summary>
	/// Invokes on the server.
	/// </summary>
	public void syncedInvokeServer(string rpc, params object[] paramList)
	{
		syncedInvoke(uLink.NetworkFlags.NoTimestamp, rpc, uLink.RPCMode.Server, paramList);
	}
	/// <summary>
	/// Invokes on ALL.
	/// </summary>
	public void syncedInvokeAll(string rpc, params object[] paramList)
	{
		syncedInvoke(uLink.NetworkFlags.NoTimestamp, rpc, uLink.RPCMode.All, paramList);
	}
	public void syncedInvoke(uLink.NetworkFlags flags, string rpc, uLink.RPCMode mode, params object[] paramList)
	{
#if !DEBUG_BUILD
		if(Registry.isLocalGame)
		{
			_localInvoke(rpc, paramList);
		}
		else
#endif
#if DEBUG_BUILD
		if(_networkView != null)
#endif
		{
			try
			{
				_networkView.RPC(flags, rpc, mode, paramList);
			}
			catch
			{
				Debug.LogError(name + ": Error on RPC!", gameObject);
				throw; //rethrowing here improves the log output
			}
		}
	}

#if !DEBUG_BUILD
	private void _localInvoke(string rpc, params object[] paramList)
	{
		System.Type type = GetType();
		if(type == null)
		{
			Debug.LogError("Something definitely went wrong..syncedIncoke: " + rpc);
			return;
		}

		MethodInfo mi = type.GetMethod(rpc, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding);
		if(mi == null)
		{
			Debug.LogError("syncedInvoke: method " + rpc + " not found!", gameObject);
			return;
		}

		try
		{
			mi.Invoke(this, paramList);
		}
		catch(System.Exception e)
		{
			Debug.LogError(e, gameObject);
		}
	}
#endif
#endif
}
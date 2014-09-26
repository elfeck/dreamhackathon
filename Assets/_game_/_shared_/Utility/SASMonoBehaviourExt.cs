using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate IEnumerator SASCoroutine();

/// <summary>
/// This class further extents the functionality of SASMonobehavior.
/// </summary>
public class SASMonoBehaviourExt : SASMonoBehaviour
{
	//----------------------------------------------------------------//
	
	public class CoroutineInfo
	{
		public IEnumerator curr;
		public bool continueAfterReactivation = false;
		public bool running = true;
	}
	private HashSet<CoroutineInfo> _activeCoroutines = new HashSet<CoroutineInfo>();
	private List<CoroutineInfo> _coroutinesToReenable = new List<CoroutineInfo>();
	
	protected virtual void OnEnable()
	{
		foreach(var info in _coroutinesToReenable)
			StartCoroutine(SASParentCoroutine(info));
		_coroutinesToReenable.Clear();
	}
	
	protected override void OnDisable()
	{
		base.OnDisable();
		
		foreach(var info in _activeCoroutines)
		{
			if(info.continueAfterReactivation) _coroutinesToReenable.Add(info);
		}
		_activeCoroutines.Clear();
	}
	
	/// <summary>
	/// Starts a monitored co-routine with the possibility to resume it even after object deactivation and re-activation.
	/// Also returns an info object to hold the coroutine's information. E.g. if it is still running.
	/// </summary>
	public CoroutineInfo StartCoroutine(SASCoroutine method, bool continueAfterObjectReactivation)
	{
		CoroutineInfo info = new CoroutineInfo();
		info.continueAfterReactivation = continueAfterObjectReactivation;
		info.running = true;
		info.curr = method();
		StartCoroutine(SASParentCoroutine(info));
		return info;
	}

	IEnumerator SASParentCoroutine(CoroutineInfo info)
	{
		_activeCoroutines.Add(info);
		
		while(true)
		{
			yield return info.curr.Current;
			bool ok = info.curr.MoveNext();
			if(!ok) break;
		}
		
		//unreg
		info.running = false;
		_activeCoroutines.Remove(info);
	}
	
	//----------------------------------------------------------------//
}

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Reverts the GameObject to the Prefab at a given event or manually.
/// \authors Tobias Mayr
/// </summary>
[AddComponentMenu("stillalive-studios/Utility/Revert To Prefab On Event")]
[ExecuteInEditMode]
public class RevertToPrefabOnEvent : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	public enum ResetEvent {None, OnEnable, OnDisable};
	/// <summary>
	/// Select the event that triggers the reset. None if you want to do it manually only.
	/// </summary>
	public ResetEvent resetEvent = ResetEvent.OnEnable;
	/// <summary>
	/// Also revert to prefab if the connection is broken.
	/// </summary>
	public bool revertDisconnected = true;

#if UNITY_EDITOR
	// This tow flags are needed to break an infinite loop OnEnable->Revert->OnEnable... (same with OnDisable)
	private bool _onEnableCalled = false;
	private bool _onDisableCalled = false;
#endif

	//-------------------------- Method declarations -----------------------------//

	public void revert()
	{
#if UNITY_EDITOR
		if(!EditorApplication.isPlayingOrWillChangePlaymode)
		{
			if(gameObject != null && (PrefabUtility.GetPrefabType(gameObject) == PrefabType.PrefabInstance ||
			                          (revertDisconnected && PrefabUtility.GetPrefabType(gameObject) == PrefabType.DisconnectedPrefabInstance)))
			{
				//these are set true here so it also works when calling reset manually
				_onEnableCalled = true;
				_onDisableCalled = true;

				if(revertDisconnected && PrefabUtility.GetPrefabType(gameObject) == PrefabType.DisconnectedPrefabInstance)
					PrefabUtility.ReconnectToLastPrefab(gameObject);

				PrefabUtility.RevertPrefabInstance(gameObject);
				EditorUtility.SetDirty(gameObject);
			}
		}
		else
#endif
			Debug.LogWarning("Cannot revert to prefab in play mode!");
	}

#if UNITY_EDITOR
	void OnEnable()
	{
		if(!_onEnableCalled && !EditorApplication.isPlayingOrWillChangePlaymode)
		{
			if(resetEvent == ResetEvent.OnEnable) revert();
		}

		_onEnableCalled = false;
	}
	
	protected override void OnDisable()
	{
		base.OnDisable();

		if(!_onDisableCalled && !EditorApplication.isPlayingOrWillChangePlaymode)
		{
			if(resetEvent == ResetEvent.OnDisable) revert();
		}

		_onDisableCalled = false;
	}
#endif
}

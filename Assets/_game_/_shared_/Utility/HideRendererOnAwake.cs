using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Hide Renderer On Awake")]
public class HideRendererOnAwake : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	public bool hideChildRendererToo = false;

	//-------------------------- Method declarations -----------------------------//

	void Start()
	{
#if DEBUG_BUILD
		DeactivateHideRenderer dhr = DeactivateHideRenderer.inst;
		if(dhr != null && dhr.deactivateHideRendererOnAwake) return;
#endif
		
		hideRenderer();
	}
	
	public void hideRenderer()
	{
		Renderer[] arr = null;
		if(!hideChildRendererToo) arr = GetComponents<Renderer>();
		else arr = GetComponentsInChildren<Renderer>();
		
		foreach(var r in arr)
#if DEBUG_BUILD
			r.enabled = false;
#else
			Destroy(r);
#endif
	}
	
	public void showRenderer()
	{
		Renderer[] arr = null;
		if(!hideChildRendererToo) arr = GetComponents<Renderer>();
		else arr = GetComponentsInChildren<Renderer>();
		
		foreach(var r in arr)
			r.enabled = true;
	}
}

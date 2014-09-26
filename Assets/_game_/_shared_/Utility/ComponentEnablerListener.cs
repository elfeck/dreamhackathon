using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ComponentEnablerListenerState {Enable, Disable, Delete}

[AddComponentMenu("stillalive-studios/Utility/Component Dis-Enabler Event Listener")]
public class ComponentEnablerListener : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	public GameObject eventSender = null;
	public string eventToTrigger = "";
	public MonoBehaviour component = null;
	public ComponentEnablerListenerState onEvent = ComponentEnablerListenerState.Disable;

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{
		if(eventSender == null) eventSender = gameObject;
		if(eventToTrigger != "")
		{
			Messenger.AddListener(eventToTrigger, eventSender, modifyComponent);
			Messenger<GameObject>.AddListener(eventToTrigger, eventSender, modifyComponent);
		}
	}
	
	void modifyComponent(GameObject obj)
	{
		modifyComponent();
	}
	
	void modifyComponent()
	{
		if(component == null) return;
		
		if(onEvent == ComponentEnablerListenerState.Delete) Destroy(component);
		else if(onEvent == ComponentEnablerListenerState.Disable) component.enabled = false;
		else if(onEvent == ComponentEnablerListenerState.Enable) component.enabled = true;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectActivationTrigger : SASMonoBehaviour
{
	
	public List<GameObject> objectsToActivate = new List<GameObject>();
	public bool fadeInAndOut = true;
	public float fadeInTime = 1f;
	public float fadeOutTime = 1f;
	[HideInInspector]
	public static string[] standard_tags = {"Player","NetworkPlayer"};
	public List<string> triggerOnlyForTags = new List<string>(standard_tags);
	public float timeout = 2f;
	
	private int _objectsCount = 0;
	private float _lastTrigger = 0f;
	
	void Awake()
	{
		foreach(GameObject objectToActivate in objectsToActivate)
		{
			if(fadeInAndOut)
			{
				if(objectToActivate != null)
				{
					MeshRenderer renderer = objectToActivate.GetComponent<MeshRenderer>();
					//If the object has a renderer, set up the materials alpha value
					if(renderer != null) 
					{
						Color matColor = renderer.material.color;
						matColor.a = 0;
						renderer.material.color = matColor;
					}
					if(!fadeInAndOut || gameObject.activeSelf) objectToActivate.SetActive(gameObject.activeSelf);
				}
			}
			if(objectToActivate != null && objectToActivate.activeSelf)
			{
//				Debug.LogWarning("The object to activate by ObjectActivationTrigger should be deactivated by default.");
				objectToActivate.SetActive(false);
			}
		}
		if(triggerOnlyForTags == null || triggerOnlyForTags.Count<1)
			triggerOnlyForTags = new List<string>(standard_tags);
	}
	void OnEnable()
	{
		InvokeRepeating(checkTimeOut,timeout,timeout);
	}
	
	void checkTimeOut()
	{
		if(_lastTrigger+timeout>Time.time) return;
		//Deactivate if timed out
		_objectsCount = 0;
		objectsSetActive(false);
	}

	void OnTriggerEnter(Collider coll)
	{
		if(!foundTag(coll)) return;
		_objectsCount++;
		if(_objectsCount>1) return;
		objectsSetActive(true);
	}
	
	void OnTriggerStay(Collider coll)
	{
		if(!foundTag(coll)) return;
		//Update the trigger time
		_lastTrigger = Time.time;	
	}
	
	void OnTriggerExit(Collider coll)
	{
		if(!foundTag(coll)) return;
		_objectsCount--;
		if(_objectsCount>0) return;
		objectsSetActive(false);
	}
	
	private bool _active = false;
	void objectsSetActive(bool active)
	{
		_active = active;
		foreach(GameObject objectToActivate in objectsToActivate)
		{
			if(objectToActivate != null)
			{
				if(!fadeInAndOut || active) objectToActivate.SetActive(active);
			}
		}
	}
	
	
	
	void Update()
	{
		if(fadeInAndOut)
		{
			foreach(GameObject obj in objectsToActivate)
			{
				if(obj != null)
				{
					MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
					//Return if the object has no renderer
					if(renderer != null)
					{
						Material mat = renderer.material;
						Color matColor = renderer.material.color;
						if(_active && mat.color.a<1)
							matColor.a = fadeInTime<=0 ? 1f : Mathf.Min(1f,mat.color.a+(Time.deltaTime/fadeInTime));
						if(!_active)
						{
							if(mat.color.a>0)
								matColor.a = fadeOutTime<=0 ? 0f : Mathf.Max(0f,mat.color.a-(Time.deltaTime/fadeOutTime));
							else
								obj.SetActive(false);
						}
						renderer.material.color = matColor;
					}
				}
			}
		}
	}
	
	bool foundTag(Collider coll)
	{
		foreach(string tag in triggerOnlyForTags)
			if(tag.Equals(coll.tag)) return true;
		return false;
	}
}

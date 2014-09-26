using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Utility/Switch Light On")]
public class SwitchOnLight : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	public bool disableOnAwake = true;
	public TriggerDelegate enableOnTrigger = null;
	public TriggerDelegate disableOnTrigger = null;
	public float fadingFactor = 0.1f;
	
	private float _currTargetIntensity;
	private float _fullIntensity;
	private Light _light;

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{
		if(enableOnTrigger != null) enableOnTrigger.registerOnTriggerEnter(switchOn);
		if(disableOnTrigger != null) disableOnTrigger.registerOnTriggerEnter(switchOff);
		
		_light = light;
		_fullIntensity = _light.intensity;
		if(disableOnAwake)
		{
			_light.enabled = false;
			_light.intensity = 0f;
		}
	}
	
	public void switchOn()
	{
		_currTargetIntensity = _fullIntensity;
		_light.enabled = true;
	}
	
	public void switchOff()
	{
		_currTargetIntensity = 0f;
	}
	
	void Update()
	{
		if(!_light.enabled && _currTargetIntensity <= 0f) return;
		
		_light.intensity = HelperFunctions.applyRecursiveFilter(_light.intensity, _currTargetIntensity, fadingFactor);
		if(_light.intensity < 0.001f)
		{
			_light.enabled = false;
			_light.intensity = 0f;
		}
	}
}

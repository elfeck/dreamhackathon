using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Trigger/Ambient Light Changer")]
public class AmbientLightChanger : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	static public Color startAmbientLightColor = RenderSettings.ambientLight;
	static public Color targetAmbientLightColor = RenderSettings.ambientLight;
	static public float interpolationEndTime = -1f;
	static public float interpolationStartTime = -1f;
//	static public bool interpolationSmoothStep = false;
	
	public TriggerDelegate triggerVolume = null;
	public Color ambientLightColor = Color.gray;
	public float changeDuration = 1f;
//	public bool useSmoothStepInterpolation = false;

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{	
		if(triggerVolume) triggerVolume.registerOnTriggerEnter(changeAmbientLight);
	}
	
	public void changeAmbientLight()
	{
		AmbientLightChanger.startAmbientLightColor = RenderSettings.ambientLight;
		AmbientLightChanger.targetAmbientLightColor = ambientLightColor;
		AmbientLightChanger.interpolationStartTime = Time.time;
		AmbientLightChanger.interpolationEndTime = Time.time + changeDuration;
	}
	
	public void Update()
	{
		if(Time.time > AmbientLightChanger.interpolationEndTime) return;
		
		float f = Mathf.Clamp01((Time.time - interpolationStartTime) / (interpolationEndTime - interpolationStartTime));
		RenderSettings.ambientLight = Color.Lerp(AmbientLightChanger.startAmbientLightColor, AmbientLightChanger.targetAmbientLightColor, f);
	}
}

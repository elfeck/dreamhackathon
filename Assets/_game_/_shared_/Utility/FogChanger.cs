using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("stillalive-studios/Trigger/Fog Changer")]
public class FogChanger : SASMonoBehaviour
{
	//------------------------- Property declaration -----------------------------//
	
	static public Color startFogColor;
	static public Color targetFogColor;
	static public float interpolationEndTime = -1f;
	static public float interpolationStartTime = -1f;
	
	public TriggerDelegate triggerVolume = null;
	public enum FogStateChanges {NoChange, Toggle, Enable, Disable}
	public FogStateChanges changeFogState = FogStateChanges.NoChange;
	public bool changeFogColor = false;
	public Color fogColor = Color.gray;
	public float colorChangeDuration = 1f;

	//-------------------------- Method declarations -----------------------------//

	void Awake()
	{	
		if(triggerVolume) triggerVolume.registerOnTriggerEnter(changeFog);
		FogChanger.startFogColor = RenderSettings.fogColor;
		FogChanger.targetFogColor = RenderSettings.fogColor;
	}
	
	public void changeFog()
	{
		if(changeFogColor)
		{
			FogChanger.startFogColor = RenderSettings.fogColor;
			FogChanger.targetFogColor = fogColor;
			FogChanger.interpolationStartTime = Time.time;
			FogChanger.interpolationEndTime = Time.time + colorChangeDuration;
		}
		
		if(changeFogState == FogStateChanges.Enable) RenderSettings.fog = true;
		else if(changeFogState == FogStateChanges.Disable) RenderSettings.fog = false;
		else if(changeFogState == FogStateChanges.Toggle) RenderSettings.fog = !RenderSettings.fog;
	}
	
	public void Update()
	{
		if(Time.time > AmbientLightChanger.interpolationEndTime) return;
		
		float f = Mathf.Clamp01((Time.time - interpolationStartTime) / (interpolationEndTime - interpolationStartTime));
		RenderSettings.ambientLight = Color.Lerp(FogChanger.startFogColor, FogChanger.targetFogColor, f);
	}
}

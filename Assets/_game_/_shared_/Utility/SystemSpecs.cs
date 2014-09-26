using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SystemSpecs : SASMonoBehaviour
{
	/// <summary>
	/// Gets the system specs with detailed information about the hardware.
	/// </summary>
	static public string getInfo()
	{
		return ""
			+"CPU: " + SystemInfo.processorType + "[Cores: " + SystemInfo.processorCount + "]"+"\n"
			+"RAM: " + SystemInfo.systemMemorySize+"MB\n"
			+"Graphics: " + SystemInfo.graphicsDeviceVendor + " [" + SystemInfo.graphicsDeviceVendorID + "] " + SystemInfo.graphicsDeviceName
				+ " [" + SystemInfo.graphicsDeviceID + "] v" + SystemInfo.graphicsDeviceVersion+"\n"
			+"Graphics RAM: " + SystemInfo.graphicsMemorySize+"MB\n"
			+"ShaderLevel: " + SystemInfo.graphicsShaderLevel+"\n"
			+"OS: " + SystemInfo.operatingSystem+"\n"
			+"Device: " + SystemInfo.deviceModel + ", " + SystemInfo.deviceName + ", " + SystemInfo.deviceType + " [" 
				+ SystemInfo.deviceUniqueIdentifier + "]"+"\n"
			;
	}
}

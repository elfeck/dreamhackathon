using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// PropertyAttribute to give names to the fields in a Vector4
/// </summary>
public class SASVector4Attribute : PropertyAttribute
{
	public readonly string x;
	public readonly string y;
	public readonly string z;
	public readonly string w;
	public SASVector4Attribute(string x, string y, string z, string w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}
}

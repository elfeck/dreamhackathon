//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using System.Collections;
using HeavyDutyInspector;

[System.Serializable]
public abstract class NamedMonoBehaviour : MonoBehaviour {

	[NMBName]
	public string	scriptName;

	[NMBColor]
	public Color	scriptNameColor = Color.green;
}
//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	public class HideVariableAttribute : PropertyAttribute {

		/// <summary>
		/// Works like HideInInspector but doesn't prevent DecoratorDrawers from being displayed
		/// </summary>
		public HideVariableAttribute()
		{
			
		}
	}
	
}
	
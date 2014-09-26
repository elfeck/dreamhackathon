//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	public class NamedMonoBehaviourAttribute : PropertyAttribute {

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		public Type type;

		/// <summary>
		/// Displays a reference using the NamedMonoBehaviour Drawer. Use with variables of a type extending NamedMonoBehaviour.
		/// </summary>
		/// <param name="classType">The type of the variable. This has to be a class extending NamedMonoBehaviour.</param>
		public NamedMonoBehaviourAttribute(Type classType)
		{
			type = classType;
		}
	#endif

		/// <summary>
		/// Displays a reference using the NamedMonoBehaviour Drawer. Use with variables of a type extending NamedMonoBehaviour.
		/// </summary>
		public NamedMonoBehaviourAttribute()
		{
			// Only here for the summary
		}
	}

}

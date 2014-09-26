//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	public class ReorderableListAttribute : PropertyAttribute {
		
		public Texture2D arrowUp
		{
			get;
			private set;
		}

		public Texture2D arrowDown
		{
			get;
			private set;
		}
		
		public bool useNamedMonoBehaviourDrawer
		{
			get;
			private set;
		}
		
		public bool doubleComponentRefSizeInChildren
		{
			get;
			private set;
		}

		/// <summary>
		/// Adds to a list the ability to reorder its content and add or remove items from anywhere in the list.
		/// </summary>
		/// <param name="listName">The list's name to access it via reflection.</param>
		/// <param name="doubleComponentRefSizeInChildren">If set to <c>true</c> doubles the height of references in children serialized objects (Used if your children use NamedMonoBehaviourAttribute or ComponentSelectionAttribute)</param>
		/// <param name="useNamedMonoBehaviourDrawer">If set to <c>true</c> to display NamedMonoBehaviour with the NamedMonoBehaviour drawer. By default, all Components are displayed with the ComponentSelection drawer.</param>
		public ReorderableListAttribute(bool doubleComponentRefSizeInChildren = false, bool useNamedMonoBehaviourDrawer = false)
		{
	#if UNITY_EDITOR
			arrowUp = (Texture2D)Resources.Load("ArrowUp");
			arrowDown = (Texture2D)Resources.Load("ArrowDown");
	#endif
			this.doubleComponentRefSizeInChildren = doubleComponentRefSizeInChildren;
			this.useNamedMonoBehaviourDrawer = useNamedMonoBehaviourDrawer;
		}
	}

}
	#endif

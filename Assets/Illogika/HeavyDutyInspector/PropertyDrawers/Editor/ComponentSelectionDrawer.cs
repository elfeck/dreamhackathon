//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ComponentSelectionAttribute))]
	public class ComponentSelectionDrawer : IllogikaDrawer {
			
		ComponentSelectionAttribute componentSelectionAttribute { get { return ((ComponentSelectionAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return base.GetPropertyHeight(prop, label) * 2;
	    }

		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			OnComponentGUI(position, prop, label, componentSelectionAttribute.componentType, componentSelectionAttribute.fieldName, componentSelectionAttribute.requiredValues, componentSelectionAttribute.defaultObject, componentSelectionAttribute.isPrefab, 0);
	#else
			OnComponentGUI(position, prop, label, componentSelectionAttribute.fieldName, componentSelectionAttribute.requiredValues, componentSelectionAttribute.defaultObject, componentSelectionAttribute.isPrefab, 0);
	#endif
			EditorGUI.EndProperty();
		}
	}

}

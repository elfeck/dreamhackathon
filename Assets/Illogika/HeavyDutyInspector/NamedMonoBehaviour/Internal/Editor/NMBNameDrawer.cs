//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(NMBNameAttribute))]
	public class NMBNameDrawer : IllogikaDrawer {
			
		NMBNameAttribute nmbNameAttribute { get { return ((NMBNameAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return base.GetPropertyHeight(prop, label);
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			int originalIndentLevel = EditorGUI.indentLevel;

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID (FocusType.Passive), label);
			EditorGUI.indentLevel = 0;

			position.width -= 40;

			if(prop.hasMultipleDifferentValues)
			{
				EditorGUI.BeginChangeCheck();
				
				string temp = EditorGUI.TextField(position, "-");
				
				if(EditorGUI.EndChangeCheck())
					prop.stringValue = temp;
			}
			else
			{
				prop.stringValue = EditorGUI.TextField(position, prop.stringValue);
			}

			EditorGUI.indentLevel = originalIndentLevel;
	#else
			position.width -= 55;

			EditorGUI.PropertyField(position, prop);
	#endif
			EditorGUI.EndProperty();
		}
	}

}

//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(TextAreaAttribute))]
	public class TextAreaDrawer : IllogikaDrawer {
			
		TextAreaAttribute textAreaAttribute { get { return ((TextAreaAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			GUIStyle style = "TextArea";
			return Mathf.Max(style.CalcHeight(new GUIContent(prop.stringValue), Screen.width - 18), base.GetPropertyHeight(prop, label)) + base.GetPropertyHeight(prop, label);
	    }

		public float GetBasePropertyHeight (SerializedProperty prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, label);
		}
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);
			
			if(prop.propertyType != SerializedPropertyType.String)
			{
				WrongVariableTypeWarning("AssetPath", "strings");
				return;
			}
			
			EditorGUI.PrefixLabel(position, GUIUtility.GetControlID (FocusType.Passive), label);

			position.y += GetBasePropertyHeight(prop, label);
			position.height -= GetBasePropertyHeight(prop, label);

			GUIStyle style = "TextArea";
			style.stretchWidth = false;
			style.stretchHeight = true;

			prop.stringValue = EditorGUI.TextArea(position, prop.stringValue, style);

			EditorGUI.EndProperty();
		}
	}

}
#endif

﻿//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2014  Illogika
//----------------------------------------------
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(TagListAttribute))]
	public class TagListDrawer : IllogikaDrawer {
			
		TagListAttribute tagListAttribute { get { return ((TagListAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			if(prop.serializedObject.targetObjects.Length > 1)
			{
				if(int.Parse(prop.propertyPath.Split('[')[1].Split(']')[0]) != 0)
					return -2.0f;
				else
					return base.GetPropertyHeight(prop, label) * 2;
			}

	    	return base.GetPropertyHeight(prop, label);
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			int index = int.Parse(prop.propertyPath.Split('[')[1].Split(']')[0]);

			IList list = null;
			try
			{
				list = (prop.serializedObject.targetObject as MonoBehaviour).GetType().GetField(prop.propertyPath.Split('.')[0]).GetValue(prop.serializedObject.targetObject) as IList;
			}
			catch
			{
				try{
					list = (prop.serializedObject.targetObject as ScriptableObject).GetType().GetField(prop.propertyPath.Split('.')[0]).GetValue(prop.serializedObject.targetObject) as IList;
				}
				catch{
					Debug.LogWarning(string.Format("The script has no property named {0} or {0} is not an IList",prop.propertyPath.Split('.')[0]));
				}
			}

			if(prop.serializedObject.targetObjects.Length > 1)
			{
				if(index == 0)
				{
					position.height = base.GetPropertyHeight(prop, label) * 2;
					EditorGUI.indentLevel = 1;
					position = EditorGUI.IndentedRect(position);
					EditorGUI.HelpBox(position, "Multi object editing is not supported.", MessageType.Warning);
				}
				return;
			}

			int originalIndentLevel = EditorGUI.indentLevel;

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID (FocusType.Passive), label);
			EditorGUI.indentLevel = 0;

			if(tagListAttribute.canDeleteFirstElement || index != 0)
				position.width -= 18;

			if(prop.stringValue == "")
				prop.stringValue = "Untagged";

			prop.stringValue = EditorGUI.TagField(position, prop.stringValue);

			position.x += position.width;
			position.width = 16;

			if((tagListAttribute.canDeleteFirstElement || index != 0) && GUI.Button(position, "", "OL Minus"))
			{
				list.RemoveAt(index);
			}


			EditorGUI.indentLevel = originalIndentLevel;
			EditorGUI.EndProperty();
		}
	}

}
#endif

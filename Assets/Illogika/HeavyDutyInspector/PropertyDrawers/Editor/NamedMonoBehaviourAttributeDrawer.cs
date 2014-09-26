//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(NamedMonoBehaviourAttribute))]
	public class NamedMonoBehaviourAttributeDrawer : NamedMonoBehaviourDrawer {
		
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		NamedMonoBehaviourAttribute namedMonoBehaviourAttribute { get { return ((NamedMonoBehaviourAttribute)attribute); } }
	#endif	

		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return base.GetPropertyHeight(prop, label);
	    }
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			if(property.propertyType != SerializedPropertyType.ObjectReference)
			{
				WrongVariableTypeWarning("NamedMonoBehaviour", "object references");
				return;
			}

	#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
			if(!fieldInfo.FieldType.IsArray && !fieldInfo.FieldType.IsSubclassOf(typeof(NamedMonoBehaviour)) && !(fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(NamedMonoBehaviour))))
			{
				WrongVariableTypeWarning("NamedMonoBehaviour", "classes extending NamedMonoBehaviour");
				return;
			}
	#endif

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			OnNamedMonoBehaviourGUI(position, property, label, namedMonoBehaviourAttribute.type, 0);
	#else
			Type type; 
			if(fieldInfo.FieldType.IsArray)
				type = fieldInfo.FieldType.GetElementType();
			else if(fieldInfo.FieldType.IsGenericType)
				type = fieldInfo.FieldType.GetGenericArguments()[0];
			else
				type = fieldInfo.FieldType;

			OnNamedMonoBehaviourGUI(position, property, label, type, 0);
	#endif
			EditorGUI.EndProperty();
		}
	}

}

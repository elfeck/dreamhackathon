//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2014  Illogika
//----------------------------------------------
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(HideConditionalAttribute))]
	public class HideConditionalDrawer : IllogikaDrawer {
			
		HideConditionalAttribute hideConditionalAttribute { get { return ((HideConditionalAttribute)attribute); } }

		public bool isVisible(SerializedProperty prop)
		{
			switch(hideConditionalAttribute.conditionType)
			{
			case HideConditionalAttribute.ConditionType.IsNotNull:
				return fieldInfo.DeclaringType.GetField(hideConditionalAttribute.variableName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(prop.serializedObject.targetObject) != null;
			case HideConditionalAttribute.ConditionType.IntOrEnum:
				return hideConditionalAttribute.enumValues.Contains((int)fieldInfo.DeclaringType.GetField(hideConditionalAttribute.variableName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(prop.serializedObject.targetObject));
			case HideConditionalAttribute.ConditionType.FloatRange:
				if(hideConditionalAttribute.minValue < hideConditionalAttribute.maxValue)
				{
					Debug.LogError("Min value has to be lower than max value");
					return false;
				}
				else
				{
					return (float)fieldInfo.DeclaringType.GetField(hideConditionalAttribute.variableName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(prop.serializedObject.targetObject) >= hideConditionalAttribute.minValue &&
						(float)fieldInfo.DeclaringType.GetField(hideConditionalAttribute.variableName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(prop.serializedObject.targetObject) <= hideConditionalAttribute.maxValue;
				}
			case HideConditionalAttribute.ConditionType.Bool:
				return (bool)fieldInfo.DeclaringType.GetField(hideConditionalAttribute.variableName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(prop.serializedObject.targetObject) == hideConditionalAttribute.boolValue;
			default:
				break;
			}
			return false;
		}

		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			if(isVisible(prop))
	    		return base.GetPropertyHeight(prop, label);

			return -2.0f;
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			if(isVisible(prop))
			{
				EditorGUI.PropertyField(position, prop);
			}

			EditorGUI.EndProperty();
		}
	}

}
#endif

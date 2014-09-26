//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public class ButtonDrawer : IllogikaDrawer {
				
		ButtonAttribute buttonAttribute { get { return ((ButtonAttribute)attribute); } }
		
		bool ShowVariable(SerializedProperty prop)
		{
			bool showVariable = !buttonAttribute.hideVariable;

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			showVariable &= CanDisplayVariable(prop);
	#endif
				
			return showVariable;
		}
			
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			float baseHeight = base.GetPropertyHeight(prop, label);
			return ShowVariable(prop) ? baseHeight * 2 : baseHeight;
	    }

		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			bool showVariable = ShowVariable(prop);
				
			if (showVariable)
				position.height /= 2;

			if(GUI.Button(EditorGUI.IndentedRect(position), buttonAttribute.buttonText))
			{
				foreach(Object obj in prop.serializedObject.targetObjects)
				{
					MonoBehaviour go = obj as MonoBehaviour;
					if (go != null)
					{
						CallMethod(go, buttonAttribute.buttonFunction);
					}
					else
					{
						ScriptableObject so = obj as ScriptableObject;
						if(so != null)
						{
							CallMethod(so, buttonAttribute.buttonFunction);
						}
					}
				}
			}
				
			if (showVariable)
				position.y += position.height;
				
			if(showVariable)
			{
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
				base.OnGUI(position, prop, label);
	#else
				EditorGUI.PropertyField(position, prop);
	#endif			
			}
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			else
			{
				if(!buttonAttribute.hideVariable)
				{
					UnsupportedVariableWarning("Button");
				}
			}
	#endif	

			EditorGUI.EndProperty();
		}
	}

}

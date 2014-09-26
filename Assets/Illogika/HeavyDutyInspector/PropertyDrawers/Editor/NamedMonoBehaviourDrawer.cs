//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(NamedMonoBehaviour))]
	public class NamedMonoBehaviourDrawer : IllogikaDrawer {

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		// Change this in derived classes in the OnGUI function before calling this class' OnGUI function.
		// This is only required if you use a version of Unity before 4.3
		protected System.Type type = typeof(NamedMonoBehaviour);
	#endif	

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2

			if(property.hasChildren)
				EditorGUI.indentLevel -= 1;

			OnNamedMonoBehaviourGUI(position, property, label, type, 0);
	#else
			OnNamedMonoBehaviourGUI(position, property, label, fieldInfo.FieldType, 0);
	#endif
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * 2;
		}


	}

}

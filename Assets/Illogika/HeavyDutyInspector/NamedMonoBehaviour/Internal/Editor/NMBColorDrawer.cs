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

	[CustomPropertyDrawer(typeof(NMBColorAttribute))]
	public class NMBColorDrawer : PropertyDrawer {
			
		NMBColorAttribute nmbColorAttribute { get { return ((NMBColorAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return 0;
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			position.y -= base.GetPropertyHeight(prop, label);
	#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
			position.y -= 2;
	#endif
			position.height = base.GetPropertyHeight(prop, label);

			position.x += position.width;
			position.x -= 50;

			position.width = 50;

			if(prop.hasMultipleDifferentValues)
			{
				EditorGUI.BeginChangeCheck();

				Color temp = EditorGUI.ColorField(position, Color.white);
				Color tempGuiColor = GUI.color;
				GUI.color = Color.grey;
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
				position.x += 13;
	#endif
				GUI.Label(position, "-");
				GUI.color = tempGuiColor;

				if(EditorGUI.EndChangeCheck())
					prop.colorValue = temp;
			}
			else
			{
				prop.colorValue = EditorGUI.ColorField(position, prop.colorValue);
			}

			EditorGUI.EndProperty();
		}
	}

}

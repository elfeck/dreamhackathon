//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(BackgroundAttribute))]
	public class BackgroundDrawer : DecoratorDrawer {
			
		BackgroundAttribute backgroundAttribute { get { return ((BackgroundAttribute)attribute); } }
		
		public override float GetHeight ()
		{
			return 0;
	    }
		
		public override void OnGUI (Rect position)
		{
			int originalIndentLevel = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			position.x = 0;
			position.width = Screen.width;
			position.height = base.GetHeight();

			Color temp = GUI.color;

			GUI.color = backgroundAttribute.color;
			EditorGUI.HelpBox(position, "", MessageType.None);

			GUI.color = temp;

			EditorGUI.indentLevel = originalIndentLevel;
		}
	}

}
#endif

//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2014  Illogika
//----------------------------------------------
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ChangeCheckCallbackAttribute))]
	public class ChangeCheckCallbackDrawer : IllogikaDrawer {
			
		ChangeCheckCallbackAttribute changeCheckCallbackAttribute { get { return ((ChangeCheckCallbackAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	       return base.GetPropertyHeight(prop, label);
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			EditorGUI.BeginChangeCheck();
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			base.OnGUI(position, prop, label);
	#else
			EditorGUI.PropertyField(position, prop);
	#endif

			if(EditorGUI.EndChangeCheck())
			{
				try{
					(prop.serializedObject.targetObject as MonoBehaviour).StartCoroutine(WaitForCallback(prop));
				}
				catch{
					Debug.LogError("ChangeCheckCallback can only work with MonoBehaviours");
				}
			}

			EditorGUI.EndProperty();
		}

		IEnumerator WaitForCallback(SerializedProperty prop)
		{
			yield return null;
			foreach(Object obj in prop.serializedObject.targetObjects)
			{
				MonoBehaviour go = obj as MonoBehaviour;
				if (go != null)
				{
					CallMethod(go, changeCheckCallbackAttribute.callback);
				}
			}
		}
	}

}

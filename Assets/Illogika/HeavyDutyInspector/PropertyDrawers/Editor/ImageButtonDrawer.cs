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

	[CustomPropertyDrawer(typeof(ImageButtonAttribute))]
	public class ImageButtonDrawer : IllogikaDrawer {

		private Texture image;
		private GUIStyle imageStyle;

		ImageButtonAttribute imageButtonAttribute { get { return ((ImageButtonAttribute)attribute); } }

		bool ShowVariable(SerializedProperty prop)
		{
			bool showVariable = !imageButtonAttribute.hideVariable;
			
			#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			showVariable &= CanDisplayVariable(prop);
			#endif
			
			return showVariable;
		}

		private float imageHeight
		{
			get{
				if(image == null)
				{
					image = (Texture)AssetDatabase.LoadAssetAtPath("Assets/" + imageButtonAttribute.imagePath, typeof(Texture));
					imageStyle = new GUIStyle();
					imageStyle.normal.background = (Texture2D)image;
				}
				
				return image != null ? image.height : 0.0f;
			}
		}

		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			float baseHeight = base.GetPropertyHeight(prop, label);
			return ShowVariable(prop) ? baseHeight + imageHeight: imageHeight;
		}
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);
			
			bool showVariable = ShowVariable(prop);

			Rect indentPosition = EditorGUI.IndentedRect(position);
			indentPosition.height = image.height;

			if(GUI.Button(indentPosition, ""))
			{
				foreach(Object obj in prop.serializedObject.targetObjects)
				{
					MonoBehaviour go = obj as MonoBehaviour;
					if (go != null)
					{
						CallMethod(go, imageButtonAttribute.buttonFunction);
					}
				}
			}

			indentPosition.x = indentPosition.x + ( indentPosition.width / 2 - image.width / 2);
			indentPosition.width = image.width;

			indentPosition.y = indentPosition.y + ( indentPosition.height / 2 - image.height / 2);
			indentPosition.height = image.height;

			EditorGUI.LabelField(indentPosition, GUIContent.none, imageStyle);

			if (showVariable)
				position.y += indentPosition.height;
			
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
				if(!imageButtonAttribute.hideVariable)
				{
					UnsupportedVariableWarning("Button");
				}
			}
			#endif	
			
			EditorGUI.EndProperty();
		}
	}

}

//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;
using HeavyDutyInspector;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ImageAttribute))]
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
	public class ImageDrawer : IllogikaDrawer {
#else
	public class ImageDrawer : DecoratorDrawer {
#endif
		private Texture image;
		private GUIStyle imageStyle;
		
		
		ImageAttribute imageAttribute { get { return ((ImageAttribute)attribute); } }

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		bool ShowVariable(SerializedProperty prop)
		{
			bool showVariable = !imageAttribute.hideVariable;
			
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			showVariable &= CanDisplayVariable(prop);
	#endif
			
			return showVariable;
		}
#endif
		private float imageHeight
		{
			get{
				if(image == null)
				{
					image = (Texture)AssetDatabase.LoadAssetAtPath("Assets/" + imageAttribute.imagePath, typeof(Texture));
					imageStyle = new GUIStyle();
					imageStyle.normal.background = (Texture2D)image;
				}
				
				return image != null ? image.height : 0.0f;
			}
		}

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			return image != null ? imageHeight + 10.0f + (ShowVariable(prop) ? base.GetPropertyHeight(prop, label) : 0.0f) : 0.0f;
	#else
			return image != null ? imageHeight + 10.0f + (ShowVariable(prop) ? base.GetPropertyHeight(prop, label) : 0.0f) : 0.0f;
	#endif
	    }
#else
		public override float GetHeight ()
		{
			return image != null ? imageHeight + 10.0f : 0.0f;
		}
#endif
		
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
#else
		public override void OnGUI (Rect position)
#endif
		{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			EditorGUI.BeginProperty(position, label, prop);

			Rect basePosition = position;

			basePosition.y += imageHeight + 9.0f;
			
			if(ShowVariable(prop))
			{
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
				base.OnGUI(basePosition, prop, label);
	#else
				EditorGUI.PropertyField(basePosition, prop);
	#endif
			}
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			else
			{
				if(!imageAttribute.hideVariable)
				{
					UnsupportedVariableWarning("Image");
				}
			}
	#endif
#endif
			int baseIndentLevel = EditorGUI.indentLevel;

			EditorGUI.indentLevel = 0;
			position = EditorGUI.IndentedRect(position);

			position.y += 5;
			position.height = imageHeight;

			if(image == null)
				return;

			switch(imageAttribute.alignment)
			{
			case Alignment.Left:
				// Left. Do nothing.
				break;
			case Alignment.Center:
				position.x = position.x + (position.width - image.width) / 2;
				break;
			case Alignment.Right:
				position.x = position.x + position.width - image.width;
				break;
			default:
				break;
			}

			position.width = image.width;
	    	EditorGUI.LabelField(position, GUIContent.none, imageStyle);

			EditorGUI.indentLevel = baseIndentLevel;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			EditorGUI.EndProperty();
#endif
		}
	}

}

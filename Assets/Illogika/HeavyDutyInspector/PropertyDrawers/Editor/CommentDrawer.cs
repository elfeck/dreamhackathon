//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace HeavyDutyInspector
{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
	[CustomPropertyDrawer(typeof(CommentAttribute))]
	public class CommentDrawer : IllogikaDrawer {
#else
	[CustomPropertyDrawer(typeof(CommentAttribute))]
	public class CommentDrawer : DecoratorDrawer {
#endif
		CommentAttribute commentAttribute { get { return ((CommentAttribute)attribute); } }

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		bool ShowVariable(SerializedProperty prop)
		{
			bool showVariable = !commentAttribute.hideVariable;
			
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			showVariable &= CanDisplayVariable(prop);
	#endif
			
			return showVariable;
		}
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
#if UNITY_4_3
            return ShowVariable(prop) ? GetCommentHeight() + base.GetPropertyHeight(prop, label): GetCommentHeight(); // Commit fix
#else
            return ShowVariable(prop) ? GetCommentHeight(prop, label) + base.GetPropertyHeight(prop, label): GetCommentHeight(prop, label);
#endif
        }
#else
		public override float GetHeight()
		{
			return GetCommentHeight();
		}
#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		public float GetCommentHeight(SerializedProperty prop, GUIContent label)
#else
		public float GetCommentHeight()
#endif
		{
			GUIStyle style = "HelpBox";
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			return Mathf.Max(style.CalcHeight(new GUIContent(commentAttribute.comment), Screen.width - (commentAttribute.messageType != MessageType.None ? 53 : 21) ), base.GetPropertyHeight(prop, label) * 1.5f);
	#else
			return Mathf.Max(style.CalcHeight(new GUIContent(commentAttribute.comment), Screen.width - (commentAttribute.messageType != MessageType.None ? 53 : 21) ), 40);
	#endif
		}
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
#else
		public override void OnGUI (Rect position)
#endif
		{
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			EditorGUI.BeginProperty(position, label, prop);
#endif
			Rect commentPosition = EditorGUI.IndentedRect (position);

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			commentPosition.height = GetCommentHeight(prop, label);
#else
			commentPosition.height = GetCommentHeight();
#endif
			
			DrawComment(commentPosition, commentAttribute.comment);
			
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			position.y += commentPosition.height;
			position.height -= commentPosition.height;

			if(ShowVariable(prop))
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
				if(!commentAttribute.hideVariable)
				{
					UnsupportedVariableWarning("Comment");
				}
			}
	#endif
			EditorGUI.EndProperty();
#endif
		}
		
		private void DrawComment(Rect position, string comment)
		{
			EditorGUI.HelpBox(position, comment, commentAttribute.messageType);
		}
	}

}

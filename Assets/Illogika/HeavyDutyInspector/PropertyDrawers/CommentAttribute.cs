//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace HeavyDutyInspector
{

	public enum CommentType { Error,
							  Info,
							  None,
							  Warning }

#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
	[AttributeUsage (AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
#endif
	public class CommentAttribute : PropertyAttribute {
		
		public string comment
		{
			get;
			private set;
		}
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		public bool hideVariable
		{
			get;
			private set;
		}
#endif
	#if UNITY_EDITOR	
		public MessageType messageType
		{
			get;
			private set;
		}
	#endif

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		/// <summary>
		/// Adds a comment before this variable. In versions of Unity older than 4.3, the variable can only be displayed if it is of type bool, int, float, string, Color, Object reference, Rect, Vector2 or Vector3. Other variable types will have the variable hidden by default. In Unity 4.3 or higher, variables of any type can be displayed.
		/// </summary>
		/// <param name='comment'>
		/// The comment to display.
		/// </param>
		/// <param name='messageType'>
		/// The icon to be displayed next to the comment, if any.
		/// </param>
		public CommentAttribute(string comment, CommentType messageType = CommentType.None, bool hideVariable = false)
#else
		/// <summary>
		/// Adds a comment before this variable.
		/// </summary>
		/// <param name='comment'>
		/// The comment to display.
		/// </param>
		/// <param name='messageType'>
		/// The icon to be displayed next to the comment, if any.
		/// </param>
		public CommentAttribute(string comment, CommentType messageType = CommentType.None, int order = 0)
#endif
		{
			this.comment = comment;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			this.hideVariable = hideVariable;
#else
			this.order = order;
#endif
	#if UNITY_EDITOR
			switch(messageType)
			{
			case CommentType.Error:
				this.messageType = MessageType.Error;
				break;
			case CommentType.Info:
				this.messageType = MessageType.Info;
				break;
			case CommentType.None:
				this.messageType = MessageType.None;
				break;
			case CommentType.Warning:
				this.messageType = MessageType.Warning;
				break;
			default:
				break;
			}
	#endif
		}
	}

}

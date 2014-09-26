//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------
using System;
using UnityEngine;
using System.Collections;

namespace HeavyDutyInspector
{

	public enum Alignment {
		Left,
		Center,
		Right
	}

#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
	[AttributeUsage (AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
#endif
	public class ImageAttribute : PropertyAttribute {
		
		public string imagePath
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
		public Alignment alignment
		{
			get;
			private set;
		}

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
		/// <summary>
		/// Adds the specified image in the inspector before the variable. In versions of Unity older than 4.3, the variable can only be displayed if it is of type bool, int, float, string, Color, Object reference, Rect, Vector2 or Vector3. Other variable types will have the variable hidden by default. In Unity 4.3 or higher, variables of any type can be displayed.
		/// </summary>
		/// <param name="imagePath">Path to the image. The path is relative to the project's Asset folder.</param>
		/// <param name="alignment">The image's alignment, either Left, Center or Right.</param>
		/// <param name="hideVariable">Do you want to hide the variable, or display the variable in addition to the image.</param>
		public ImageAttribute(string imagePath, Alignment alignment = Alignment.Center, bool hideVariable = false)
#else
		/// <summary>
		/// Adds the specified image in the inspector before the variable.
		/// </summary>
		/// <param name="imagePath">Path to the image. The path is relative to the project's Asset folder.</param>
		/// <param name="alignment">The image's alignment, either Left, Center or Right.</param>
		public ImageAttribute(string imagePath, Alignment alignment = Alignment.Center, int order = 0)
#endif
		{
			if(imagePath.ToLower().Substring(0, 7).Equals("assets/"))
				imagePath = imagePath.Substring(7);

			this.imagePath = imagePath;

			this.alignment = alignment;
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3
			this.hideVariable = hideVariable;
#else
			this.order = order;
#endif
		}
	}
}

	
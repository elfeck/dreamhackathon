﻿//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2 && !UNITY_4_3
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{
	public enum Style {
		Box,
		Line,
	}

	public class ComplexHeaderAttribute : PropertyAttribute {

		public string text
		{
			get;
			private set;
		}

		public Style style
		{
			get;
			private set;
		}

		public Alignment textAlignment
		{
			get;
			private set;
		}

		public Color textColor
		{
			get;
			private set;
		}

		public Color backgroundColor
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates a header before the variable it is applied to.
		/// </summary>
		/// <param name="text">Header text</param>
		/// <param name="style">Header style. Either against a solid color background or surrounded by lines.</param>
		/// <param name="textAlignment">Text alignment. Left, Right or Centered</param>
		/// <param name="textColor">Text color.</param>
		/// <param name="backgroundColor">Background color.</param>
		public ComplexHeaderAttribute(string text, Style style, Alignment textAlignment, ColorEnum textColor, ColorEnum backgroundColor)
		{
			this.text = text;
			this.style = style;
			this.textAlignment = textAlignment;
			this.textColor = ColorEx.GetColorByEnum(textColor);
			this.backgroundColor = ColorEx.GetColorByEnum(backgroundColor);

			// Always display first to make sure it is diplayed at the top, before any other DecoratorDrawer
			order = -int.MaxValue;
		}

		/// <summary>
		/// Creates a header before the variable it is applied to.
		/// </summary>
		/// <param name="text">Header text</param>
		/// <param name="style">Header style. Either against a solid color background or surrounded by lines.</param>
		/// <param name="textAlignment">Text alignment. Left, Right or Centered</param>
		/// <param name="textColorR">Text color red component.</param>
		/// <param name="textColorG">Text color green component.</param>
		/// <param name="textColorB">Text color blue component.</param>
		/// <param name="backgroundColorR">Background color red component.</param>
		/// <param name="backgroundColorG">Background color green component.</param>
		/// <param name="backgroundColorB">Background color blue component.</param>
		public ComplexHeaderAttribute(string text, Style style, Alignment textAlignment, float textColorR, float textColorG, float textColorB, float backgroundColorR, float backgroundColorG, float backgroundColorB)
		{
			this.text = text;
			this.style = style;
			this.textAlignment = textAlignment;
			this.textColor = new Color(textColorR, textColorG, textColorB);
			this.backgroundColor = new Color(backgroundColorR, backgroundColorG, backgroundColorB);

			// Always display first to make sure it is diplayed at the top, before any other DecoratorDrawer
			order = -int.MaxValue;
		}
	}
	
}
#endif
	
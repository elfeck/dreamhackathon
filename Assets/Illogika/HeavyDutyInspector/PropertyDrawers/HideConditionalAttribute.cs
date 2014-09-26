//----------------------------------------------
//            Heavy-Duty Inspector
//         Copyright © 2014  Illogika
//----------------------------------------------
#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	public class HideConditionalAttribute : PropertyAttribute {

		public enum ConditionType
		{
			IsNotNull,
			Bool,
			IntOrEnum,
			FloatRange
		}

		public ConditionType conditionType
		{
			get;
			private set;
		}

		public string variableName
		{
			get;
			private set;
		}

		public bool boolValue
		{
			get;
			private set;
		}

		public List<int> enumValues
		{
			get;
			private set;
		}

		public float minValue
		{
			get;
			private set;
		}

		public float maxValue
		{
			get;
			private set;
		}

		public bool isNotNull
		{
			get;
			private set;
		}

		/// <summary>
		/// Hides this variable until the value of another variable is not null.
		/// </summary>
		/// <param name="conditionVariableName">The name of the variable whose value will be evaluated.</param>
		public HideConditionalAttribute(string conditionVariableName)
		{
			conditionType = ConditionType.IsNotNull;
			variableName = conditionVariableName;
		}

		/// <summary>
		/// Hides this variable until a condition is met.
		/// </summary>
		/// <param name="conditionVariableName">The name of the variable whose value will be evaluated.</param>
		/// <param name="visibleState">The state the condition variable has to be in for this variable to be shown in the Inspector.</param>
		public HideConditionalAttribute(string conditionVariableName, bool visibleState)
		{
			conditionType = ConditionType.Bool;
			variableName = conditionVariableName;
			boolValue = visibleState;
		}

		/// <summary>
		/// Hides this variable until a condition is met.
		/// </summary>
		/// <param name="conditionVariableName">The name of the variable whose value will be evaluated. Can be an int or an enum.</param>
		/// <param name="visibleStates">The states the condition variable can be in for this variable to be shown in the Inspector.</param>
		public HideConditionalAttribute(string conditionVariableName, params int[] visibleState)
		{
			conditionType = ConditionType.IntOrEnum;
			variableName = conditionVariableName;
			enumValues = new List<int>();
			enumValues = visibleState.ToList();
		}

		/// <summary>
		/// Hides this variable until a condition is met.
		/// </summary>
		/// <param name="conditionVariableName">The name of the variable whose value will be evaluated.</param>
		/// <param name="minValue">The minimum value the condition variable can contain for this variable to be shown in the Inspector. Inclusive.</param>
		/// <param name="maxValue">The maximum value this variable can contain for this variable to be shown in the Inspector. Inclusive.</param>
		public HideConditionalAttribute(string conditionVariableName, float minValue, float maxValue)
		{
			conditionType = ConditionType.FloatRange;
			variableName = conditionVariableName;
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}

}
#endif

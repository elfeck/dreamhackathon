//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	public class IllogikaDrawer : PropertyDrawer {

		private bool doOnlyOnce;
		
	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		protected bool CanDisplayVariable(SerializedProperty prop)
		{
			return 	prop.propertyType == SerializedPropertyType.Boolean ||
				prop.propertyType == SerializedPropertyType.Color ||
					prop.propertyType == SerializedPropertyType.Float ||
					prop.propertyType == SerializedPropertyType.Integer ||
					prop.propertyType == SerializedPropertyType.ObjectReference ||
					prop.propertyType == SerializedPropertyType.Rect ||
					prop.propertyType == SerializedPropertyType.String ||
					prop.propertyType == SerializedPropertyType.Vector2 ||
					prop.propertyType == SerializedPropertyType.Vector3;
		}

		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			switch(prop.propertyType)
			{
			case SerializedPropertyType.Rect:
				return base.GetPropertyHeight(prop, label) * 3;
			case SerializedPropertyType.Vector2:
				return base.GetPropertyHeight(prop, label) * 2;
			case SerializedPropertyType.Vector3:
				return base.GetPropertyHeight(prop, label) * 2;
			default:
				return base.GetPropertyHeight(prop, label);
			}
		}

		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{			
			switch(prop.propertyType)
			{
			case SerializedPropertyType.Boolean:
				prop.boolValue = EditorGUI.Toggle(position, label, prop.boolValue);
				break;
			case SerializedPropertyType.Color:
				prop.colorValue = EditorGUI.ColorField(position, label, prop.colorValue);
				break;
			case SerializedPropertyType.Float:
				prop.floatValue = EditorGUI.FloatField(position, label, prop.floatValue);
				break;
			case SerializedPropertyType.Integer:
				prop.intValue = EditorGUI.IntField(position, label, prop.intValue);
				break;
			case SerializedPropertyType.ObjectReference:
				System.Type type = typeof(UnityEngine.Object);
				if(prop.objectReferenceValue != null)
					type = prop.objectReferenceValue.GetType();
				prop.objectReferenceValue = EditorGUI.ObjectField(position, label, prop.objectReferenceValue, type, true);
				break;
			case SerializedPropertyType.Rect:
				prop.rectValue = EditorGUI.RectField(position, label, prop.rectValue);
				break;
			case SerializedPropertyType.String:
				prop.stringValue = EditorGUI.TextField(position, label, prop.stringValue);
				break;
			case SerializedPropertyType.Vector2:
				prop.vector2Value = EditorGUI.Vector2Field(position, label.text, prop.vector2Value);
				break;
			case SerializedPropertyType.Vector3:
				prop.vector3Value = EditorGUI.Vector3Field(position, label.text, prop.vector3Value);
				break;
			default:
				break;
			}
			
		}
		
		protected void UnsupportedVariableWarning(string attributeName)
		{
			Debug.LogError(string.Format("You have used the {0} Attribute with a variable type that is not supported. Your variable will be hidden by default in the Inspector. To remove this warning, explicitly hide the variable in the Attribute's constructor. Refer to the documentation for a list of supported types, or upgrade to Unity 4.3, for which all variable types are supported.", attributeName));
			doOnlyOnce = true;
		}
	#else
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			if(prop.propertyType == SerializedPropertyType.Rect)
			{
				return base.GetPropertyHeight(prop, label) * 2;
			}
			
			return base.GetPropertyHeight(prop, label);
		}
		
	#endif

		protected void WrongVariableTypeWarning(string attributeName, string variableType)
		{
			if(!doOnlyOnce)
			{
				Debug.LogError(string.Format("The {0}Attribute is designed to be applied to {1} only!", attributeName, variableType));
				doOnlyOnce = true;
			}
		}

		private Dictionary<string, GameObject> targetObjects = new Dictionary<string, GameObject>();

	#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
		protected void OnComponentGUI (Rect position, SerializedProperty prop, GUIContent label, System.Type componentType, string fieldName, string[] requiredValues, string defaultObject, bool isPrefab, int rightOffset)
	#else
		protected void OnComponentGUI (Rect position, SerializedProperty prop, GUIContent label, string fieldName, string[] requiredValues, string defaultObject, bool isPrefab, int rightOffset)
	#endif
		{
			if(prop.propertyType != SerializedPropertyType.ObjectReference)
			{
				WrongVariableTypeWarning("ComponentSelection", "object references");
				return;
			}
			
			int originalIndentLevel = EditorGUI.indentLevel;

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID (FocusType.Passive), label);
			EditorGUI.indentLevel = 0;
			
			if(prop.hasMultipleDifferentValues)
			{
				position.height = GetPropertyHeight(prop, label);
				
				#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
				if(fieldInfo.FieldType.IsArray || fieldInfo.FieldType.IsGenericType)
					position.x += 15;
				#endif
				EditorGUI.HelpBox(position, "Multi object editing is not supported for references with different values.", MessageType.Warning);
				return;
			}
			
			if(!targetObjects.ContainsKey(prop.propertyPath))
				targetObjects.Add(prop.propertyPath, null);
			
			// Set back the target game object if the drawed object has been deselected.
			if(prop.objectReferenceValue != null)
				targetObjects[prop.propertyPath] = (prop.objectReferenceValue as Component).gameObject;

			try{
				if(targetObjects[prop.propertyPath] == null)
					targetObjects[prop.propertyPath] = string.IsNullOrEmpty(defaultObject) ? (prop.serializedObject.targetObject as MonoBehaviour).gameObject : isPrefab ? Resources.Load(defaultObject) as GameObject : GameObject.Find(defaultObject);
			}
			catch{
				if(targetObjects[prop.propertyPath] == null)
					targetObjects[prop.propertyPath] = string.IsNullOrEmpty(defaultObject) ? null : isPrefab ? Resources.Load(defaultObject) as GameObject : null;
			}

			if((prop.serializedObject.targetObject as ScriptableObject) != null)
			{
				if(targetObjects[prop.propertyPath] != null && !AssetDatabase.Contains(targetObjects[prop.propertyPath]))
				   targetObjects[prop.propertyPath] = null;
			}

			position.height /= 2;
			position.width -= rightOffset;
			
			EditorGUI.BeginChangeCheck();
			
			Color tempColor = GUI.color;
			try{
				GUI.color = targetObjects[prop.propertyPath] == (prop.serializedObject.targetObject as MonoBehaviour).gameObject ? Color.green : Color.yellow;
			}
			catch{
				GUI.color = Color.yellow;
			}
			
			targetObjects[prop.propertyPath] = EditorGUI.ObjectField(position, targetObjects[prop.propertyPath], typeof(GameObject), true) as GameObject;
			
			GUI.color = tempColor;
			
			if(targetObjects[prop.propertyPath] == null)
				return;

			position.width += rightOffset;

			#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			List<Component> components = targetObjects[prop.propertyPath].GetComponents(componentType).ToList();
			#else
			System.Type componentType;
			List<Component> components;
			if(fieldInfo.FieldType.IsArray)
				componentType = fieldInfo.FieldType.GetElementType();
			else if(fieldInfo.FieldType.IsGenericType)
				componentType = fieldInfo.FieldType.GetGenericArguments()[0];
			else
				componentType = fieldInfo.FieldType;
			
			components = targetObjects[prop.propertyPath].GetComponents(componentType).ToList();
			#endif

			if(EditorGUI.EndChangeCheck())
			{
#if UNITY_4_0 
				Undo.RegisterUndo(prop.serializedObject.targetObjects, "Change Target Object");
#else
				Undo.RecordObjects(prop.serializedObject.targetObjects, "Change Target Object");
#endif
				prop.objectReferenceValue = targetObjects[prop.propertyPath].GetComponent(componentType);

				foreach(Object obj in prop.serializedObject.targetObjects)
				{
					EditorUtility.SetDirty(obj);
				}
			}

			if(components.Contains(prop.serializedObject.targetObject as Component))
				components.Remove(prop.serializedObject.targetObject as Component);
			
			List<string> componentsNames = new List<string>();
			Dictionary<System.Type, int> componentsNumbers = new Dictionary<System.Type, int>();
			Dictionary<string, int> namedMonoBehavioursNumbers = new Dictionary<string, int>();

			foreach(Component component in components)
			{
				if(!componentsNumbers.ContainsKey(component.GetType()))
				{
					componentsNumbers.Add(component.GetType(), 1);
				}
				
				if(component is NamedMonoBehaviour)
				{ 
					if(string.IsNullOrEmpty((component as NamedMonoBehaviour).scriptName))
					{
						if(string.IsNullOrEmpty(fieldName))
						{
							componentsNames.Add(component.GetType().ToString().Replace("UnityEngine.", "") + " " + componentsNumbers[component.GetType()]++.ToString());
						}
						else
						{
							System.Object val = GetFieldOrPropertyValue(component, fieldName);

							if(requiredValues != null && requiredValues.Length > 0)
							{
								if(requiredValues.Contains(val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")))
									componentsNames.Add(component.GetType().ToString().Replace("UnityEngine.", "") + " " + componentsNumbers[component.GetType()]++.ToString() + " (" + (val == null ? "null" : val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")) + ")");
							}
							else
							{
								componentsNames.Add(component.GetType().ToString().Replace("UnityEngine.", "") + " " + componentsNumbers[component.GetType()]++.ToString() + " (" + (val == null ? "null" : val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")) + ")");
							}
						}
					}
					else
					{
						if(namedMonoBehavioursNumbers.ContainsKey((component as NamedMonoBehaviour).scriptName))
						{
							(component as NamedMonoBehaviour).scriptName += (" " + namedMonoBehavioursNumbers[(component as NamedMonoBehaviour).scriptName]++);
						}
						else
						{
							namedMonoBehavioursNumbers.Add((component as NamedMonoBehaviour).scriptName, 2);
						}

						if(string.IsNullOrEmpty(fieldName))
						{
							componentsNames.Add((component as NamedMonoBehaviour).scriptName);
						}
						else
						{
							System.Object val = GetFieldOrPropertyValue(component, fieldName);
							if(requiredValues != null && requiredValues.Length > 0)
							{
								if(requiredValues.Contains(val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")))
									componentsNames.Add((component as NamedMonoBehaviour).scriptName + " (" + (val == null ? "null" : val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")) + ")");
							}
							else
							{
								componentsNames.Add((component as NamedMonoBehaviour).scriptName + " (" + (val == null ? "null" : val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")) + ")");
							}
						}
						
					}
				}
				else
				{
					if(string.IsNullOrEmpty(fieldName))
					{
						componentsNames.Add(component.GetType().ToString().Replace("UnityEngine.", "") + " " + componentsNumbers[component.GetType()]++.ToString());
					}
					else
					{
						System.Object val = GetFieldOrPropertyValue(component, fieldName);
						if(requiredValues != null && requiredValues.Length > 0)
						{
							if(requiredValues.Contains(val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")))
								componentsNames.Add(component.GetType().ToString().Replace("UnityEngine.", "") + " " + componentsNumbers[component.GetType()]++.ToString() + " (" + (val == null ? "null" : val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")) + ")");
						}
						else
						{
							componentsNames.Add(component.GetType().ToString().Replace("UnityEngine.", "") + " " + componentsNumbers[component.GetType()]++.ToString() + " (" + (val == null ? "null" : val.ToString().Replace(" (" + val.GetType().ToString() + ")", "")) + ")");
						}
					}
				}
			}
			
			components.Insert(0, null);
			#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			componentsNames.Insert(0, "None (" + componentType.ToString().Replace("UnityEngine.", "") + ")");
			#else
			componentsNames.Insert(0, "None (" + componentType.ToString().Replace("UnityEngine.", "") + ")");
			#endif
			
			position.y += position.height;
			
			int index = 0;
			
			if(prop.objectReferenceValue != null)
			{
				try
				{
					index = components.IndexOf(prop.objectReferenceValue as Component);
				}
				catch
				{
					prop.objectReferenceValue = null;
				}
			}
			
			prop.objectReferenceValue = components[EditorGUI.Popup(position, index, componentsNames.ToArray())];
			
			EditorGUI.indentLevel = originalIndentLevel;
		}
		
		protected System.Object GetFieldOrPropertyValue(Component component, string fieldName)
		{
			if(component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) != null)
				return component.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(component);
			else if(component.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) != null)
				return component.GetType().GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(component, null);
			else
				Debug.LogError(string.Format("{0} does not contain a field or property named {1}!", component, fieldName));

			return "";
		}

		protected virtual void OnNamedMonoBehaviourGUI(Rect position, SerializedProperty property, GUIContent label, System.Type scriptType, int rightOffset)
		{
			position.height /= 2;
			
			int originalIndentLevel = EditorGUI.indentLevel;

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID (FocusType.Passive), label);
			EditorGUI.indentLevel = 0;

			position.width -= rightOffset;

			if(property.hasMultipleDifferentValues)
			{
				EditorGUI.BeginChangeCheck();
				
				Object temp = EditorGUI.ObjectField(position, Resources.Load("-"), scriptType, true);

				if(EditorGUI.EndChangeCheck())
					property.objectReferenceValue = temp;

				position.width += rightOffset;

				position.y += position.height;
				Color color = GUI.color;
				GUI.color = new Color(1, 0.5f, 0);
				EditorGUI.LabelField(position, "[Multiple Values]");
				GUI.color = color;
			}
			else
			{
				property.objectReferenceValue = EditorGUI.ObjectField(position,(NamedMonoBehaviour)property.objectReferenceValue, scriptType, true);

				position.width += rightOffset;

				position.y += position.height;
				
				if(property.objectReferenceValue != null)
				{
					NamedMonoBehaviour monoBehaviour = (NamedMonoBehaviour)property.objectReferenceValue;
					Color color = GUI.color;
					GUI.color = monoBehaviour.scriptNameColor;
					EditorGUI.LabelField(position, string.Format("[{0}]", !string.IsNullOrEmpty(monoBehaviour.scriptName) ? monoBehaviour.scriptName : monoBehaviour.GetType().ToString() ) );
					GUI.color = color;
				}
			}
			
			EditorGUI.indentLevel = originalIndentLevel;
		}

		public static void CallMethod(MonoBehaviour go, string methodName)
		{
			MethodInfo buttonFunction = GetMethodRecursively(go.GetType(), methodName);
			if (buttonFunction == null)
			{
				Debug.LogError(string.Format("Function {0} not found in class {1} or any of its base classes.", methodName, go.GetType().ToString()));
			}
			else
			{
				buttonFunction.Invoke(go, null);
			}
		}

		public static void CallMethod(ScriptableObject so, string methodName)
		{
			MethodInfo buttonFunction = GetMethodRecursively(so.GetType(), methodName);
			if (buttonFunction == null)
			{
				Debug.LogError(string.Format("Function {0} not found in class {1} or any of its base classes.", methodName, so.GetType().ToString()));
			}
			else
			{
				buttonFunction.Invoke(so, null);
			}
		}

		static MethodInfo GetMethodRecursively(System.Type type, string methodName)
		{
			MethodInfo buttonFunction = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			if (buttonFunction == null)
			{
				if(type.BaseType != null)
					return GetMethodRecursively(type.BaseType, methodName);
				else
					return null;
			}
			return buttonFunction;
		}
	}

}

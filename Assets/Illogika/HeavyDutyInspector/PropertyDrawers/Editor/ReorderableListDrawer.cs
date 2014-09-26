//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

#if !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(ReorderableListAttribute))]
	public class ReorderableListDrawer : IllogikaDrawer {

		ReorderableListAttribute reorderableListAttribute { get { return ((ReorderableListAttribute)attribute); } }

		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			if(prop.serializedObject.targetObjects.Length > 1)
			{
				if(int.Parse(prop.propertyPath.Split('[')[1].Split(']')[0]) != 0)
					return -2.0f;
				else
					return base.GetPropertyHeight(prop, label) * 2;
			}

			if(prop.hasChildren && prop.isExpanded)
			{
				int i = 1;
				foreach(SerializedProperty childProp in prop)
				{
					if(reorderableListAttribute.doubleComponentRefSizeInChildren && childProp.type.Contains("<$"))
					{
						foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
						{
							System.Type cachedType = assembly.GetType(childProp.type.Split('$')[1].Split('>')[0]);
							if(cachedType == null)
								cachedType = assembly.GetType("UnityEngine." + childProp.type.Split('$')[1].Split('>')[0]);
							if(cachedType != null && (cachedType.IsSubclassOf(typeof(Component)) || cachedType == typeof(Component) || cachedType.IsSubclassOf(typeof(NamedMonoBehaviour)) || cachedType == typeof(NamedMonoBehaviour)))
								++i;
						}
					}
					++i;
				}
				return (base.GetPropertyHeight(prop, label) + 1)* i + 2;
			}
			else if(fieldInfo.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(Component)))
			{
				return (base.GetPropertyHeight(prop, label) + 1) * 2;
			}
			else
			{
				return base.GetPropertyHeight(prop, label);
			}
		}
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			int index = int.Parse(prop.propertyPath.Split('[')[1].Split(']')[0]);

			if(!fieldInfo.FieldType.IsGenericType || fieldInfo.FieldType.IsArray)
			{
				Debug.LogWarning("The Reorderable List Attribute can only be used with Lists.");
			}

			IList list = null;
			try
			{
				list = (prop.serializedObject.targetObject as MonoBehaviour).GetType().GetField(prop.propertyPath.Split('.')[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(prop.serializedObject.targetObject) as IList;
			}
			catch
			{
				try{
					list = (prop.serializedObject.targetObject as ScriptableObject).GetType().GetField(prop.propertyPath.Split('.')[0], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(prop.serializedObject.targetObject) as IList;
				}
				catch{
					Debug.LogWarning(string.Format("The script has no property named {0} or {0} is not an IList",prop.propertyPath.Split('.')[0]));
				}
			}

			if(prop.serializedObject.targetObjects.Length > 1)
			{
				if(index == 0)
				{
					position.height = base.GetPropertyHeight(prop, label) * 2;
					EditorGUI.indentLevel = 1;
					position = EditorGUI.IndentedRect(position);
					EditorGUI.HelpBox(position, "Multi object editing is not supported.", MessageType.Warning);
				}
				return;
			}

			Rect basePosition = position;

			position.width -= 84;
			position.height = 16;

			if(prop.hasChildren && prop.isExpanded)
			{
				prop.isExpanded = EditorGUI.PropertyField(position, prop);

				EditorGUI.indentLevel += 1;
				Rect childPosition = basePosition;
				foreach(SerializedProperty childProp in prop)
				{
					childPosition.height = base.GetPropertyHeight(childProp, label);
					childPosition.y += base.GetPropertyHeight(childProp, label) + 2;

					if(reorderableListAttribute.doubleComponentRefSizeInChildren && childProp.type.Contains("<$"))
					{
						bool hasDrawn = false;
						string typeName = childProp.type.Split('$')[1].Split('>')[0];
						foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
						{
							System.Type cachedType = assembly.GetType(typeName);
							if(cachedType == null)
								cachedType = assembly.GetType("UnityEngine." + childProp.type.Split('$')[1].Split('>')[0]);
							if(cachedType != null && (cachedType.IsSubclassOf(typeof(Component)) || cachedType == typeof(Component) || cachedType.IsSubclassOf(typeof(NamedMonoBehaviour)) || cachedType == typeof(NamedMonoBehaviour)))
							{
								childPosition.height *= 2;

								EditorGUI.PropertyField(childPosition, childProp);

								childPosition.y += base.GetPropertyHeight(childProp, label);
								hasDrawn = true;
								break;
							}
						}

						if(!hasDrawn)
							EditorGUI.PropertyField(childPosition, childProp);
					}
					else
					{
						EditorGUI.PropertyField(childPosition, childProp);
					}
				}
			}
			else if (fieldInfo.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(NamedMonoBehaviour)) && reorderableListAttribute.useNamedMonoBehaviourDrawer)
			{
				System.Type type; 
				if(fieldInfo.FieldType.IsArray)
					type = fieldInfo.FieldType.GetElementType();
				else if(fieldInfo.FieldType.IsGenericType)
					type = fieldInfo.FieldType.GetGenericArguments()[0];
				else
					type = fieldInfo.FieldType;

				OnNamedMonoBehaviourGUI(basePosition, prop, label, type, 82);
			}
			else if (fieldInfo.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(Component)))
			{
				OnComponentGUI(basePosition, prop, label, "", null, "", false, 82);
			}
			else
			{
				EditorGUI.PropertyField(position, prop);
			}

			basePosition.x += basePosition.width - 82;
			basePosition.width = 25;
			basePosition.height = 16;

			if(index != 0)
			{
				if(GUI.Button(basePosition, reorderableListAttribute.arrowUp, "ButtonLeft"))
				{
					Undo.RecordObjects(prop.serializedObject.targetObjects, "Move Item Up In List");

					list.Insert(index - 1, list[index]);
					list.RemoveAt(index + 1);

					foreach(Object obj in prop.serializedObject.targetObjects)
					{
						EditorUtility.SetDirty(obj);
					}
				}
			}
			else
			{
				Color temp = GUI.color;
				GUI.color = Color.gray;
				GUI.Box(basePosition, reorderableListAttribute.arrowUp, "ButtonLeft");
				GUI.color = temp;
			}

			basePosition.x += 25;

			if(index != list.Count - 1)
			{
				if(GUI.Button(basePosition, reorderableListAttribute.arrowDown, "ButtonRight") && index != list.Count - 1)
				{
					Undo.RecordObjects(prop.serializedObject.targetObjects, "Move Item Down In List");

					list.Insert(index + 2, list[index]);
					list.RemoveAt(index);

					foreach(Object obj in prop.serializedObject.targetObjects)
					{
						EditorUtility.SetDirty(obj);
					}
				}
			}
			else
			{
				Color temp = GUI.color;
				GUI.color = Color.gray;
				GUI.Box(basePosition, reorderableListAttribute.arrowDown, "ButtonRight");
				GUI.color = temp;
			}

			basePosition.x += 25;
			basePosition.width = 16;

			if(GUI.Button(basePosition, "", "OL Plus"))
			{
				Undo.RecordObjects(prop.serializedObject.targetObjects, "Add Item In List");

				if(fieldInfo.FieldType.GetGenericArguments()[0].IsClass && !fieldInfo.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(MonoBehaviour)) && fieldInfo.FieldType.GetGenericArguments()[0].GetConstructor(new Type[0]) != null)
					list.Insert(index + 1, System.Activator.CreateInstance(fieldInfo.FieldType.GetGenericArguments()[0]));
				else
					list.Insert(index + 1, list[index]);

				foreach(Object obj in prop.serializedObject.targetObjects)
				{
					EditorUtility.SetDirty(obj);
				}
			}

			basePosition.x += 16;

			if(GUI.Button(basePosition, "", "OL Minus"))
			{
				Undo.RecordObjects(prop.serializedObject.targetObjects, "Remove Item In List");

				list.RemoveAt(index);

				foreach(Object obj in prop.serializedObject.targetObjects)
				{
					EditorUtility.SetDirty(obj);
				}
			}

			EditorGUI.EndProperty();
		}
	}

}
#endif
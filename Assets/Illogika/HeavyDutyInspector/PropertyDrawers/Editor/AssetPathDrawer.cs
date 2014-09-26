//----------------------------------------------
//            Heavy-Duty Inspector
//      Copyright © 2013 - 2014  Illogika
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace HeavyDutyInspector
{

	[CustomPropertyDrawer(typeof(AssetPathAttribute))]
	public class AssetPathDrawer : IllogikaDrawer {

		AssetPathAttribute assetPathAttribute { get { return ((AssetPathAttribute)attribute); } }
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			return base.GetPropertyHeight(prop, label) * 2;
	    }
		
		public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, prop);

			if(prop.propertyType != SerializedPropertyType.String)
			{
				WrongVariableTypeWarning("AssetPath", "strings");
				return;
			}

			int originalIndentLevel = EditorGUI.indentLevel;

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID (FocusType.Passive), label);
			EditorGUI.indentLevel = 0;

			position.height /= 2;

			if(prop.hasMultipleDifferentValues)
			{
				EditorGUI.BeginChangeCheck();

				Object temp = EditorGUI.ObjectField(position, Resources.Load("-"), assetPathAttribute.type, false);

				if(EditorGUI.EndChangeCheck())
				{
					assetPathAttribute.obj = temp;
					prop.stringValue = FormatString(AssetDatabase.GetAssetPath(temp));
				}
			}
			else
			{
				EditorGUI.BeginChangeCheck();

				if(assetPathAttribute.obj == null && !string.IsNullOrEmpty(prop.stringValue))
					SelectObject(prop.stringValue);

				assetPathAttribute.obj = EditorGUI.ObjectField(position, assetPathAttribute.obj, assetPathAttribute.type, false);
				string temp = AssetDatabase.GetAssetPath(assetPathAttribute.obj);

				if(EditorGUI.EndChangeCheck())
				{
					prop.stringValue = temp;
					prop.stringValue = FormatString(prop.stringValue);
				}

				position.y += base.GetPropertyHeight(prop, label);

				EditorGUI.SelectableLabel(position, prop.stringValue);
			}

			EditorGUI.indentLevel = originalIndentLevel;
			EditorGUI.EndProperty();
		}

		void SelectObject(string path)
		{
			switch(assetPathAttribute.pathOptions)
			{
			case PathOptions.RelativeToAssets:
				assetPathAttribute.obj = AssetDatabase.LoadAssetAtPath("Assets/" + path, assetPathAttribute.type);
				break;
			case PathOptions.RelativeToResources:
				assetPathAttribute.obj = Resources.Load(path);
				break;
			case PathOptions.FilenameOnly:
				string fullPath = (from p in AssetDatabase.GetAllAssetPaths() where Path.GetFileName(p).Equals(path) select p).FirstOrDefault();
				assetPathAttribute.obj = AssetDatabase.LoadAssetAtPath(fullPath, assetPathAttribute.type);
				break;
			}
		}

		string FormatString(string path)
		{
			switch(assetPathAttribute.pathOptions)
			{
			case PathOptions.RelativeToAssets:
				path = path.Substring(path.IndexOf("Assets/") + 7);
				break;
			case PathOptions.RelativeToResources:
				if(path.Contains("Resources/"))
					path = path.Substring(path.IndexOf("Resources/") + 10).Replace(Path.GetExtension(path), "");
				else
					Debug.LogWarning("Selected asset is not in a Resources folder");
				break;
			case PathOptions.FilenameOnly:
				path = Path.GetFileName(path);
				break;
			}

			return path;
		}
	}

}

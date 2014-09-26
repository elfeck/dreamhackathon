using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Sorts the hierarchy according to SAS conventions:
/// - TODO
/// \authors Julian Mautner
/// </summary>
public class SASHierarchyOrder : BaseHierarchySort
{
	public override int Compare(GameObject lhs, GameObject rhs)
	{
		if(lhs == rhs)
			return 0;
		if(lhs == null)
			return -1;
		if(rhs == null)
			return 1;
		
		return EditorUtility.NaturalCompare(lhs.name, rhs.name);
		//new unity sorting important for UI
//		return lhs.transform.GetSiblingIndex() - rhs.transform.GetSiblingIndex();
	}
}

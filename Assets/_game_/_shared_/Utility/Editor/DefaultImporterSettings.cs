using UnityEngine;
using UnityEditor;
     
public class ImporterPostProcessor : AssetPostprocessor
{
	void OnPreprocessModel()
	{
		var importer = assetImporter as ModelImporter;
		if(importer == null) return;

		//if the meta file already exists, we assume that this is NOT the first import and THUS do NOT
		//change ANY settings.
		string metaFile = importer.assetPath + ".meta";
		bool notTheFirstImport = System.IO.File.Exists(metaFile);
		
		if(!notTheFirstImport)
		{
			//only change the importer settings if
			importer.addCollider = false;
			importer.importMaterials = false;
			importer.globalScale = 1f;
			
			if(importer.assetPath.Contains("@"))
			{
				//this is an animation file
				importer.importAnimation = true;
				importer.generateSecondaryUV = false;
				importer.animationType = ModelImporterAnimationType.Generic;
			}
			else
			{
				importer.animationType = ModelImporterAnimationType.None;
				importer.generateSecondaryUV = true;
				importer.importAnimation = false;
			}
		}
	}
	
	//does not work as clipAnimation is always empty :-(
//	void OnPostprocessModel(GameObject go)
//	{
//		var importer = assetImporter as ModelImporter;
//		if(importer == null) return;
//
//		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Object));
//		if(asset == null && importer.importAnimation)
//		{
//			//animation file!
//			foreach(var clip in importer.clipAnimations)
//			{
//				clip.keepOriginalOrientation = true;
//				clip.keepOriginalPositionXZ = true;
//				clip.keepOriginalPositionY = true;
//				clip.lockRootHeightY = true;
//				clip.lockRootPositionXZ = true;
//				clip.lockRootRotation = true;
//				clip.loopPose = true;
//			}
//		}
//	}
}
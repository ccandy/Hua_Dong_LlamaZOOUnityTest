﻿using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CaveGenerator))]
public class NewBehaviourScript : Editor {
	
	// Use this for initialization

	static int caveNum;
	private string caveName;
	private bool   trapCave;
	public override void OnInspectorGUI()
	{
		

		CaveGenerator myTarget 		= (CaveGenerator)target;
		myTarget.caveWidth  		= EditorGUILayout.IntField ("Cave Width", myTarget.caveWidth);
		myTarget.caveHeight 		= EditorGUILayout.IntField ("Cave Height", myTarget.caveHeight);
		myTarget.caveFillPrectage 	= EditorGUILayout.FloatField ("Cave FillPrectage", myTarget.caveFillPrectage);
		caveName 					= EditorGUILayout.TextField ("Cave Name",caveName);
		trapCave 					= EditorGUILayout.Toggle ("trap Cave", trapCave);
		if(GUILayout.Button("Generate Cave")){
			myTarget.GenerateMap ();
		}

		if(GUILayout.Button("Save Cave")){
//			Cave cave 		=new Cave();
			caveNum ++;
			string meshName = "CaveMesh1"+"" +caveName;
			Debug.Log (meshName +"," +caveName);
			string meshLocation = "Assets/Resources/Meshes/"+meshName+".asset";
			if (!AssetDatabase.IsValidFolder ("Assets/Resources")) {
				AssetDatabase.CreateFolder ("Assets", "Resources");
			}
			if (!AssetDatabase.IsValidFolder ("Assets/Resources/Meshes")) {
				AssetDatabase.CreateFolder ("Assets/Resources", "Meshes");
			}
			Mesh m        = myTarget.GetComponent<MeshFilter>().sharedMesh;

			AssetDatabase.CreateAsset(m, meshLocation);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			string fileName 			= "asset1"+ "" + caveName;
			string fileLocation 		= "Assets/Resources/Prefabs/"+fileName+".prefab";
			if (!AssetDatabase.IsValidFolder ("Assets/Resources")) {
				AssetDatabase.CreateFolder ("Assets", "Resources");
			}
			if (!AssetDatabase.IsValidFolder ("Assets/Resources/Prefabs")) {
				AssetDatabase.CreateFolder ("Assets/Resources", "Prefabs");
			}
				
			GameObject gB 				      = new GameObject ("Cave");
			gB.AddComponent<Cave> ();
			gB.GetComponent<Cave> ().trapCave = trapCave;
			MeshFilter mF 					  = gB.AddComponent<MeshFilter> ();
			mF.sharedMesh 					  = m;

			gB.AddComponent<MeshRenderer> ();

			PrefabUtility.CreatePrefab(fileLocation,gB);
		}


	}
}

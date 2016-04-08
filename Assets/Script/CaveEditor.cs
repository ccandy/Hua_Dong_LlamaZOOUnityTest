using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CaveGenerator))]
public class NewBehaviourScript : Editor {
	
	// Use this for initialization
	public override void OnInspectorGUI()
	{
		

		CaveGenerator myTarget = (CaveGenerator)target;
		myTarget.caveWidth  		= EditorGUILayout.IntField ("Cave Width", myTarget.caveWidth);
		myTarget.caveHeight 		= EditorGUILayout.IntField ("Cave Height", myTarget.caveHeight);
		myTarget.smallestRegion 	= EditorGUILayout.IntField ("Cave SmallestRegion", myTarget.smallestRegion);
		myTarget.caveFillPrectage 	= EditorGUILayout.FloatField ("Cave FillPrectage", myTarget.caveFillPrectage);
		myTarget.numOfSmooth 	    = EditorGUILayout.IntField ("Num Of Smooth",myTarget.numOfSmooth);

		if(GUILayout.Button("Generate Cave")){
			myTarget.GenerateMap ();
		}

		if(GUILayout.Button("Save Cave")){
			Mesh m1 = myTarget.GetComponent<MeshFilter>().sharedMesh;
			AssetDatabase.CreateAsset(m1, "Assets/" + "cave" + ".asset"); 
		}


	}
}

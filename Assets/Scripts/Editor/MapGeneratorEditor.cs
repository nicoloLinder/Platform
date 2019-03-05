using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenerator))]
public class MapGeneratorEditor : Editor {

	//	VARS

		//	PUBLIC

		//	PRIVATE

	//	GETTERS

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR

	public override void OnInspectorGUI() {
		MapGenerator mapGen = (MapGenerator)target;

		if (DrawDefaultInspector ()) {
			if (mapGen.drawMapInEditor) {
				mapGen.DrawMapInEditor ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			if (mapGen.drawMapInEditor) {
				mapGen.seed = System.DateTime.Now.Ticks.ToString();
				mapGen.DrawMapInEditor ();
			}
			else{
				mapGen.seed = System.DateTime.Now.Ticks.ToString();
				// mapGen.Reset();
			}
		}
	}

	//	METHODS

		//	PUBLIC

		//	PRIVATE
}

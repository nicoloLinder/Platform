using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (TerrainGenerator))]
public class TerrainGeneratorEditor : Editor {

	//	VARS

		//	PUBLIC

		//	PRIVATE

	//	GETTERS

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR

	public override void OnInspectorGUI() {
		DrawDefaultInspector ();

		TerrainGenerator terrainGen = (TerrainGenerator)target;
		MapGenerator mapGen = FindObjectOfType<MapGenerator>();

		if (GUILayout.Button ("Generate")) {
			if (Application.isPlaying) {
				terrainGen.Reset();
				mapGen.seed = System.DateTime.Now.Ticks.ToString();
			}
		}
	}

	//	METHODS

		//	PUBLIC

		//	PRIVATE
}

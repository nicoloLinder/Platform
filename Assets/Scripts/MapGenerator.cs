using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	//	VARS

		//	PUBLIC

	public string seed;
	public const int mapSize = 32; 
	[Range(1,100)]
	public int fillPercentage = 1;
	public Vector2 bxsy;
	public bool drawMapInEditor;
	public bool drawFlatMapInEditor;

		//	PRIVATE

	Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();

	//	GETTERS

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR

	void Start() {
		seed = System.DateTime.Now.Ticks.ToString();
	}
	
	void Update(){
		if(mapDataThreadInfoQueue.Count > 0){
			for(int i = 0; i < mapDataThreadInfoQueue.Count; i++){
				MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
				threadInfo.callback(threadInfo.parameter);
			}
		}
	}

	//	METHODS

		//	PUBLIC

	public void DrawMapInEditor(){
		MapDisplay display = FindObjectOfType<MapDisplay>(); 
		MapData mapData = GenerateMapData(Coordinates.zero);
		if(!drawFlatMapInEditor){
			display.DrawMapInEditor(TextureGenerator.GenerateTexture(mapData.cellMap, mapData.groundMap, mapData.airMap, mapData.waterMap));
		}
		else{
            display.DrawMapInEditor(TextureGenerator.GenerateTexture(mapData.airMap));
		}
	}

	public void RequestMapData(Action<MapData> callback, Coordinates mapCoordinates){
		ThreadStart threadStart = delegate {
			MapDataThread(callback, mapCoordinates);
		};

		new Thread(threadStart).Start();
	}

		//	PRIVATE

	void MapDataThread(Action<MapData> callback, Coordinates mapCoordinates){
		MapData mapData = GenerateMapData(mapCoordinates);
		lock (mapDataThreadInfoQueue){
			mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
		}
	}



	public MapData GenerateMapData(Coordinates mapCoordinates) {
		int[,] cellMap = CellularAutomata.GenerateCellularAutomata(seed, mapCoordinates, mapSize, mapSize, fillPercentage, bxsy);
        return new MapData(cellMap, CellularAutomata.GenerateGroundMap(cellMap), CellularAutomata.GenerateCaveMap(cellMap), WaterAutomata.GenerateWaterMap(cellMap));
	}

	//	STRUCT

	struct MapThreadInfo<T> {
		public readonly Action<T> callback;
		public readonly T parameter;

		public MapThreadInfo(Action<T> _callback, T _parameter){
				callback = _callback;
				parameter = _parameter;
		}
	}
}



public struct MapData {

	public readonly int[,] cellMap;
	public int[,] groundMap;
	public int[,] airMap;
	public int [,] waterMap;

	public MapData(int[,] _cellMap, int[,] _groundMap, int[,] _airMap, int[,] _waterMap){
		cellMap = _cellMap;
		groundMap = _groundMap;
		airMap = _airMap;
		waterMap = _waterMap;
	}

}
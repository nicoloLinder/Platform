using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InfiniteTerrain : MonoBehaviour {

	//	VARS

		//	PUBLIC

	static InfiniteTerrain instance;
	
	public static float maxViewDistance = 33;
	public static float maxColliderDistance = 16;
	public Transform viewer;

	public static Vector2 viewerPosition;

	public Sprite[] tiles;
	

		//	PRIVATE

	static MapGenerator mapGenerator;
	int chunkSize;
	int chunksVisibleInViewDistance;

	List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();

	//	GETTERS

	public static InfiniteTerrain GetInstance(){
		return instance;
	}

	public TerrainChunk GetTerrainChunk(Vector2 terrainChunkKey){
		return terrainChunkDictionary[terrainChunkKey];
	}

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR

	void Awake() {
		if(instance == null){
			instance = this;
		}
		else if(instance != this){
			Destroy(this);
		}
	}

	void Start () {
		chunkSize = MapGenerator.mapSize;
		chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
		mapGenerator = FindObjectOfType<MapGenerator>();
	}
	
	void Update () {
		viewerPosition = new Vector2(viewer.position.x, viewer.position.y);
		UpdateVisibleChunk();
	}
	//	METHODS

		//	PUBLIC

	public void Reset(){
		terrainChunkDictionary.Clear();
		terrainChunksVisibleLastUpdate.Clear();
		foreach(Transform child in transform){
			Destroy(child.gameObject);
		}
	}

	public void RemoveTile(GameObject tile) {
		int x = (int)(tile.transform.parent.position.x/32);
		int y = (int)(tile.transform.parent.position.y/32);
		Vector2 chunk = new Vector2(x,y);
		GetTerrainChunk(chunk).RemoveTile(tile);
	}

		//	PRIVATE

	void UpdateVisibleChunk () {
		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

		foreach(TerrainChunk chunk in terrainChunksVisibleLastUpdate){
			chunk.SetVisible(false);
		}

		terrainChunksVisibleLastUpdate.Clear();

		for(int i = -chunksVisibleInViewDistance; i <= chunksVisibleInViewDistance; i++) {
			for(int j = -chunksVisibleInViewDistance; j <= chunksVisibleInViewDistance; j++) {
				Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + i, currentChunkCoordY + j);

				if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
					terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
					if(terrainChunkDictionary[viewedChunkCoord].IsVisible()) {
						terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
					}
				}
				else {
					if((viewedChunkCoord.x + viewedChunkCoord.y) % 2 == 0){
						terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
					}
					else{
						if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.up)){
							terrainChunkDictionary.Add(viewedChunkCoord + Vector2.up, new TerrainChunk(viewedChunkCoord + Vector2.up, chunkSize, transform));
						}
						if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.down)){
							terrainChunkDictionary.Add(viewedChunkCoord + Vector2.down, new TerrainChunk(viewedChunkCoord + Vector2.down, chunkSize, transform));
						}
						if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.left)){
							terrainChunkDictionary.Add(viewedChunkCoord + Vector2.left, new TerrainChunk(viewedChunkCoord + Vector2.left, chunkSize, transform));
						}
						if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.right)){
							terrainChunkDictionary.Add(viewedChunkCoord + Vector2.right, new TerrainChunk(viewedChunkCoord + Vector2.right, chunkSize, transform));
						}
						terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
					}
				}
			}
		}
	}

	//	CLASSES

	public class TerrainChunk {

		//	VARS

			//	PUBLIC

		public MapData mapData;

			//	PRIVATE

		GameObject chunkObject;
		GameObject[,] tiles;
		Vector2 position;
		Vector2 coordinates;
		int size;
		Bounds bounds;
		bool mapDataRequested = false;
		bool mapDataRecieved = false;
		List<Collider2D> colliders = new List<Collider2D>();
		bool collidable = false;

		//	GETTERS

		public bool IsVisible() {
			return chunkObject.activeSelf;
		}
		

		//	SETTERS

		public void SetVisible(bool visible) {
			chunkObject.SetActive(visible);
		}

		//	FACTORY

		public TerrainChunk(Vector2 _coordinates, int size, Transform parent) {
			position = _coordinates * size;
			coordinates = _coordinates;
			this.size = size;
			bounds = new Bounds(position, Vector2.one * size);

			Vector3 positionV3 = new Vector3(position.x, position.y,0);
			
			chunkObject = new GameObject("Chunk : " + coordinates.x + " / "+ coordinates.y);
			chunkObject.transform.parent = parent;
			chunkObject.transform.position = positionV3;
			chunkObject.transform.localScale = Vector3.one;

			if ((coordinates.x + coordinates.y)  % 2 == 0) {
				mapGenerator.RequestMapData(OnMapDataRecieved, coordinates);
				mapDataRequested = true;
			}

			SetVisible(false);
		}

		//	METHODS

			//	PUBLIC

		public void UpdateTerrainChunk() {
			if (!mapDataRequested) {
				if (InfiniteTerrain.GetInstance().GetTerrainChunk(coordinates + Vector2.up).mapDataRecieved && InfiniteTerrain.GetInstance().GetTerrainChunk(coordinates + Vector2.down).mapDataRecieved && InfiniteTerrain.GetInstance().GetTerrainChunk(coordinates + Vector2.left).mapDataRecieved && InfiniteTerrain.GetInstance().GetTerrainChunk(coordinates + Vector2.right).mapDataRecieved) {
					mapGenerator.RequestMapData(OnMapDataRecieved, coordinates);
					mapDataRequested = true;
				}
			}

			float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
			if(colliders.Count != 0 && !collidable && viewerDistanceFromNearestEdge <= maxColliderDistance){
				foreach(Collider2D collider in colliders){
					collider.enabled = true;
				}
				collidable = true;
			}
			if(colliders.Count != 0 && collidable &&  viewerDistanceFromNearestEdge > maxColliderDistance){
				foreach(Collider2D collider in colliders){
					collider.enabled = false;
				}
				collidable = false;
			}
			bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;
			SetVisible(visible);
		}

		public void RemoveTile(GameObject tile){
			Vector2 index = GetTileIndex(tile);
			try {
				tiles[(int)index.x, (int)index.y] = null;
			} 
			catch (System.Exception e) {
				Debug.LogError(index.ToString() + " " + tile + " " + coordinates);
			}
			mapData.cellMap[(int)index.x, (int)index.y] = 0;
			colliders.Remove(tile.GetComponent<Collider2D>());
			Destroy(tile);

			if(index.x > 0 && index.y > 0 && index.x < tiles.GetLength(0)-1 && index.y < tiles.GetLength(1)-1){
				if(mapData.cellMap[(int)index.x,(int)index.y+1] != 0) {
					UpdateTile(index + Vector2.up);
				}
				if(mapData.cellMap[(int)index.x,(int)index.y-1] != 0) {
					UpdateTile(index + Vector2.down);
				}
				if(mapData.cellMap[(int)index.x-1,(int)index.y] != 0) {
					UpdateTile(index + Vector2.left);
				}
				if(mapData.cellMap[(int)index.x+1,(int)index.y] != 0) {
					UpdateTile(index + Vector2.right);
				}
			}
		}

			//	PRIVATE
		
		void OnMapDataRecieved(MapData _mapData) {
			mapData = _mapData;
			// chunkObject.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.GenerateTexture(_mapData.cellMap);
			GenerateChunk();
			mapDataRecieved = true;
		}

		void GenerateChunk() {
			tiles = new GameObject[mapData.cellMap.GetLength(0),mapData.cellMap.GetLength(1)];
			
			for(int i = 0; i < mapData.cellMap.GetLength(0); i++) {
				for(int j = 0; j < mapData.cellMap.GetLength(1); j++) {
					if(mapData.cellMap[i,j] != 0){
						tiles[i,j] = CreateTile(new Vector2(i,j));
						// Debug.Log(neighbourMap[i,j]);
					}
				}
			}
		}

		GameObject CreateTile(Vector2 coords){
			int configuration = 0;
			int x = (int)coords.X;
			int y = (int)coords.Y;

			if(x == 0 || mapData.cellMap[x-1,y] != 0) {
				configuration += 8;
			}
			if(y == mapData.cellMap.GetLength(1)-1 || mapData.cellMap[x,y+1] != 0) {
				configuration += 4;
			}
			if(x == mapData.cellMap.GetLength(0)-1 || mapData.cellMap[x+1,y] != 0) {
				configuration += 2;
			}
			if(y == 0 || mapData.cellMap[x,y-1] != 0) {
				configuration += 1;
			}


			GameObject temp = new GameObject(x + "/" + y);
			temp.tag = "Tile";
			temp.transform.parent = chunkObject.transform;
			temp.transform.localPosition = new Vector3(x-size/2,y-size/2,0);
			temp.AddComponent<SpriteRenderer>().sprite = InfiniteTerrain.GetInstance().tiles[configuration];
			if(configuration != 15){
				colliders.Add(temp.AddComponent<EdgeCollider2D>());
				((EdgeCollider2D)colliders[colliders.Count-1]).points = GenerateEdgeCollider2DPoints(configuration);
				colliders[colliders.Count-1].enabled = false;
			}
			return temp;
		}

		Vector2 GetTileIndex(GameObject tile){
			for(int x = 0; x < tiles.GetLength(0); x++) {
				for(int y = 0; y < tiles.GetLength(1); y++) {
					if(tile == tiles[x,y]){
						return new Vector2(x,y);
					}
				}
			}
			return Vector2.zero - Vector2.one;
		}

		void UpdateTile(Vector2 coords){
			int x = (int)coords.X;
			int y = (int)coords.Y;
			int configuration = GetConfiguration(coords);

			GameObject temp = tiles[x,y];
			temp.GetComponent<SpriteRenderer>().sprite = InfiniteTerrain.GetInstance().tiles[configuration];
			if(configuration != 15){
				if(temp.GetComponent<EdgeCollider2D>() == null){
					colliders.Add(temp.AddComponent<EdgeCollider2D>());
					((EdgeCollider2D)colliders[colliders.Count-1]).points = GenerateEdgeCollider2DPoints(configuration);
				}
				temp.GetComponent<EdgeCollider2D>().points = GenerateEdgeCollider2DPoints(configuration);;
				// ((EdgeCollider2D)colliders[colliders.Count-1]);
				// colliders[colliders.Count-1].enabled = false;
			}
		}

		int GetConfiguration(Vector2 coords) {
			int configuration = 0;
			int x = (int)coords.X;
			int y = (int)coords.Y;

			if(x == 0 || mapData.cellMap[x-1,y] != 0) {
				configuration += 8;
			}
			if(y == mapData.cellMap.GetLength(1)-1 || mapData.cellMap[x,y+1] != 0) {
				configuration += 4;
			}
			if(x == mapData.cellMap.GetLength(0)-1 || mapData.cellMap[x+1,y] != 0) {
				configuration += 2;
			}
			if(y == 0 || mapData.cellMap[x,y-1] != 0) {
				configuration += 1;
			}

			return configuration;
		}

		Vector2[] GenerateEdgeCollider2DPoints(int configuration){
			List<Vector2> points = new List<Vector2>();
			Vector2 topLeft = new Vector2(-0.5f,0.5f);
			Vector2 topRight = new Vector2(0.5f,0.5f);
			Vector2 bottomRight = new Vector2(0.5f,-0.5f);
			Vector2 bottomLeft = new Vector2(-0.5f,-0.5f);

			switch (configuration) {
			case 0:
				points.Add(topLeft);
				points.Add(topRight);
				points.Add(bottomRight);
				points.Add(bottomLeft);
				points.Add(topLeft);
				break;
			case 1:
				points.Add(bottomLeft);
				points.Add(topLeft);
				points.Add(topRight);
				points.Add(bottomRight);
				break;
			case 2:
				points.Add(bottomRight);
				points.Add(bottomLeft);
				points.Add(topLeft);
				points.Add(topRight);
				break;
			case 3:
				points.Add(bottomLeft);
				points.Add(topLeft);
				points.Add(topRight);
				break;
			case 4:
				points.Add(topRight);
				points.Add(bottomRight);
				points.Add(bottomLeft);
				points.Add(topLeft);
				break;
			case 5:
				points.Add(topLeft);
				points.Add(topRight);
				points.Add(bottomRight);
				points.Add(bottomLeft);
				points.Add(topLeft);
				break;
			case 6:
				points.Add(topLeft);
				points.Add(bottomLeft);
				points.Add(bottomRight);
				break;
			case 7:
				points.Add(topLeft);
				points.Add(bottomLeft);
				break;
			case 8:
				points.Add(topLeft);
				points.Add(topRight);
				points.Add(bottomRight);
				points.Add(bottomLeft);
				break;
			case 9:
				points.Add(topLeft);
				points.Add(topRight);
				points.Add(bottomRight);
				break;
			case 10:
				points.Add(topLeft);
				points.Add(topRight);
				points.Add(bottomRight);
				points.Add(bottomLeft);
				points.Add(topLeft);
				break;
			case 11:
				points.Add(topLeft);
				points.Add(topRight);
				break;
			case 12:
				points.Add(topRight);
				points.Add(bottomRight);
				points.Add(bottomLeft);
				break;
			case 13:
				points.Add(topRight);
				points.Add(bottomRight);
				break;
			case 14:
				points.Add(bottomLeft);
				points.Add(bottomRight);
				break;
			}
			return points.ToArray();
		}
	}

}

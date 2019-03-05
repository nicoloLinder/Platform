using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGenerator : MonoBehaviour {

	//	VARS

		//	PUBLIC

	public static TerrainGenerator Instance { get { return instance; } }

	public static float maxViewDistance;
	public static float maxSpawnDistance;
	public static float maxColliderDistance;

	public Material material;
    public Material tileMaterial;
    public Material backgroundMaterial;

    public bool drawGizmo;

	public Transform player;
	public static Vector2 playerPosition;

	public Sprite[] tiles;
    public Sprite[] fullTileAlt;
    public Sprite[] backgoundTiles;

    //	PRIVATE

    private static TerrainGenerator instance;

	private static MapGenerator mapGenerator;
	private int chunkSize;
	// private int chunksVisibleInViewDistance;

	List<Chunk> chunksVisibleLastUpdate = new List<Chunk>();

	Dictionary<Coordinates, Chunk> chunkDictionary = new Dictionary<Coordinates, Chunk>();

	//	GETTERS

	public Chunk GetChunk(Coordinates chunkCoord) {
		if(chunkDictionary.ContainsKey(chunkCoord)){
			return chunkDictionary[chunkCoord];
		}
		return null;
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


	public void Reset(){
		chunkDictionary.Clear();
		chunksVisibleLastUpdate.Clear();
		foreach(Transform child in transform){
			Destroy(child.gameObject);
		}
		player.gameObject.SetActive(false);
		Camera.main.gameObject.GetComponent<CameraFollow>().enabled = false;
		InvokeRepeating("ActivatePlayer", 0, 0.1f);
	}

	void Start () {
		chunkSize = MapGenerator.mapSize;
		maxViewDistance = 48;
		maxSpawnDistance = 64;
		maxColliderDistance = 8;
		// chunksVisibleInViewDistance = Mathf.RoundToInt(maxSpawnDistance / chunkSize);
		mapGenerator = FindObjectOfType<MapGenerator>();

		InvokeRepeating("ActivatePlayer", 0, 0.1f);

		// SetUpTerrain();
	}
	
	void Update () {
		playerPosition = (Vector2) player.position;
		SetUpTerrain();
        if(Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene(0);
        }
	}

	void OnDrawGizmos(){
		int currentChunkCoordX = Mathf.RoundToInt(playerPosition.x / 32);
		int currentChunkCoordY = Mathf.RoundToInt(playerPosition.y / 32);
		Coordinates currentCoords = new Coordinates(currentChunkCoordX, currentChunkCoordY);
		Coordinates coords;

		for(int x = -2; x <= 2 ; x++) {
			for(int y = -2; y <= 2 ; y++) {
				coords = currentCoords + new Coordinates(x,y);
				Gizmos.color = Color.red;
				Gizmos.DrawRay((((Vector3)coords)+Vector3.up/2 + Vector3.left/2)*32+Vector3.down*0.5f, Vector3.right*32);
				Gizmos.DrawRay((((Vector3)coords)+Vector3.up/2 + Vector3.left/2)*32+Vector3.left*0.5f, Vector3.down*32);
				Gizmos.DrawRay((((Vector3)coords)+Vector3.down/2 + Vector3.left/2)*32+Vector3.down*0.5f, Vector3.right*32);
				Gizmos.DrawRay((((Vector3)coords)+Vector3.down/2 + Vector3.right/2)*32+Vector3.left*0.5f, Vector3.up*32);
			}
		}
	}

	//	METHODS

		//	PUBLIC

	public void AddTile(Tile tile, Coordinates coords) {
		// Debug.Log(tile + "|" + coords);
		Chunk chunk;
		if(coords.X < 0) {
			if(coords.Y < 0) {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.down + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(chunkSize-1, chunkSize-1));
				}
			}
			else if(coords.Y >= chunkSize) {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.up + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(chunkSize-1, 0));
				}
			}
			else {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(chunkSize-1, coords.Y));
				}
			}
		}
		if(coords.X >= chunkSize) {
			if(coords.Y < 0) {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.down + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(0, chunkSize-1));
				}
			}
			else if(coords.Y >= chunkSize) {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.down + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(0, 0));
				}
			}
			else {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.down + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(0, coords.Y));
				}
			}
		}
		else{
			if(coords.Y < 0) {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.down + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(coords.X, chunkSize-1));
				}
			}
			else if(coords.Y >= chunkSize) {
				chunk = chunkDictionary[tile.ParentChunk.Coords + Coordinates.down + Coordinates.left];
				if(chunk != null){
					chunk.AddTile(new Coordinates(coords.X, 0));
				}
			}
			else {
				tile.ParentChunk.AddTile(coords);
			}
		}

		
	}

	public void RemoveTile(Tile tile) {
		tile.ParentChunk.RemoveTile(tile);
	}

		//	PRIVATE

	void UpdateVisibleChunks() {
		int currentChunkCoordX = Mathf.RoundToInt(playerPosition.x / 32);
		int currentChunkCoordY = Mathf.RoundToInt(playerPosition.y / 32);
		Coordinates currentCoords = new Coordinates(currentChunkCoordX, currentChunkCoordY);
		Coordinates coords;
		foreach(Chunk chunk in chunksVisibleLastUpdate) {
			chunk.Visible = false;
		}

		chunksVisibleLastUpdate.Clear();

		for(int x = -2; x <= 2 ; x++) {
			for(int y = -2; y <= 2 ; y++) {
				coords = currentCoords + new Coordinates(x,y);

				if(chunkDictionary.ContainsKey(coords)) {
					chunkDictionary[coords].UpdateChunk();
					if(chunkDictionary[coords].Visible) {
						chunksVisibleLastUpdate.Add(chunkDictionary[coords]);
					}
				}
				else {
					if((coords.X + coords.Y) % 2 == 0) {
						chunkDictionary.Add(coords, new Chunk(coords, chunkSize, transform, mapGenerator));
					}
					else {
						if(!chunkDictionary.ContainsKey(coords + Coordinates.up)) {
							chunkDictionary.Add(coords + Coordinates.up, new Chunk(coords + Coordinates.up, chunkSize, transform, mapGenerator));
						}
						if(!chunkDictionary.ContainsKey(coords + Coordinates.down)) {
							chunkDictionary.Add(coords + Coordinates.down, new Chunk(coords + Coordinates.down, chunkSize, transform, mapGenerator));
						}
						if(!chunkDictionary.ContainsKey(coords + Coordinates.left)) {
							chunkDictionary.Add(coords + Coordinates.left, new Chunk(coords + Coordinates.left, chunkSize, transform, mapGenerator));
						}
						if(!chunkDictionary.ContainsKey(coords + Coordinates.right)) {
							chunkDictionary.Add(coords + Coordinates.right, new Chunk(coords + Coordinates.right, chunkSize, transform, mapGenerator));
						}
						try {
							chunkDictionary.Add(coords, new Chunk(coords, chunkSize, transform, mapGenerator));
						}
						catch (System.Exception e) {
							Debug.LogError(coords);
							Debug.LogError(e);
						}
					}
				}
			}
		}
	}

	void SetUpTerrain() {
		int currentChunkCoordX = Mathf.RoundToInt(playerPosition.x / 32);
		int currentChunkCoordY = Mathf.RoundToInt(playerPosition.y / 32);
		Coordinates currentCoords = new Coordinates(currentChunkCoordX, currentChunkCoordY);
		Coordinates coords;

		for(int x = -2; x <= 2 ; x++) {
			for(int y = -2; y <= 2 ; y++) {
				coords = currentCoords + new Coordinates(x,y);

				if(chunkDictionary.ContainsKey(coords)) {
					chunkDictionary[coords].UpdateChunk();
					if(chunkDictionary[coords].Visible) {
						chunksVisibleLastUpdate.Add(chunkDictionary[coords]);
						GetChunk(coords).Gizmo = drawGizmo;
					}
				}
				else {
					if((coords.X + coords.Y) % 2 == 0) {
						chunkDictionary.Add(coords, new Chunk(coords, chunkSize, transform, mapGenerator, mapGenerator.GenerateMapData(coords)));
					}
					else {
						if(!chunkDictionary.ContainsKey(coords + Coordinates.up)) {
							chunkDictionary.Add(coords + Coordinates.up, new Chunk(coords + Coordinates.up, chunkSize, transform, mapGenerator, mapGenerator.GenerateMapData(coords + Coordinates.up)));
						}
						if(!chunkDictionary.ContainsKey(coords + Coordinates.down)) {
							chunkDictionary.Add(coords + Coordinates.down, new Chunk(coords + Coordinates.down, chunkSize, transform, mapGenerator, mapGenerator.GenerateMapData(coords + Coordinates.down)));
						}
						if(!chunkDictionary.ContainsKey(coords + Coordinates.left)) {
							chunkDictionary.Add(coords + Coordinates.left, new Chunk(coords + Coordinates.left, chunkSize, transform, mapGenerator, mapGenerator.GenerateMapData(coords + Coordinates.left)));
						}
						if(!chunkDictionary.ContainsKey(coords + Coordinates.right)) {
							chunkDictionary.Add(coords + Coordinates.right, new Chunk(coords + Coordinates.right, chunkSize, transform, mapGenerator, mapGenerator.GenerateMapData(coords + Coordinates.right)));
						}
						chunkDictionary.Add(coords, new Chunk(coords, chunkSize, transform, mapGenerator, mapGenerator.GenerateMapData(coords)));
					}
				}
			}
		}
	}

	void ActivatePlayer() {
		if(chunkDictionary.ContainsKey(Coordinates.zero) && chunkDictionary[Coordinates.zero].ChunkGenerated) {
			player.gameObject.SetActive(true);
			player.transform.position = Vector3.zero;
			Camera.main.gameObject.GetComponent<CameraFollow>().enabled = true;
			CancelInvoke();
		}
	}
}

// if(terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
// 	terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
// 	if(terrainChunkDictionary[viewedChunkCoord].IsVisible()) {
// 		terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunkCoord]);
// 	}
// }
// else {
// 	if((viewedChunkCoord.x + viewedChunkCoord.y) % 2 == 0){
// 		terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
// 	}
// 	else{
// 		if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.up)){
// 			terrainChunkDictionary.Add(viewedChunkCoord + Vector2.up, new TerrainChunk(viewedChunkCoord + Vector2.up, chunkSize, transform));
// 		}
// 		if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.down)){
// 			terrainChunkDictionary.Add(viewedChunkCoord + Vector2.down, new TerrainChunk(viewedChunkCoord + Vector2.down, chunkSize, transform));
// 		}
// 		if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.left)){
// 			terrainChunkDictionary.Add(viewedChunkCoord + Vector2.left, new TerrainChunk(viewedChunkCoord + Vector2.left, chunkSize, transform));
// 		}
// 		if(!terrainChunkDictionary.ContainsKey(viewedChunkCoord + Vector2.right)){
// 			terrainChunkDictionary.Add(viewedChunkCoord + Vector2.right, new TerrainChunk(viewedChunkCoord + Vector2.right, chunkSize, transform));
// 		}
// 		terrainChunkDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
// 	}
// }

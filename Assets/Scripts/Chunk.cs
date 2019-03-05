using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    CAVE_MAP
}

public class Chunk
{

    //	VARS

    //	PUBLIC


    public Coordinates Coords { get { return coords; } }
    public bool Visible { get { return visible; } set { visible = value; } }
    public bool Gizmo { get { return gizmo; } set { if (gizmo != value) { debugGameObject.SetActive(value); } gizmo = value; } }
    public int Size { get { return size; } }
    public bool ChunkGenerated { get { return chunkGenerated; } }
    public MapData mapData;


    //	PRIVATE

    private Coordinates coords;
    private bool visible;
    private bool gizmo = true;
    private int size;
    private Bounds bounds;
    private GameObject chunkGameObject;
    private GameObject debugGameObject;

    private bool mapDataRequested;
    private bool mapDataRecieved;
    private bool chunkGenerated;

    private bool collidable;

    private GameObject[,] chunkTiles;
    private GameObject[,] backgroundTiles;
    private List<EdgeCollider2D> chunkColliders = new List<EdgeCollider2D>();

    private MapGenerator mapGenerator;

    //	GETTERS

    public int GetTileValue(Coordinates coords)
    {
        return mapData.cellMap[coords.X, coords.Y];
    }

    public int GetTileValue(Coordinates coords, MapType mapType)
    {
        switch (mapType)
        {
            case MapType.CAVE_MAP:
                return mapData.airMap[coords.X, coords.Y];
            default:
                return mapData.cellMap[coords.X, coords.Y];
        }

    }

    public Tile GetTile(Coordinates coords)
    {
        Chunk tempChunk = GetChunk(coords);
        if (tempChunk.chunkTiles[coords.X, coords.Y] != null)
        {
            return tempChunk.chunkTiles[coords.X, coords.Y].GetComponent<Tile>();
        }
        else
        {
            return null;
        }
    }

    //	SETTERS

    //	FACTORY

    public Chunk(Coordinates _coord, int _size, Transform parent, MapGenerator _mapGenerator)
    {
        coords = _coord;
        size = _size;
        mapGenerator = _mapGenerator;

        Vector3 position = (Vector3)(coords * size);

        bounds = new Bounds(position, Vector2.one * size);

        chunkGameObject = new GameObject("Chunk :" + coords.X + "|" + coords.Y);
        chunkGameObject.transform.parent = parent;
        chunkGameObject.transform.position = position;
        chunkGameObject.transform.localScale = Vector3.one;

        debugGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        debugGameObject.transform.parent = chunkGameObject.transform;
        debugGameObject.transform.position = position + Vector3.forward;
        debugGameObject.transform.localScale = Vector3.one * 3.2f;
        debugGameObject.GetComponent<MeshRenderer>().material = TerrainGenerator.Instance.material;
        debugGameObject.transform.localRotation = Quaternion.Euler(90, 180, 0);
        Gizmo = false;

        if ((coords.X + coords.Y) % 2 == 0)
        {
            mapGenerator.RequestMapData(OnMapDataRecieved, coords);
            mapDataRequested = true;
        }

        Visible = false;
        chunkGameObject.SetActive(false);
    }
    public Chunk(Coordinates _coord, int _size, Transform parent, MapGenerator _mapGenerator, MapData _mapData)
    {
        coords = _coord;
        size = _size;
        mapGenerator = _mapGenerator;

        Vector3 position = (Vector3)(coords * size);

        bounds = new Bounds(position, Vector2.one * size);

        // chunkGameObject = new GameObject("Chunk :" + coords.X + "|" + coords.Y);
        chunkGameObject = new GameObject("Chunk :" + coords.X + "|" + coords.Y);
        chunkGameObject.transform.parent = parent;
        chunkGameObject.transform.position = position;
        chunkGameObject.transform.localScale = Vector3.one;

        debugGameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
        debugGameObject.transform.parent = chunkGameObject.transform;
        debugGameObject.transform.position = position + Vector3.forward - Vector3.right * 0.5f - Vector3.up * 0.5f;
        debugGameObject.transform.localScale = Vector3.one * 3.2f;
        debugGameObject.transform.localRotation = Quaternion.Euler(90, 180, 0);
        debugGameObject.GetComponent<MeshRenderer>().material = TerrainGenerator.Instance.material;
        debugGameObject.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.GenerateTexture(_mapData.cellMap, _mapData.groundMap, _mapData.airMap, _mapData.waterMap);
        Gizmo = false;


        mapData = _mapData;
        mapDataRequested = true;
        mapDataRecieved = true;

        Visible = false;
        chunkGameObject.SetActive(false);
    }

    //	METHODS

    //	PUBLIC

    public void UpdateChunk()
    {
        CheckForChunkGeneration();

        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(TerrainGenerator.playerPosition));

        if (chunkColliders.Count != 0 && !collidable && viewerDistanceFromNearestEdge <= TerrainGenerator.maxColliderDistance)
        {
            foreach (Collider2D collider in chunkColliders)
            {
                collider.enabled = true;
            }
            collidable = true;
        }
        if (chunkColliders.Count != 0 && collidable && viewerDistanceFromNearestEdge > TerrainGenerator.maxColliderDistance)
        {
            foreach (Collider2D collider in chunkColliders)
            {
                collider.enabled = false;
            }
            collidable = false;
        }

        Visible = viewerDistanceFromNearestEdge <= TerrainGenerator.maxViewDistance;
        chunkGameObject.SetActive(visible);
    }

    public void AddTile(Coordinates tileCoords)
    {
        try
        {
            if (mapData.cellMap[tileCoords.X, tileCoords.Y] == 1 || chunkTiles[tileCoords.X, tileCoords.Y] != null)
            {
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        mapData.cellMap[tileCoords.X, tileCoords.Y] = 1;
        mapData.groundMap = CellularAutomata.GenerateGroundMap(mapData.cellMap);
        mapData.airMap = CellularAutomata.GenerateCaveMap(mapData.cellMap);
        chunkTiles[tileCoords.X, tileCoords.Y] = CreateTile(tileCoords);
        if (gizmo)
        {
            debugGameObject.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.GenerateTexture(mapData.cellMap, mapData.groundMap, mapData.airMap, mapData.waterMap);
        }
        Collider2D tempCollider = chunkTiles[tileCoords.X, tileCoords.Y].GetComponent<EdgeCollider2D>();
        if (tempCollider != null)
        {
            chunkTiles[tileCoords.X, tileCoords.Y].GetComponent<EdgeCollider2D>().enabled = true;
            chunkTiles[tileCoords.X, tileCoords.Y].GetComponent <PlatformEffector2D>().enabled = true;
        }

        IterateTileToUpdate(tileCoords);
    }

    public void RemoveTile(Tile tile)
    {

        Coordinates tileCoords = tile.Coords;

        chunkTiles[tileCoords.X, tileCoords.Y] = null;
        mapData.cellMap[tileCoords.X, tileCoords.Y] = 0;
        mapData.groundMap = CellularAutomata.GenerateGroundMap(mapData.cellMap);
        mapData.airMap = CellularAutomata.GenerateCaveMap(mapData.cellMap);
        if (gizmo)
        {
            debugGameObject.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.GenerateTexture(mapData.cellMap, mapData.groundMap, mapData.airMap, mapData.waterMap);
        }
        chunkColliders.Remove(tile.gameObject.GetComponent<EdgeCollider2D>());
        GameObject.Destroy(tile.gameObject);

        IterateTileToUpdate(tileCoords);
    }

    public void IterateTileToUpdate(Coordinates tileCoords)
    {
        for (int x = tileCoords.X - 1; x <= tileCoords.X + 1; x++)
        {
            for (int y = tileCoords.Y - 1; y <= tileCoords.Y + 1; y++)
            {
                Tile tempTile;
                //if(x == tileCoords.X && y == tileCoords.Y){
                //    continue;
                //}
                if (y > size - 1)
                {
                    if (x > size - 1)
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up + Coordinates.right).GetTile(new Coordinates(0, 0));
                    }
                    else if (x < 0)
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up + Coordinates.left).GetTile(new Coordinates(size - 1, 0));
                    }
                    else
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up).GetTile(new Coordinates(tileCoords.X, 0));
                    }
                }
                else if (y < 0)
                {
                    if (x > size - 1)
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down + Coordinates.right).GetTile(new Coordinates(0, size - 1));
                    }
                    else if (x < 0)
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down + Coordinates.left).GetTile(new Coordinates(size - 1, size - 1));
                    }
                    else
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down).GetTile(new Coordinates(tileCoords.X, size - 1));
                    }
                }
                else
                {
                    if (x > size - 1)
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.right).GetTile(new Coordinates(0, tileCoords.Y));
                    }
                    else if (x < 0)
                    {
                        tempTile = TerrainGenerator.Instance.GetChunk(coords + Coordinates.left).GetTile(new Coordinates(size - 1, tileCoords.Y));
                    }
                    else
                    {
                        tempTile = GetTile(new Coordinates(x, y));
                    }
                }
                if (tempTile != null)
                {
                    tempTile.ParentChunk.UpdateTile(tempTile.Coords);
                }
            }
        }
    }

    //	PRIVATE

    void CheckForChunkGeneration()
    {
        if (!mapDataRequested)
        {
            if (TerrainGenerator.Instance.GetChunk(coords + Coordinates.up).mapDataRecieved && TerrainGenerator.Instance.GetChunk(coords + Coordinates.down).mapDataRecieved && TerrainGenerator.Instance.GetChunk(coords + Coordinates.left).mapDataRecieved && TerrainGenerator.Instance.GetChunk(coords + Coordinates.right).mapDataRecieved)
            {
                mapGenerator.RequestMapData(OnMapDataRecieved, coords);
                mapDataRequested = true;
            }
        }
        if (!chunkGenerated && mapDataRecieved)
        {
            Chunk up = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up);
            Chunk down = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down);
            Chunk left = TerrainGenerator.Instance.GetChunk(coords + Coordinates.left);
            Chunk right = TerrainGenerator.Instance.GetChunk(coords + Coordinates.right);
            Chunk topLeft = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up + Coordinates.left);
            Chunk topRight = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up + Coordinates.right);
            Chunk bottomLeft = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down + Coordinates.left);
            Chunk bottomRight = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down + Coordinates.right);

            if (up != null && down != null && left != null && right != null && topLeft != null && topRight != null && bottomLeft != null && bottomRight != null)
            {
                if (up.mapDataRecieved && down.mapDataRecieved && left.mapDataRecieved && right.mapDataRecieved && topLeft.mapDataRecieved && topRight.mapDataRecieved && bottomLeft.mapDataRecieved && bottomRight.mapDataRecieved)
                {
                    GenerateChunk();
                    chunkGenerated = true;
                }
            }
        }
    }

    void GenerateChunk()
    {
        chunkTiles = new GameObject[size, size];
        backgroundTiles = new GameObject[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (mapData.cellMap[x, y] != 0)
                {
                    chunkTiles[x, y] = CreateTile(new Coordinates(x, y));
                }
                if (mapData.airMap[x, y] != 0)
                {
                    backgroundTiles[x, y] = CreateBackgroundTile(new Coordinates(x, y));
                }
            }
        }
    }

    GameObject CreateTile(Coordinates _coords)
    {
        int configuration = GetConfiguration(_coords);

        GameObject temp = new GameObject(_coords.X + "/" + _coords.Y);
        temp.tag = "Tile";
        temp.layer = 9;
        temp.transform.parent = chunkGameObject.transform;
        temp.transform.localPosition = new Vector3(_coords.X - size / 2, _coords.Y - size / 2, -0.1f);
        //if(configuration == 15) {
        //	temp.transform.localScale = new Vector3(Mathf.Sign(Random.Range(-1,2)), Mathf.Sign(Random.Range(-1,2)),1);
        //}
        //else {
        temp.transform.localScale = new Vector3(1, 1, 1);

        //}
        temp.AddComponent<SpriteRenderer>().sprite = TerrainGenerator.Instance.tiles[configuration];
        if(configuration == 15 && Random.Range(0,10) == 5){
            temp.GetComponent<SpriteRenderer>().sprite =  TerrainGenerator.Instance.fullTileAlt[Random.Range(0, TerrainGenerator.Instance.fullTileAlt.Length)];
        }
        temp.GetComponent<SpriteRenderer>().material = TerrainGenerator.Instance.tileMaterial;
        temp.AddComponent<Tile>().SetupTile(_coords, this);

        if (configuration < 15)
        {
            chunkColliders.Add(temp.AddComponent<EdgeCollider2D>());
            PlatformEffector2D platformEffector = temp.AddComponent<PlatformEffector2D>();
            platformEffector.useOneWay = true;
            chunkColliders[chunkColliders.Count - 1].points = GenerateEdgeCollider2DPoints(configuration);
            chunkColliders[chunkColliders.Count - 1].enabled = false;
            chunkColliders[chunkColliders.Count - 1].usedByEffector = true;
        }
        return temp;
    }

    GameObject CreateBackgroundTile(Coordinates _coords)
    {
        int configuration = GetConfiguration(_coords);
        if(configuration < GetConfiguration(_coords, MapType.CAVE_MAP)){
            configuration = GetConfiguration(_coords, MapType.CAVE_MAP);
        }

        GameObject temp = new GameObject(_coords.X + "/" + _coords.Y);
        temp.tag = "Tile";
        temp.layer = 9;
        temp.transform.parent = chunkGameObject.transform;
        temp.transform.localPosition = new Vector3(_coords.X - size / 2, _coords.Y - size / 2, 0);

        temp.transform.localScale = new Vector3(1, 1, 1);

        SpriteRenderer spriteRenderer = temp.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = TerrainGenerator.Instance.backgoundTiles[configuration];
        spriteRenderer.sortingOrder = -1;
        spriteRenderer.material = TerrainGenerator.Instance.backgroundMaterial;
        temp.AddComponent<Tile>().SetupTile(_coords, this);


        return temp;
    }

    void UpdateTile(Coordinates coords)
    {
        if (GetTile(coords) == null)
        {
            return;
        }
        int configuration = GetConfiguration(coords);

        if (configuration == 1 || configuration == 2 || configuration == 4 || configuration == 5 || configuration == 8 || configuration == 10 || configuration == 0)
        {
            RemoveTile(GetTile(coords));
            return;
        }

        GameObject temp = chunkTiles[coords.X, coords.Y];
        try
        {
            temp.GetComponent<SpriteRenderer>().sprite = TerrainGenerator.Instance.tiles[configuration];
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        //if(configuration == 15) {
        //	temp.transform.localScale = new Vector3(Mathf.Sign(Random.Range(-1,2)), Mathf.Sign(Random.Range(-1,2)),1);
        //}
        //else {
        temp.transform.localScale = new Vector3(1, 1, 1);
        //}

        if (configuration != 15)
        {
            if (temp.GetComponent<EdgeCollider2D>() == null)
            {
                chunkColliders.Add(temp.AddComponent<EdgeCollider2D>());
                ((EdgeCollider2D)chunkColliders[chunkColliders.Count - 1]).points = GenerateEdgeCollider2DPoints(configuration);
            }
            temp.GetComponent<EdgeCollider2D>().points = GenerateEdgeCollider2DPoints(configuration); ;
        }
    }

    int GetConfiguration(Coordinates tileCoords)
    {
        int configuration = 0;

        int up = 0;
        if (tileCoords.Y == size - 1)
        {
            up = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up).GetTileValue(new Coordinates(tileCoords.X, 0));
        }
        else
        {
            up = GetTileValue(tileCoords + Coordinates.up);
        }
        int down = 0;
        if (tileCoords.Y == 0)
        {
            down = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down).GetTileValue(new Coordinates(tileCoords.X, size - 1));
        }
        else
        {
            down = GetTileValue(tileCoords + Coordinates.down);
        }
        int left = 0;
        if (tileCoords.X == 0)
        {
            left = TerrainGenerator.Instance.GetChunk(coords + Coordinates.left).GetTileValue(new Coordinates(size - 1, tileCoords.Y));
        }
        else
        {
            left = GetTileValue(tileCoords + Coordinates.left);
        }
        int right = 0;
        if (tileCoords.X == size - 1)
        {
            right = TerrainGenerator.Instance.GetChunk(coords + Coordinates.right).GetTileValue(new Coordinates(0, tileCoords.Y));
        }
        else
        {
            right = GetTileValue(tileCoords + Coordinates.right);
        }

        if (left != 0)
        {
            configuration += 8;
        }
        if (up != 0)
        {
            configuration += 4;
        }
        if (right != 0)
        {
            configuration += 2;
        }
        if (down != 0)
        {
            configuration += 1;
        }

        if (configuration == 15)
        {
            if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X + 1, tileCoords.Y + 1), new Coordinates(tileCoords.X + 1, tileCoords.Y + 1)))
            {
                configuration = 16;
            }
            else if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X + -1, tileCoords.Y + 1), new Coordinates(tileCoords.X - 1, tileCoords.Y + 1)))
            {
                configuration = 17;
            }
            else if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X - 1, tileCoords.Y - 1), new Coordinates(tileCoords.X - 1, tileCoords.Y - 1)))
            {
                configuration = 18;
            }
            else if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X + 1, tileCoords.Y - 1), new Coordinates(tileCoords.X + 1, tileCoords.Y - 1)))
            {
                configuration = 19;
            }
        }

        //switch (configuration) {
        //case 3:
        //	if(CheckDiagonal(configuration, new Coordinates(tileCoords.X+1, tileCoords.Y+1), new Coordinates(tileCoords.X-1, tileCoords.Y-1))) {
        //		configuration = 16;
        //	}
        //	break;
        //case 6:
        //	if(CheckDiagonal(configuration, new Coordinates(tileCoords.X-1, tileCoords.Y+1), new Coordinates(tileCoords.X+1, tileCoords.Y-1))) {
        //		configuration = 17;
        //	}
        //	break;
        //case 9:
        //	if(CheckDiagonal(configuration, new Coordinates(tileCoords.X-1, tileCoords.Y+1), new Coordinates(tileCoords.X+1, tileCoords.Y-1))) {
        //		configuration = 18;
        //	}
        //	break;
        //case 12:
        //	if(CheckDiagonal(configuration, new Coordinates(tileCoords.X+1, tileCoords.Y+1), new Coordinates(tileCoords.X-1, tileCoords.Y-1))) {
        //		configuration = 19;
        //	}
        //	break;
        //}

        return configuration;
    }

    int GetConfiguration(Coordinates tileCoords, MapType mapType)
    {
        int configuration = 0;

        int up = 0;
        if (tileCoords.Y == size - 1)
        {
            up = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up).GetTileValue(new Coordinates(tileCoords.X, 0), mapType);
        }
        else
        {
            up = GetTileValue(tileCoords + Coordinates.up, mapType);
        }
        int down = 0;
        if (tileCoords.Y == 0)
        {
            down = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down).GetTileValue(new Coordinates(tileCoords.X, size - 1), mapType);
        }
        else
        {
            down = GetTileValue(tileCoords + Coordinates.down, mapType);
        }
        int left = 0;
        if (tileCoords.X == 0)
        {
            left = TerrainGenerator.Instance.GetChunk(coords + Coordinates.left).GetTileValue(new Coordinates(size - 1, tileCoords.Y), mapType);
        }
        else
        {
            left = GetTileValue(tileCoords + Coordinates.left, mapType);
        }
        int right = 0;
        if (tileCoords.X == size - 1)
        {
            right = TerrainGenerator.Instance.GetChunk(coords + Coordinates.right).GetTileValue(new Coordinates(0, tileCoords.Y), mapType);
        }
        else
        {
            right = GetTileValue(tileCoords + Coordinates.right, mapType);
        }

        if (left != 0)
        {
            configuration += 8;
        }
        if (up != 0)
        {
            configuration += 4;
        }
        if (right != 0)
        {
            configuration += 2;
        }
        if (down != 0)
        {
            configuration += 1;
        }

        if (configuration == 15)
        {
            if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X + 1, tileCoords.Y + 1), new Coordinates(tileCoords.X + 1, tileCoords.Y + 1), mapType))
            {
                configuration = 16;
            }
            else if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X + -1, tileCoords.Y + 1), new Coordinates(tileCoords.X - 1, tileCoords.Y + 1), mapType))
            {
                configuration = 17;
            }
            else if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X - 1, tileCoords.Y - 1), new Coordinates(tileCoords.X - 1, tileCoords.Y - 1), mapType))
            {
                configuration = 18;
            }
            else if (!CheckDiagonal(configuration, new Coordinates(tileCoords.X + 1, tileCoords.Y - 1), new Coordinates(tileCoords.X + 1, tileCoords.Y - 1), mapType))
            {
                configuration = 19;
            }
        }

        //switch (configuration) {
        //case 3:
        //  if(CheckDiagonal(configuration, new Coordinates(tileCoords.X+1, tileCoords.Y+1), new Coordinates(tileCoords.X-1, tileCoords.Y-1))) {
        //      configuration = 16;
        //  }
        //  break;
        //case 6:
        //  if(CheckDiagonal(configuration, new Coordinates(tileCoords.X-1, tileCoords.Y+1), new Coordinates(tileCoords.X+1, tileCoords.Y-1))) {
        //      configuration = 17;
        //  }
        //  break;
        //case 9:
        //  if(CheckDiagonal(configuration, new Coordinates(tileCoords.X-1, tileCoords.Y+1), new Coordinates(tileCoords.X+1, tileCoords.Y-1))) {
        //      configuration = 18;
        //  }
        //  break;
        //case 12:
        //  if(CheckDiagonal(configuration, new Coordinates(tileCoords.X+1, tileCoords.Y+1), new Coordinates(tileCoords.X-1, tileCoords.Y-1))) {
        //      configuration = 19;
        //  }
        //  break;
        //}

        return configuration;
    }

    int GetQuickConfiguration(Coordinates tileCoords)
    {
        int configuration = 0;

        int up = 0;
        if (tileCoords.Y == size - 1)
        {
            up = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up).GetTileValue(new Coordinates(tileCoords.X, 0));
        }
        else
        {
            up = GetTileValue(tileCoords + Coordinates.up);
        }
        int down = 0;
        if (tileCoords.Y == 0)
        {
            down = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down).GetTileValue(new Coordinates(tileCoords.X, size - 1));
        }
        else
        {
            down = GetTileValue(tileCoords + Coordinates.down);
        }
        int left = 0;
        if (tileCoords.X == 0)
        {
            left = TerrainGenerator.Instance.GetChunk(coords + Coordinates.left).GetTileValue(new Coordinates(size - 1, tileCoords.Y));
        }
        else
        {
            left = GetTileValue(tileCoords + Coordinates.left);
        }
        int right = 0;
        if (tileCoords.X == size - 1)
        {
            right = TerrainGenerator.Instance.GetChunk(coords + Coordinates.right).GetTileValue(new Coordinates(0, tileCoords.Y));
        }
        else
        {
            right = GetTileValue(tileCoords + Coordinates.right);
        }

        if (left != 0)
        {
            configuration += 8;
        }
        if (up != 0)
        {
            configuration += 4;
        }
        if (right != 0)
        {
            configuration += 2;
        }
        if (down != 0)
        {
            configuration += 1;
        }

        return configuration;
    }

    Vector2[] GenerateEdgeCollider2DPoints(int configuration)
    {
        List<Vector2> points = new List<Vector2>();

        Vector2 topLeft = new Vector2(-0.5f, 0.5f);
        Vector2 topRight = new Vector2(0.5f, 0.5f);
        Vector2 bottomRight = new Vector2(0.5f, -0.5f);
        Vector2 bottomLeft = new Vector2(-0.5f, -0.5f);

        switch (configuration)
        {
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
            case 16:
                points.Add(bottomLeft);
                //points.Add(topLeft);
                points.Add(topRight);
                break;
            case 17:
                points.Add(topLeft);
                // points.Add(bottomLeft);
                points.Add(bottomRight);
                break;
            case 18:
                points.Add(topLeft);
                // points.Add(topRight);
                points.Add(bottomRight);
                break;
            case 19:
                points.Add(topRight);
                // points.Add(bottomRight);
                points.Add(bottomLeft);
                break;
        }
        return points.ToArray();
    }

    void OnMapDataRecieved(MapData _mapData)
    {
        mapData = _mapData;
        debugGameObject.GetComponent<MeshRenderer>().material.mainTexture = TextureGenerator.GenerateTexture(_mapData.cellMap, _mapData.groundMap, _mapData.airMap, _mapData.waterMap);
        mapDataRecieved = true;
    }

    bool CheckDiagonal(int configuration, Coordinates coords1, Coordinates coords2)
    {
        Coordinates correctCoords = new Coordinates(((coords1.X < 0) ? size - 1 : ((coords1.X >= size) ? 0 : coords1.X)), ((coords1.Y < 0) ? size - 1 : ((coords1.Y >= size) ? 0 : coords1.Y)));
        int conf1 = GetChunk(coords1).GetTileValue(correctCoords);
        correctCoords = new Coordinates(((coords2.X < 0) ? size - 1 : ((coords2.X >= size) ? 0 : coords2.X)), ((coords2.Y < 0) ? size - 1 : ((coords2.Y >= size) ? 0 : coords2.Y)));
        int conf2 = GetChunk(coords2).GetTileValue(correctCoords);
        return conf1 != 0 || conf2 != 0;
    }

    bool CheckDiagonal(int configuration, Coordinates coords1, Coordinates coords2, MapType mapType)
    {
        Coordinates correctCoords = new Coordinates(((coords1.X < 0) ? size - 1 : ((coords1.X >= size) ? 0 : coords1.X)), ((coords1.Y < 0) ? size - 1 : ((coords1.Y >= size) ? 0 : coords1.Y)));
        int conf1 = GetChunk(coords1).GetTileValue(correctCoords, mapType);
        correctCoords = new Coordinates(((coords2.X < 0) ? size - 1 : ((coords2.X >= size) ? 0 : coords2.X)), ((coords2.Y < 0) ? size - 1 : ((coords2.Y >= size) ? 0 : coords2.Y)));
        int conf2 = GetChunk(coords2).GetTileValue(correctCoords, mapType);
        return conf1 != 0 || conf2 != 0;
    }

    private Chunk GetChunk(Coordinates tileCoords)
    {
        if (tileCoords.X < 0)
        {
            if (tileCoords.Y < 0)
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.down + Coordinates.left);
            }
            else if (tileCoords.Y >= size)
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.up + Coordinates.left);
            }
            else
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.left);
            }
        }
        if (tileCoords.X >= size)
        {
            if (tileCoords.Y < 0)
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.down + Coordinates.right);
            }
            else if (tileCoords.Y >= size)
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.up + Coordinates.right);
            }
            else
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.right);
            }
        }
        else
        {
            if (tileCoords.Y < 0)
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.down);
            }
            else if (tileCoords.Y >= size)
            {
                return TerrainGenerator.Instance.GetChunk(coords + Coordinates.up);
            }
            else
            {
                return this;
            }
        }
    }
}



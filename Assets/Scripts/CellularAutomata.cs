using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class CellularAutomata {

	//	VARS

		//	PUBLIC

		//	PRIVATE

	//	GETTERS


	//	FACTORY

	//	METHODS

		//	PUBLIC

	public static int[,] GenerateCellularAutomata(string seed, Coordinates coords, int width, int height, int fillPercentage, Vector2 bxsy) {

		int[,] cellMap = new int[width,height];

		System.Random pseudoRandom = new System.Random(seed.GetHashCode() + coords.X + coords.Y * 255);


		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				if(i == 0 || j == 0 || i == cellMap.GetLength(0)-1 || j == cellMap.GetLength(1)-1){
					cellMap[i,j] = 1;
				}
				else{
					cellMap[i,j] = (pseudoRandom.Next(0,100) < fillPercentage - coords.Y) ? 1 : 0;
				}
			}
		}



		for(int i = 0; i < 7; i++){
			if((coords.X + coords.Y) % 2 == 0) {
				IterateMap(cellMap, bxsy);
				// IterateMapWithBorder(cellMap, bxsy);
			}
			else{
				IterateMap(cellMap, coords, bxsy);
			}
		}

        for (int i = 0; i < cellMap.GetLength(0); i++)
        {
            for (int j = 0; j < cellMap.GetLength(1); j++)
            {
                int liveNeighbours = GetLiveNeighbours(cellMap, i, j);
                if(liveNeighbours <= 3){
                    cellMap[i, j] = 0;
                }
            }
        }

		return cellMap;
	}



		//	PRIVATE

	static void IterateMap(int[,] cellMap, Coordinates coords, Vector2 bxsy) {
		for(int i = 0; i < cellMap.GetLength(0); i++) {
			for(int j = 0; j < cellMap.GetLength(1); j++) {
				if(i == 0 || j == 0 || i == cellMap.GetLength(0)-1 || j == cellMap.GetLength(1)-1){
					cellMap[i,j] = DealWithEdge(cellMap, coords, i, j);
					continue;
				}
				
				int liveNeighbours = GetLiveNeighbours(cellMap, i, j);

				if(liveNeighbours > bxsy.x) {
					cellMap[i,j] = 1;
				}

				else if(liveNeighbours < bxsy.y) {
					cellMap[i,j] = 0;
				}

			}
		}
	}

	static void IterateMapWithBorder(int[,] cellMap, Vector2 bxsy) {
		for(int i = 0; i < cellMap.GetLength(0); i++) {
			for(int j = 0; j < cellMap.GetLength(1); j++) {
				
				int liveNeighbours = GetLiveNeighboursWithBorder(cellMap, i, j);

				if(liveNeighbours > bxsy.x) {
					cellMap[i,j] = 1;
				}

				else if(liveNeighbours < bxsy.y) {
					cellMap[i,j] = 0;
				}

			}
		}
	}

	static void IterateMap(int[,] cellMap, Vector2 bxsy) {
		for(int i = 0; i < cellMap.GetLength(0); i++) {
			for(int j = 0; j < cellMap.GetLength(1); j++) {
				
				if(i == 0 && j == 0){
					cellMap[i,j] = 1;
					continue;
				}
				else if(i == 0 && j == cellMap.GetLength(1)-1){
					cellMap[i,j] = 1;
					continue;
				}
				else if(i == cellMap.GetLength(0)-1 && j == 0){
					cellMap[i,j] = 1;
					continue;
				}
				else if(i == cellMap.GetLength(0)-1 && j == cellMap.GetLength(1)-1){
					cellMap[i,j] = 1;
					continue;
				}

				int liveNeighbours = GetLiveNeighbours(cellMap, i, j);

				if(liveNeighbours > bxsy.x) {
					cellMap[i,j] = 1;
				}

				else if(liveNeighbours < bxsy.y) {
					cellMap[i,j] = 0;
				}

			}
		}
	}

	static int GetLiveNeighboursWithBorder(int[,] cellMap, int i, int j) {
		if(i == 0 || i == cellMap.GetLength(0)-1 || j == 0 || j == cellMap.GetLength(1)-1){
			return 8;
		}
		else{
			return cellMap[i-1,j-1] + cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i-1,j] + cellMap[i+1,j] + cellMap[i-1,j+1] + cellMap[i,j+1] + cellMap[i+1,j+1];
		}
	}

	static int GetLiveNeighbours(int[,] cellMap, int i, int j) {
		int liveNeighbours = 0;

		if(i == 0) {
			if(j == 0) {
				liveNeighbours = cellMap[i+1,j] + cellMap[i,j+1] + cellMap[i+1,j+1] + 1;
			}
			else if (j == cellMap.GetLength(1)-1){
				liveNeighbours = cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i+1,j] + 1;
			}
			else {
				liveNeighbours = cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i+1,j] + cellMap[i,j+1] + cellMap[i+1,j+1] + 1;
			}
		}
		else if(i == cellMap.GetLength(0)-1) {
			if(j == 0) {
				liveNeighbours = cellMap[i-1,j] + cellMap[i,j+1] + cellMap[i-1,j+1] + 1;
			}
			else if (j == cellMap.GetLength(1)-1){
				liveNeighbours = cellMap[i,j-1] + cellMap[i-1,j-1] + cellMap[i-1,j] + 1;
			}
			else {
				liveNeighbours = cellMap[i,j-1] + cellMap[i-1,j-1] + cellMap[i-1,j] + cellMap[i,j+1] + cellMap[i-1,j+1] + 1;
			}
		}

		else {
			if(j == 0) {
				liveNeighbours = cellMap[i-1,j] + cellMap[i+1,j] + cellMap[i-1,j+1] + cellMap[i,j+1] + cellMap[i+1,j+1] + 1;
			}
			else if (j == cellMap.GetLength(1)-1){
				liveNeighbours = cellMap[i-1,j-1] + cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i-1,j] + cellMap[i+1,j] + 1;
			}
			else{
				liveNeighbours = cellMap[i-1,j-1] + cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i-1,j] + cellMap[i+1,j] + cellMap[i-1,j+1] + cellMap[i,j+1] + cellMap[i+1,j+1];
			}
		}

		return liveNeighbours;
	}

	static int GetLiveNeighbours(int[,] cellMap, Coordinates coords, int i, int j) {
		int liveNeighbours = 0;

		int[,] up = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up).mapData.cellMap;
		int[,] down = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down).mapData.cellMap;
		int[,] left = TerrainGenerator.Instance.GetChunk(coords + Coordinates.left).mapData.cellMap;
		int[,] right = TerrainGenerator.Instance.GetChunk(coords + Coordinates.right).mapData.cellMap;

		if(i == 0) {
			if(j == 0) {
				liveNeighbours = cellMap[i+1,j] + cellMap[i,j+1] + cellMap[i+1,j+1] + left[cellMap.GetLength(0)-1,j] + left[cellMap.GetLength(0)-1,j+1] + down[i,cellMap.GetLength(1)-1] + down[i+1,cellMap.GetLength(1)-1];
			}
			else if (j == cellMap.GetLength(1)-1){
				liveNeighbours = cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i+1,j] + left[cellMap.GetLength(0)-1,j] + left[cellMap.GetLength(0)-1,j-1] + up[i,0] + up[i+1,0];
			}
			else {
				liveNeighbours = cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i+1,j] + cellMap[i,j+1] + cellMap[i+1,j+1] + left[cellMap.GetLength(0)-1,j] + left[cellMap.GetLength(0)-1,j-1] + left[cellMap.GetLength(0)-1,j+1];
				// liveNeighbours = cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i+1,j] + cellMap[i,j+1] + cellMap[i+1,j+1] + (left[cellMap.GetLength(0)-1,j]);
			}
		}
		else if(i == cellMap.GetLength(0)-1) {
			if(j == 0) {
				liveNeighbours = cellMap[i-1,j] + cellMap[i,j+1] + cellMap[i-1,j+1] + right[0,j] + right[0,j+1] + down[i,cellMap.GetLength(1)-1] + down[i-1,cellMap.GetLength(1)-1];
			}
			else if (j == cellMap.GetLength(1)-1){
				liveNeighbours = cellMap[i,j-1] + cellMap[i-1,j-1] + cellMap[i-1,j] + right[0,j] + right[0,j-1] + up[i,0] + up[i-1,0];
			}
			else {
				liveNeighbours = cellMap[i,j-1] + cellMap[i-1,j-1] + cellMap[i-1,j] + cellMap[i,j+1] + cellMap[i-1,j+1] + right[0,j] + right[0,j-1] + right[0,j+1];
				// liveNeighbours = cellMap[i,j-1] + cellMap[i-1,j-1] + cellMap[i-1,j] + cellMap[i,j+1] + cellMap[i-1,j+1] + (right[0,j]);
			}
		}

		else {
			if(j == 0) {
				liveNeighbours = cellMap[i-1,j] + cellMap[i+1,j] + cellMap[i-1,j+1] + cellMap[i,j+1] + cellMap[i+1,j+1] + down[i,cellMap.GetLength(1)-1] + down[i+1,cellMap.GetLength(1)-1] + down[i-1,cellMap.GetLength(1)-1];
				// liveNeighbours = cellMap[i-1,j] + cellMap[i+1,j] + cellMap[i-1,j+1] + cellMap[i,j+1] + cellMap[i+1,j+1] + (down[i,cellMap.GetLength(1)-1]);
			}
			else if (j == cellMap.GetLength(1)-1){
				liveNeighbours = cellMap[i-1,j-1] + cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i-1,j] + cellMap[i+1,j] + up[i,0] + up[i-1,0] + up[i+1,0];
				// liveNeighbours = cellMap[i-1,j-1] + cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i-1,j] + cellMap[i+1,j] + (up[i,0]);
			}
			else{
				liveNeighbours = cellMap[i-1,j-1] + cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i-1,j] + cellMap[i+1,j] + cellMap[i-1,j+1] + cellMap[i,j+1] + cellMap[i+1,j+1];
			}
		}
		return liveNeighbours;
	}

	static int DealWithEdge(int[,] cellMap, Coordinates coords, int i, int j){
		int[,] up = TerrainGenerator.Instance.GetChunk(coords + Coordinates.up).mapData.cellMap;
		int[,] down = TerrainGenerator.Instance.GetChunk(coords + Coordinates.down).mapData.cellMap;
		int[,] left = TerrainGenerator.Instance.GetChunk(coords + Coordinates.left).mapData.cellMap;
		int[,] right = TerrainGenerator.Instance.GetChunk(coords + Coordinates.right).mapData.cellMap;

		if(i == 0) {
			if(j == 0) {
				return (int) Mathf.Clamp01(down[0,cellMap.GetLength(1)-1] + left[cellMap.GetLength(0)-1,0]);
			}
			else if (j == cellMap.GetLength(1)-1){
				return (int) Mathf.Clamp01(up[0,0] + left[cellMap.GetLength(0)-1,cellMap.GetLength(1)-1]);
			}
			else {
				return left[cellMap.GetLength(0)-1,j];
			}
		}
		else if(i == cellMap.GetLength(0)-1) {
			if(j == 0) {
				return (int) Mathf.Clamp01(down[cellMap.GetLength(0)-1,cellMap.GetLength(1)-1] + right[0,0]);
			}
			else if (j == cellMap.GetLength(1)-1){
				return (int) Mathf.Clamp01(up[cellMap.GetLength(0)-1,0] + right[0,cellMap.GetLength(1)-1]);
			}
			else {
				return right[0,j];
			}
		}

		else {
			if(j == 0) {
				return down[i,cellMap.GetLength(1)-1];
			}
			else if (j == cellMap.GetLength(1)-1){
				return up[i,0];
			}
		}
		return 0;	
	}

	public static int GetGroundMapValue(int[,] cellMap, int x, int y) {
		int neighbours = 0;
		if(cellMap[x,y] == 1) {
			return 0;
		}
		if(x == 0) {
			if(y == 0) {
				return 0;
			} 
			else if(y == cellMap.GetLength(1)-1) {
				if(cellMap[x,y-1] == 0 || cellMap[x+1,y-1] ==0) {
					return 0;
				}
				else {
					neighbours += cellMap[x+1,y];
				}
			}
			else{
				if(cellMap[x,y-1] == 0 || cellMap[x+1,y-1] == 0) {
					return 0;
				}
				else {
					neighbours += cellMap[x+1,y];
				}
			}
		}
		else if(x == cellMap.GetLength(0)-1) {
			if(y == 0) {
				return 0;
			} 
			else if(y == cellMap.GetLength(1)-1) {
				if(cellMap[x,y-1] == 0 ||  cellMap[x-1,y-1] == 0) {
					return 0;
				}
				else {
					neighbours += cellMap[x-1,y];
				}
			}
			else{
				if(cellMap[x,y-1] == 0 ||  cellMap[x-1,y-1] == 0) {
					return 0;
				}
				else {
					neighbours += cellMap[x-1,y];
				}
			}
		}
		else {
			if(y == 0) {
				return 0;
			} 
			else if(y == cellMap.GetLength(1)-1) {
				if(cellMap[x,y-1] == 0 ||  cellMap[x+1,y-1] == 0 ||  cellMap[x-1,y-1] == 0) {
					return 0;
				}
				else {
					neighbours += cellMap[x-1,y] + cellMap[x+1,y];
				}
			}
			else{
				if(cellMap[x,y-1] == 0 || cellMap[x+1,y-1] == 0 ||  cellMap[x-1,y-1] == 0) {
					return 0;
				}
				else {
					// Debug.Log(x + "|" + y);
					neighbours += cellMap[x+1,y] + cellMap[x-1,y];
				}
			}
		}
		if(neighbours == 0) {
			 return 1;
		}
		return 0;
	}

	public static int[,] GenerateGroundMap(int[,] cellMap) {
		int[,] flatMap = new int[cellMap.GetLength(0), cellMap.GetLength(1)];
		for(int x = 0; x < cellMap.GetLength(0); x++) {
			for(int y = 0; y < cellMap.GetLength(1); y++) {
				flatMap[x,y] = GetGroundMapValue(cellMap, x,y);
			}
		}
		return flatMap;
	}

	public static int GetAriMapValue(int[,] cellMap, int x, int y, int liveCount) {
        return (GetLiveNeighbours(cellMap, x, y) > liveCount)? 1:0;
	}

	public static int[,] GenerateAirMap(int[,] cellMap) {

		int[,] airMap = new int[cellMap.GetLength(0), cellMap.GetLength(1)];
		for(int x = 0; x < cellMap.GetLength(0); x++) {
			for(int y = 0; y < cellMap.GetLength(1); y++) {
				airMap[x,y] = GetAriMapValue(cellMap, x, y, 1);
            }
		}
        return airMap;
	}


    public static int[,] GenerateCaveMap(int[,] cellMap)
    {

        int[,] airMap = new int[cellMap.GetLength(0), cellMap.GetLength(1)];
        for (int x = 0; x < cellMap.GetLength(0); x++)
        {
            for (int y = 0; y < cellMap.GetLength(1); y++)
            {
                airMap[x, y] = GetAriMapValue(cellMap, x, y, 1);
            }
        }

        int[,] tempMap = (int[,])airMap.Clone();
        for (int x = 0; x < tempMap.GetLength(0); x++)
        {
            for (int y = 0; y < tempMap.GetLength(1); y++)
            {
                airMap[x, y] = GetAriMapValue(tempMap, x, y, 1);
            }

        }
        for (int i = 0; i < 20; i++)
        {
            tempMap = (int[,])airMap.Clone();
            for (int x = 0; x < tempMap.GetLength(0); x++)
            {
                for (int y = 0; y < tempMap.GetLength(1); y++)
                {
                    airMap[x, y] = GetAriMapValue(tempMap, x, y, 4);
                    if(cellMap[x,y] == 1){
                        airMap[x, y] = 1;
                    }
                }

            }
        }
        return airMap;
    }

    //public static int [,] GenerateWaterMap (int [,] cellMap)
    //{
    //	int [,] waterMap = new int [cellMap.GetLength (0), cellMap.GetLength (1)];
    //	for (int x = 0; x < cellMap.GetLength (0); x++) {
    //		for (int y = 0; y < cellMap.GetLength (1); y++) {
    //			waterMap [x, y] = GetWaterMapValue (cellMap, x, y);
    //		}
    //	}
    //	return waterMap;
    //}

    // static int GetLiveNeighbours(int[,] cellMap, Vector2 coords, int i, int j) {
    // 	int liveNeighbours = 0;

    // 	int[,] up = TerrainGenerator.Instance.GetChunk(coords + Vector2.down).mapData.cellMap;
    // 	int[,] down = TerrainGenerator.Instance.GetChunk(coords + Vector2.up).mapData.cellMap;
    // 	int[,] left = TerrainGenerator.Instance.GetChunk(coords + Vector2.right).mapData.cellMap;
    // 	int[,] right = TerrainGenerator.Instance.GetChunk(coords + Vector2.left).mapData.cellMap;

    // 	if(i == 0) {
    // 		if(j == 0) {
    // 			return (down[0,cellMap.GetLength(1)-1] + left[cellMap.GetLength(0)-1,0]) * 10;
    // 		}
    // 		else if (j == cellMap.GetLength(1)-1){
    // 			return (up[0,0] + left[cellMap.GetLength(0)-1,cellMap.GetLength(1)-1]) * 10;
    // 		}
    // 		else {
    // 			return left[cellMap.GetLength(0)-1,j] * 10;
    // 		}
    // 	}
    // 	else if(i == cellMap.GetLength(0)-1) {
    // 		if(j == 0) {
    // 			return (down[cellMap.GetLength(0)-1,cellMap.GetLength(1)-1] + right[0,0]) * 10;
    // 		}
    // 		else if (j == cellMap.GetLength(1)-1){
    // 			return (up[cellMap.GetLength(0)-1,0] + right[0,cellMap.GetLength(1)-1]) * 10;
    // 		}
    // 		else {
    // 			return right[0,j] * 10;
    // 		}
    // 	}

    // 	else {
    // 		if(j == 0) {
    // 			return down[i,cellMap.GetLength(1)-1] * 10;
    // 		}
    // 		else if (j == cellMap.GetLength(1)-1){
    // 			return up[i,0] * 10;
    // 		}
    // 		else{
    // 			liveNeighbours = cellMap[i-1,j-1] + cellMap[i,j-1] + cellMap[i+1,j-1] + cellMap[i-1,j] + cellMap[i+1,j] + cellMap[i-1,j+1] + cellMap[i,j+1] + cellMap[i+1,j+1];
    // 		}
    // 	}
    // 	return liveNeighbours;
    // }
}





using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAutomata
{

	public static int [,] GenerateWaterMap (int [,] cellMap)
	{
		int [,] waterMap = new int [cellMap.GetLength (0), cellMap.GetLength (1)];
		int [,] flatMap = CellularAutomata.GenerateGroundMap (cellMap);

		for (int i = 0; i < cellMap.GetLength (0); i++) {
			for (int j = 0; j < cellMap.GetLength (1); j++) {
				//waterMap [i, j] = (cellularAutomata [i, j]==1)?0:1;
			}
		}

		for (int i = 0; i < 5; i++) {

		}



		return waterMap;
	}
}

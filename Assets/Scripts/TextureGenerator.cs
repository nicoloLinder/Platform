using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour {

	//	VARS

		//	PUBLIC

		//	PRIVATE

	//	GETTERS

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR
	
	//	METHODS

		//	PUBLIC

	public static Texture2D GenerateTexture(int[,] cellMap) {
		int width = cellMap.GetLength (0);
		int height = cellMap.GetLength (1);

		Texture2D texture = new Texture2D (width, height);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = (cellMap [x, y] == 0)?Color.white : Color.black;
			}
		}
		texture.SetPixels (colourMap);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply ();

		return texture;
	}

	public static Texture2D GenerateTexture(int[,] cellMap, int[,] flatMap) {
		int width = cellMap.GetLength (0);
		int height = cellMap.GetLength (1);

		Texture2D texture = new Texture2D (width, height);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = (cellMap [x, y] == 1)?Color.black : ((flatMap[x,y] == 1)? Color.red : Color.white);
			}
		}
		texture.SetPixels (colourMap);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply ();

		return texture;
	}

	public static Texture2D GenerateTexture(int[,] cellMap, int[,] flatMap, int[,] airMap, int[,]waterMap) {
		int width = cellMap.GetLength (0);
		int height = cellMap.GetLength (1);

		Texture2D texture = new Texture2D (width, height);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				colourMap [y * width + x] = (cellMap [x, y] == 1)?Color.black : ((flatMap[x,y] == 1)? Color.red : ((airMap[x,y] == 1)? Color.blue : Color.white));
				colourMap [y * width + x] = (waterMap [x, y] == 1) ? Color.green : colourMap [y * width + x];
			}
		}
		texture.SetPixels (colourMap);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply ();

		return texture;
	}

		//	PRIVATE
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	//	VARS

		//	PUBLIC

	
	public Coordinates Coords { get { return coords; } }
	public Chunk ParentChunk { get { return parentChunk; } }

		//	PRIVATE

	private Coordinates coords;
	private Chunk parentChunk;

	//	GETTERS

	//	SETTERS

	//	FACTORY

	public void SetupTile(Coordinates _coords, Chunk _parentChunk) {
		coords = _coords;
		parentChunk = _parentChunk;
	}

	//	MONOBEHAIVIOUR

	//	METHODS

		//	PUBLIC

		//	PRIVATE
}

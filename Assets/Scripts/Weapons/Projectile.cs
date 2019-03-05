	// void RemoveTile(Tile tile) {
	// 	TerrainGenerator.Instance.RemoveTile(tile);
	// }

	// void AddTile(Tile tile, Vector3 direction) {
	// 	if(tile == null){
	// 		return;
	// 	}
	// 	Coordinates coords = tile.Coords + (Coordinates) direction.normalized;
	// 	TerrainGenerator.Instance.AddTile(tile, coords);
	// }

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	//	VARS

		//	PUBLIC

		//	PRIVATE

	private event Action<GameObject, GameObject> callback;
	int probability = 1;

	//	GETTERS

	//	SETTERS

	//	FACTORY

	public void Initialize(Action<GameObject, GameObject> _callback) {
		callback = _callback;
		GetComponent<Rigidbody2D>().AddTorque(UnityEngine.Random.Range(360,720) * Mathf.Sign(UnityEngine.Random.Range(-1,2)));
	}

	//	MONOBEHAIVIOUR

	void OnTriggerEnter2D(Collider2D other){
		callback(other.gameObject, gameObject);
		if(UnityEngine.Random.Range(0,101) < probability) {
			if(other.gameObject.tag == "Tile") {
				Destroy(gameObject);
			}
		}
		else{
			probability++;
		}
	}

	//	METHODS

		//	PUBLIC

	public void KillBullet () {
		Destroy(gameObject);
	}	

		//	PRIVATE
}

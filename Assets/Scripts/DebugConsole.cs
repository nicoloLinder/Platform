using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour {

	public Transform player;

	public Text playerPosition;
	public Text currentChunk;

	
	// Update is called once per frame
	void Update () {
		playerPosition.text = "Pos: "+((Vector2)player.position).ToString();
		currentChunk.text = "Chunk: "+(new Vector2((int)(player.position.x+((player.position.x>0)?16:-16))/32,(int)(player.position.y+((player.position.y>0)?16:-16))/32)).ToString();


	}
}

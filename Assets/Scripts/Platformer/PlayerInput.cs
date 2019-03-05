using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour {

	//	VARS

		//	PUBLIC

		//	PRIVATE

	Player player;

	//	GETTERS

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR

	void Start () {
		player = GetComponent<Player>();
		// Cursor.visible = false;
	}
	
	void Update () {
		Vector2 directionalInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxis ("Vertical"));
		player.SetDirectionalInput(directionalInput);
		if(Input.GetButtonDown("Jump")){
			player.OnJumpInputDown();
		}
		if(Input.GetButtonUp("Jump")){
			player.OnJumpInputUp();
		}
        if(Input.GetKeyDown(KeyCode.X)){
            ScreenCapture.CaptureScreenshot(System.DateTime.Now.ToString("F")+".png");
        }
	}

	//	METHODS

		//	PUBLIC

		//	PRIVATE
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour {

	public float speed = 5;
	public float jumpForce = 9.81f;
	public Transform bullet;
	public Transform cameraContainer;
	public ParticleSystem shells;

	public float rateOfFire;
	public float recoil;

	Vector3 offset;
	Rigidbody2D rb2d;
	bool grounded;
	float delay;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		offset = cameraContainer.position - transform.position;
	}
	
	// Update is called once per frame

	void Update() {
		delay-= Time.deltaTime;
		if(Input.GetMouseButton(0) && delay < 0){
			shoot(false);
		}
		if(Input.GetMouseButton(1) && delay < 0){
			shoot(true);
		}
	}

	void FixedUpdate () {
		rb2d.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb2d.velocity.y);
		if(Input.GetAxis("Vertical") != 0 && grounded){
			rb2d.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, 0);
			rb2d.AddForce(Vector3.up * jumpForce);
		}
		// else if(Input.GetButton("Jump")){
		// 	// rb2d.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, 0);
		// 	rb2d.AddForce(Vector3.up * 20);
		// 	if(rb2d.velocity.y > 10){
		// 		rb2d.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, 10);
		// 	}
		// }
		cameraContainer.position = Vector3.Lerp(cameraContainer.position,offset + transform.position /*+ new Vector3(Input.GetAxis("Horizontal")*2.5f,0,0)*/,0.1f);
		
	}

	void OnCollisionEnter2D(Collision2D collisionInfo){
		grounded = true;
	}


	void OnCollisionExit2D(Collision2D collisionInfo){
		grounded = false;
	}

	void shoot(bool build) {
	// 	Vector2 shootDirection = new Vector2(Input.mousePosition.x - Screen.width/2,Input.mousePosition.y - Screen.height/2).normalized;
	// 	Vector3 shootDirectionNormal = Vector3.Cross(shootDirection, Vector3.forward);
	// 	// shootDirection = new Vector2(Mathf.Floor(shootDirection.x*5)/5, Mathf.Floor(shootDirection.y*5)/5);
	// 	Instantiate(bullet, transform.position + (Vector3)shootDirection, Quaternion.identity).GetComponent<Bullet>().Shoot(shootDirection, 20, build);
	// 	// Instantiate(bullet, transform.position + (Vector3)shootDirection, Quaternion.identity).GetComponent<Bullet>().Shoot(shootDirection + (Vector2)shootDirectionNormal/5, 20);
	// 	// Instantiate(bullet, transform.position + (Vector3)shootDirection, Quaternion.identity).GetComponent<Bullet>().Shoot(shootDirection - (Vector2)shootDirectionNormal/5, 20);
	// 	// Instantiate(bullet, transform.position + (Vector3)shootDirection, Quaternion.identity).GetComponent<Bullet>().Shoot(shootDirection + (Vector2)shootDirectionNormal/2, 20);
	// 	// Instantiate(bullet, transform.position + (Vector3)shootDirection, Quaternion.identity).GetComponent<Bullet>().Shoot(shootDirection - (Vector2)shootDirectionNormal/2, 20);
	// 	// Instantiate(bullet, transform.position + (Vector3)shootDirection, Quaternion.identity).GetComponent<Bullet>().Shoot(shootDirection + (Vector2)shootDirectionNormal, 20);
	// 	// Instantiate(bullet, transform.position + (Vector3)shootDirection, Quaternion.identity).GetComponent<Bullet>().Shoot(shootDirection - (Vector2)shootDirectionNormal, 20);
	// 	rb2d.AddForce(new Vector2((shootDirection.x>0)?-1:1,0) * recoil);
	// 	// cameraContainer.position -= (Vector3)shootDirection.normalized/10;
	// 	delay = rateOfFire;
	// 	shells.Emit(1);
	// 	float angle = Vector2.Angle(Vector2.left,shootDirection);
	// 	var shape = shells.shape;
	// 	shape.rotation = new Vector3((shootDirection.x>0)?205:-25,90,0);
	// 	shape.position = new Vector3((shootDirection.x>0)?-1:1, 0,0);
	// 	// CameraShake.shakeDuration = 0.1f;
	}
}

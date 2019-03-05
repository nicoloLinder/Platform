using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {

	//	VARS

		//	PUBLIC

	public ParticleSystem shells;

	public Player player;

		//	PRIVATE

    [SerializeField] private BaseWeapon weapon1;
    [SerializeField] private BaseWeapon weapon2;
    private float coolDown;

	//	GETTERS

	//	SETTERS

	//	FACTORY

	//	MONOBEHAIVIOUR

	void Start () {
		weapon1.Initialize (this);
		weapon2.Initialize (this);
	}
	
	void Update () {
		 //if(Input.GetKeyDown(KeyCode.X)){
		 //	BaseWeapon temp = weapon2;
		 //	weapon2 = weapon1;
		 //	weapon1 = temp;
		 //	coolDown = 0;
		 //}
		if(Input.GetMouseButton(0) && coolDown < 0) {
			weapon1.Trigger();
			coolDown = weapon1.coolDown;
		}
		coolDown -= Time.deltaTime;
	}

	void OnDrawGizmos() {
		// if(Input.GetMouseButton(0)) {
			Gizmos.color = Color.red;
			Gizmos.DrawRay(transform.position,new Vector2(Input.mousePosition.x - Screen.width/2,Input.mousePosition.y - Screen.height/2).normalized);
		// }
	}

	//	METHODS

		//	PUBLIC

	 public void Shoot(Rigidbody2D projectile, float projectileForce, float accuracy, Action<GameObject, GameObject> callback, float recoil) {
        Vector2 shootDirection =  new Vector2(Input.mousePosition.x - Screen.width/2,Input.mousePosition.y - Screen.height/2);
        Vector2 randomNormal = (Vector2)Vector3.Cross(shootDirection, Vector3.forward) * UnityEngine.Random.Range(-accuracy/100f,accuracy/100f);
        // Vector2 normal = (Vector2)Vector3.Cross(shootDirection, Vector3.forward);

        shootDirection += randomNormal;
        shootDirection.Normalize();

        float angle = Vector2.Angle(Vector2.right,shootDirection);
        if(shootDirection.y < 0){
            angle = -angle ;
        }

        //Instantiate a copy of our projectile and store it in a new rigidbody variable called clonedBullet
        Rigidbody2D clonedBullet = Instantiate(projectile, transform.position + (Vector3)shootDirection, Quaternion.identity) as Rigidbody2D;
        clonedBullet.transform.Rotate(Vector3.forward, angle);
        
        //Add force to the instantiated bullet, pushing it forward away from the bulletSpawn location, using projectile force for how hard to push it away
        clonedBullet.AddForce(shootDirection * projectileForce);
        clonedBullet.gameObject.GetComponent<Projectile>().Initialize(callback);
        player.AddForce(-recoil, new Vector2(Input.mousePosition.x - Screen.width/2,Input.mousePosition.y - Screen.height/2).normalized);
        SpawnShell(shootDirection);
    }

    public void SpreadShoot(Rigidbody2D projectile, float projectileForce, float accuracy, Action<GameObject, GameObject> callback, float recoil, int bulletCount, float range) {
        Vector2 shootDirection = new Vector2(Input.mousePosition.x - Screen.width/2,Input.mousePosition.y - Screen.height/2).normalized;
        Vector2 normal = (Vector2)Vector3.Cross(shootDirection, Vector3.forward);

        shootDirection -= normal * Mathf.Sin(range * Mathf.Deg2Rad);
        shootDirection.Normalize();
        Vector2 bulletDistance = (normal * (Mathf.Sin(range * Mathf.Deg2Rad) * 2))/bulletCount;
        Vector2 temp = shootDirection;
        float angle = 0;

        for(int i = -bulletCount/2; i <= bulletCount; i++) {
        	temp = shootDirection + (Vector2)Vector3.Cross(shootDirection, Vector3.forward) * UnityEngine.Random.Range(-accuracy/100f,accuracy/100f);
        	temp.Normalize();
        	
        	angle = Vector2.Angle(Vector2.right,temp);
	        if(temp.y < 0){
	            angle = -angle ;
	        }
	        //Instantiate a copy of our projectile and store it in a new rigidbody variable called clonedBullet
	        Rigidbody2D clonedBullet = Instantiate(projectile, transform.position + (Vector3)temp, Quaternion.identity) as Rigidbody2D;
	        clonedBullet.transform.Rotate(Vector3.forward, angle);
	        
	        //Add force to the instantiated bullet, pushing it forward away from the bulletSpawn location, using projectile force for how hard to push it away
	        clonedBullet.AddForce(temp * projectileForce);
	        clonedBullet.gameObject.GetComponent<Projectile>().Initialize(callback);

	        shootDirection += bulletDistance; 
	        shootDirection.Normalize();
        }
        player.AddForce(-recoil, new Vector2(Input.mousePosition.x - Screen.width/2,Input.mousePosition.y - Screen.height/2).normalized);
        // SpawnShell(shootDirection);
    }

		//	PRIVATE

    void SpawnShell(Vector2 direction) {
		var shape = shells.shape;
		shape.rotation = new Vector3((direction.x>0)?205:-25,90,0);
		shape.position = new Vector3((direction.x>0)?-0.8f:0.8f, 0,0);
    	shells.Emit(1);
    } 
}
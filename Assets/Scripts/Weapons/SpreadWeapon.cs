using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu (menuName = "Weapons/SpreadWeapon")]
public class SpreadWeapon : BaseWeapon {

    public float projectileForce = 500f;
    public float accuracy        = 10f;
    public int bulletCount = 5;
    public float range = 45;
    public Rigidbody2D projectile;
    public bool destroy;

    public Action<GameObject,GameObject> action;

    public override void Initialize(WeaponManager _weaponManager){
        weaponManager = _weaponManager;
    }

    public override void Trigger(){
        if(destroy){
            weaponManager.SpreadShoot(projectile, projectileForce, accuracy, DestroyCallback, recoil, bulletCount, range);
        }
        else {
            weaponManager.SpreadShoot(projectile, projectileForce, accuracy, BuildCallback, recoil, bulletCount, range);
        }
    }
}
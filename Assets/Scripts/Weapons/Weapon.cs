using UnityEngine;
using System;
using System.Collections;

[CreateAssetMenu (menuName = "Weapons/Weapon")]
public class Weapon : BaseWeapon {

    public float projectileForce = 500f;
    public float accuracy        = 10f;
    public Rigidbody2D projectile;
    public bool destroy;

    public override void Initialize(WeaponManager _weaponManager){
        weaponManager = _weaponManager;
    }

    public override void Trigger(){
        if(destroy){
            weaponManager.Shoot(projectile, projectileForce, accuracy, DestroyCallback, recoil);
        }
        else {
            weaponManager.Shoot(projectile, projectileForce, accuracy, BuildCallback, recoil);
        }
    }

    // public void BuildCallback(GameObject hit, GameObject projectile) {
    //     if(hit.gameObject.tag == tag) {
    //         Tile tile = hit.GetComponent<Tile>();
    //         if(tile == null) {
    //             return;
    //         }

    //         Coordinates coords = tile.Coords + (Coordinates)(projectile.transform.position - hit.transform.position);
    //         TerrainGenerator.Instance.AddTile(tile, coords);
    //         projectile.GetComponent<Projectile>().KillBullet();
    //     }
    // }

    //  public void DestroyCallback(GameObject hit, GameObject projectile) {
    //     if(hit.gameObject.tag == tag) {
    //         Tile tile = hit.GetComponent<Tile>();
    //         if(tile == null) {
    //             return;
    //         }
    //         TerrainGenerator.Instance.RemoveTile(tile);
    //     }
    // }

}
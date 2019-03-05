using UnityEngine;
using System.Collections;

public abstract class BaseWeapon : ScriptableObject {

    public AudioClip sound;
    public float coolDown = 1f;
    public string tag;
    public float recoil;

    protected WeaponManager weaponManager;

	public abstract void Initialize(WeaponManager _weaponManager);
    public abstract void Trigger();

    public void BuildCallback(GameObject hit, GameObject projectile) {
        if(hit.gameObject.tag == tag) {
            Tile tile = hit.GetComponent<Tile>();
            if(tile == null) {
                return;
            }

            Coordinates coords = tile.Coords + (Coordinates)((projectile.transform.position - hit.transform.position));
            TerrainGenerator.Instance.AddTile(tile, coords);
            projectile.GetComponent<Projectile>().KillBullet();
        }
    }

     public void DestroyCallback(GameObject hit, GameObject projectile) {
        if(hit.gameObject.tag == tag) {
            Tile tile = hit.GetComponent<Tile>();
            if(tile == null) {
                return;
            }
            TerrainGenerator.Instance.RemoveTile(tile);
        }
    }
}
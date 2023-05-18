using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Weapon : PickableObject{
    public GameObject weaponPrefab;
    public int bulletVelocity = 10;
    public int damage;
    public float fireRate;
    public float remainingFireRate;
    public Ammunition ammunition;
    
    private void Start()
    {
        ammunition = weaponPrefab.GetOrAddComponent<Ammunition>();
        DefineInitialState(ammunition);
    }

    protected abstract void DefineInitialState(Ammunition ammunitionToSetUp);

    protected abstract void Trigger();

    public override void PickUp(Player player)
    {
        player.AddWeapon(this);
    }

    public bool MergeAmmunition(Ammunition otherAmmunition)
    {
        return otherAmmunition.MergeAmmunition(this.ammunition);
    }
    // Fire bullet method
    public void FireBullet(Vector3 direction)
    {
        // Instantiate a new bullet from the bulletPrefab
        GameObject bulletObject = Instantiate(EventController.resourcesManager.bulletPrefab, transform.position, Quaternion.identity);

        // Get the Bullet component from the instantiated bullet object
        Bullet bullet = bulletObject.GetOrAddComponent<Bullet>();

        // Set the bullet's properties
        bullet.direction = direction.normalized;
        bullet.speed = bulletVelocity;
        bullet.damage = damage;
    }
}

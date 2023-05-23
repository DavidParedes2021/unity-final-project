using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : PickableObject{
    public GameObject weaponPrefab;
    public int bulletVelocity = 50;
    public Sprite weaponImage;
    public int damage;
    public float fireRate;
    public float remainingFireRate;
    public Ammunition ammunition;
    private Coroutine _restoreFireRateCoroutine;

    private void Awake()
    {
        CollidableObject.AttachToScript(this.gameObject,nameof(Weapon));
        if (weaponImage == null)
        {
            Debug.LogWarning("No weapon image setter for"+this);
        }
        ammunition = weaponPrefab.GetOrAddComponent<Ammunition>();
        DefineInitialState(ammunition);
    }

    protected override void Start()
    {
        base.Start();
        StartRestoreFireRate();
    }

    public void FixedUpdate()
    {
        if (fireRate >= remainingFireRate)
        {
            remainingFireRate += Time.deltaTime;
        }
    }

    protected abstract void DefineInitialState(Ammunition ammunitionToSetUp);

    public abstract void Trigger(GameObject owner,Vector3 position,Vector3 direction);

    public override void PickUp(Player player)
    {
        player.AddWeapon(this);
        gameObject.SetActive(false);
    }

    public void MergeAmmunition(Ammunition otherAmmunition)
    {
        ammunition.MergeAmmunition(otherAmmunition);
    }
    // Fire bullet method
    public void FireBullet(GameObject owner,Vector3 position, Vector3 direction)
    {
        // Instantiate a new bullet from the bulletPrefab
        GameObject bulletObject = Instantiate(EventController.ResourcesManager.bulletPrefab, position, Quaternion.identity);

        // Get the Bullet component from the instantiated bullet object
        Bullet bullet = U.GetOrAddComponent<Bullet>(bulletObject);

        // Set the bullet's properties
        bullet.speed = bulletVelocity;
        bullet.damage = damage;
        bullet.direction = direction;
        // Calculate the rotation needed to align the bullet's forward vector with the specified direction
        Quaternion rotation = Quaternion.LookRotation(direction);

        // Apply the rotation to the bullet's transform
        bullet.transform.rotation = rotation;

        bullet.StartMoving(owner);
    }
    public void StartRestoreFireRate()
    {
        if (_restoreFireRateCoroutine != null)
        {
            StopCoroutine(_restoreFireRateCoroutine);
        }
        _restoreFireRateCoroutine = StartCoroutine(RestoreFireRateCoroutine());
    }

    private IEnumerator RestoreFireRateCoroutine()
    {
        while (remainingFireRate < fireRate)
        {
            remainingFireRate += Time.deltaTime;
            yield return null;
        }

        remainingFireRate = fireRate;
    }

    public static Weapon RequireWeapon(GameObject weaponGo)
    {
        MachineGun machineGun = weaponGo.GetComponent<MachineGun>();
        if(machineGun!=null)
        {
            return machineGun;
        }
        ShotGun shotGun = weaponGo.GetComponent<ShotGun>();
        if(shotGun!=null)
        {
            return shotGun;
        }
        Gun gun = weaponGo.GetComponent<Gun>();
        if (gun != null)
        {
            return gun;
        }
        throw new Exception("The Game Object has not attached a weapon script!");
    }

    public void ReloadAmmo()
    {
        if (ammunition != null)
        {
            ammunition.Reload();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : PickableObject{
    public GameObject weaponPrefab;
    public int bulletVelocity = 10;
    public Sprite weaponImage;
    public int damage;
    public float fireRate;
    public float remainingFireRate;
    public Ammunition ammunition;
    private Coroutine _restoreFireRateCoroutine;

    private void Awake()
    {
        CollidableObject.AttachToScript(this.gameObject,nameof(Weapon));
    }

    private void Start()
    {
        if (weaponImage == null)
        {
            Debug.LogWarning("No weapon image setter for"+this);
        }
        ammunition = weaponPrefab.GetOrAddComponent<Ammunition>();
        DefineInitialState(ammunition);
    }

    public void FixedUpdate()
    {
        if (fireRate >= remainingFireRate)
        {
            remainingFireRate += Time.deltaTime;
        }
    }

    protected abstract void DefineInitialState(Ammunition ammunitionToSetUp);

    protected abstract void Trigger();

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
    public void FireBullet(Vector3 direction)
    {
        // Instantiate a new bullet from the bulletPrefab
        GameObject bulletObject = Instantiate(EventController.ResourcesManager.bulletPrefab, transform.position, Quaternion.identity);

        // Get the Bullet component from the instantiated bullet object
        Bullet bullet = bulletObject.GetOrAddComponent<Bullet>();

        // Set the bullet's properties
        bullet.direction = direction.normalized;
        bullet.speed = bulletVelocity;
        bullet.damage = damage;
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
}

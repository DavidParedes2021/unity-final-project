using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : PickableObject
{
    // Define ammunition types for different weapons
    public enum AmmunitionType
    {
        Pistol,
        Shotgun,
        MachineGun
    }

    public GameObject AmmunitionPrefab { set; get; }
    public AmmunitionType AmmoType { set; get; }
    
    private int _ammoCount;
    
    public int MaxBulletsInCannon { get; set; }

    private int _currentBulletsInCannon = 0; 

    public Ammunition(AmmunitionType ammoType, int ammoCount)
    {
        this.AmmoType = ammoType;
        this._ammoCount = ammoCount;
    }

    public override void PickUp(Player player)
    {
        player.AddAmmunition(this);
        gameObject.SetActive(false);
    }

    public bool MergeAmmunition(Ammunition otherAmmo)
    {
        if (this.AmmoType == otherAmmo.AmmoType)
        {
            this._ammoCount += otherAmmo._ammoCount;
            otherAmmo._ammoCount = 0;
            return true;
        }
        else
        {
            return false;
        }
    }
    public string Use(int amount)
    {
        if (_ammoCount <= 0) {
            return "Out of ammo";
        }else if (_currentBulletsInCannon <= 0) {
            return "Reload ammo";
        }else if(_currentBulletsInCannon<amount) {
            return "Not enough Ammo";
        }else {
            return "";
        }
    }

    public void AddAmmo(int bulletsAmount)
    {
        _ammoCount += bulletsAmount;
    }
    private void Reload(int bulletsAmount)
    {
        if (_currentBulletsInCannon == MaxBulletsInCannon)
        {
            return;
        }
    
        int bulletsToLoad = Mathf.Min(bulletsAmount, MaxBulletsInCannon - _currentBulletsInCannon);
        _currentBulletsInCannon += bulletsToLoad;
        _ammoCount -= bulletsToLoad;
    }
    public void Reload()
    {
        Reload(_ammoCount);
    }
}

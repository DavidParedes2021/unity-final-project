using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ammunition : PickableObject
{
    // Define ammunition types for different weapons
    
    private int _ammoCount;
    
    public int MaxBulletsInCannon { get; set; }

    private int _currentBulletsInCannon = 0;

    private void Awake()
    {
        CollidableObject.AttachToScript(this.gameObject,nameof(MainPlayer));
        _ammoCount = Random.Range(10, 50);
    }

    public override void PickUp(Player player)
    {
        player.AddAmmunition(this);
        EventController.DestroyItem(this);
    }

    public bool MergeAmmunition(Ammunition otherAmmo)
    {
        this._ammoCount += otherAmmo._ammoCount;
        otherAmmo._ammoCount = 0;
        return true;
    }
    public string Use(int amount)
    {
        switch (_currentBulletsInCannon)
        {
            case <= 0 when _ammoCount>0:
                return "Reload ammo";
            case <= 0 when _ammoCount<=0:
                return "Out of ammo";
            default:
            {
                if(_currentBulletsInCannon<amount) {
                    return "Not enough Ammo";
                }
                _currentBulletsInCannon--;
                return "";
            }
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

    public string getStringR()
    {
        return _currentBulletsInCannon +"/"+MaxBulletsInCannon+ ":" + _ammoCount;
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : Weapon
{
    public int initialAmmoCount=20;

    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
        ammunitionToSetUp.MaxBulletsInCannon = 20;
        ammunitionToSetUp.AddAmmo(initialAmmoCount);
        remainingFireRate = fireRate;
    }

    public override void Trigger(GameObject owner, Vector3 position,Vector3 direction)
    {
        if (remainingFireRate >= fireRate) {
            var resultUseAmmo = ammunition.Use(1);
            if (resultUseAmmo.Length != 0) {//Error
                EventController.notifyEvent(EventController.NotificationType.ScreenMessage,resultUseAmmo);
                return;
            } 
            remainingFireRate = 0;
            FireBullet(owner,position,direction);
        }
    }
}
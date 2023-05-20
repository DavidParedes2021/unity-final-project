using System;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : Weapon
{
    public int initialAmmoCount=20;

    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
        ammunitionToSetUp.AmmoType = Ammunition.AmmunitionType.Pistol;
        ammunitionToSetUp.AddAmmo(initialAmmoCount);
        ammunitionToSetUp.Reload();
        remainingFireRate = fireRate;
    }

    protected override void Trigger()
    {
        if (remainingFireRate >= fireRate) {
            var resultUseAmmo = ammunition.Use(1);
            if (resultUseAmmo.Length != 0) {//Error
                EventController.notifyEvent(EventController.NotificationType.ScreenMessage,resultUseAmmo);
                return;
            } 
            remainingFireRate = 0;
            FireBullet(EventController.getLookDirectionVector());
        }
    }
}
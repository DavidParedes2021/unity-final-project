using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotGun : Weapon
{
    public int initialAmmoCount=8;
    public int pelletCount = 5;
    public float spreadAngle = 5f;

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
            //Instantiate bullet and fire it
            // Shoot bullets rapidly
            // Spread the bullets
            // Spread the bullets
            for (int i = 0; i < pelletCount; i++)
            {
                // Calculate random spread within the defined angle range
                Quaternion spreadRotation = Quaternion.Euler(0f, Random.Range(-spreadAngle, spreadAngle), 0f);

                // Modify the look direction based on the spread rotation
                Vector3 modifiedLookDirection = spreadRotation * EventController.getLookDirectionVector();

                // Fire the bullet with the modified look direction
                FireBullet(modifiedLookDirection);
            }
        }
    }
}
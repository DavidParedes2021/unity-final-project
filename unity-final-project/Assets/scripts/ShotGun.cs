using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShotGun : Weapon
{
    public int initialAmmoCount=8;
    public int pelletCount = 5;
    public float spreadAngle = 2.5f;

    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
        ammunitionToSetUp.MaxBulletsInCannon = 10;
        ammunitionToSetUp.AddAmmo(initialAmmoCount);
        remainingFireRate = fireRate;
    }

    public override void Trigger(GameObject owner,Vector3 position,Vector3 direction)
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
                Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle));

                // Modify the look direction based on the spread rotation
                Vector3 modifiedLookDirection = spreadRotation * direction;

                // Fire the bullet with the modified look direction
                FireBullet(owner,position,modifiedLookDirection);
            }
        }
    }
}
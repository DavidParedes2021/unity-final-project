using System;
using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MachineGun : Weapon
{
    public int initialAmmoCount=100;
    public int burstCount = 2;
    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
        ammunitionToSetUp.MaxBulletsInCannon = 150;
        ammunitionToSetUp.AddAmmo(initialAmmoCount);
        remainingFireRate = fireRate;
    }
    private IEnumerator FireBurst(float bulletDelay,GameObject owner, Vector3 position, Vector3 direction)
    {
        for (int i = 0; i < burstCount; i++)
        {
            FireBullet(owner, position, direction);
            yield return new WaitForSeconds(bulletDelay);
        }
    }

    public void StartFireBurst(float delay,GameObject owner, Vector3 position, Vector3 direction)
    {
        StartCoroutine(FireBurst(delay,owner, position, direction));
    }
    public override void Trigger(GameObject owner,Vector3 position,Vector3 direction)
    {
        if (remainingFireRate >= fireRate) {
            var resultUseAmmo = ammunition.Use(1);
            if (resultUseAmmo.Length != 0) {//Error
                Ec.notifyEvent(EC.NotificationType.ScreenMessage,resultUseAmmo);
                return;
            } 
            remainingFireRate = 0;
            playSoundShoot();
            StartFireBurst(fireRate/burstCount,owner,position,direction);
        }
    }
}
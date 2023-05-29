using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZombieAcid : Weapon
{
    public int initialAmmoCount;
    public float _secondsToBam;
    public float spreadAngle;
    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
        ammunitionToSetUp.MaxBulletsInCannon = 2;
        ammunitionToSetUp.AddAmmo(initialAmmoCount);
        remainingFireRate = fireRate;
    }

    public override void Trigger(GameObject owner, Vector3 position,Vector3 direction)
    {
        if (remainingFireRate >= fireRate) {
            var resultUseAmmo = ammunition.Use(1);
            if (resultUseAmmo.Length != 0) {//Error
                ReloadAmmo(false);
                return;
            } 
            remainingFireRate = 0;
            bulletVelocity *= 2;
            var granadeBullet = FireBullet(owner, position, direction);
            bulletVelocity /= 2;
            granadeBullet.destroyAfter = _secondsToBam;
            playSoundShoot();
            StartCoroutine(ShootBulletsAfterDelay(granadeBullet));
        }
    }

    private IEnumerator ShootBulletsAfterDelay(Bullet granadeBullet)
    {
        Vector3 lastPosition = granadeBullet.transform.position;
        for (int i = 0; i < _secondsToBam * 4; i++)
        {
            yield return new WaitForSeconds(0.25f);
            if (granadeBullet.IsDestroyed() || granadeBullet.gameObject.IsDestroyed()) {
                break;
            }
            lastPosition = granadeBullet.transform.position;
        }
        if (!(granadeBullet.IsDestroyed() || granadeBullet.gameObject.IsDestroyed()))
        {
            lastPosition = granadeBullet.transform.position;
        }
        for (int i = 0; i < 50; i++)
        {
            // Calculate random spread within the defined angle range
            Quaternion spreadRotation = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle));

            // Modify the look direction based on the spread rotation
            Vector3 modifiedLookDirection = spreadRotation * gameObject.transform.forward;
            var bullet = FireBullet(null, lastPosition, modifiedLookDirection);
            bullet.targetGO = Ec.MainPlayer.gameObject;
        }
    }
}
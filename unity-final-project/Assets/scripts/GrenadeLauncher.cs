using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeLauncher : Weapon
{
    public int initialAmmoCount=3;
    public float _secondsToBam;
    public AudioClip explosionClip;
    public int damageInPerksExplosion;
    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
        ammunitionToSetUp.MaxBulletsInCannon = 3;
        ammunitionToSetUp.AddAmmo(initialAmmoCount);
        remainingFireRate = fireRate;
    }

    public override void Trigger(GameObject owner, Vector3 position,Vector3 direction)
    {
        if (remainingFireRate >= fireRate) {
            var resultUseAmmo = ammunition.Use(1);
            if (resultUseAmmo.Length != 0) {//Error
                Ec.notifyEvent(EC.NotificationType.ScreenMessage,resultUseAmmo);
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
        playSoundShoot(explosionClip);
        for (int i = 0; i < 150; i++)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
            var fireBullet = FireBullet(null, lastPosition, randomDirection);
            fireBullet.damage = damageInPerksExplosion;
        }
    }
}
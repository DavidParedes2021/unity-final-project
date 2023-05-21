using System;
using Unity.VisualScripting;

public class MachineGun : Weapon
{
    public int initialAmmoCount=100;
    public int burstCount = 10;
    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
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
            for (int i = 0; i < burstCount; i++) {
                FireBullet(EventController.getLookDirectionVector());
            }
        }
    }
}
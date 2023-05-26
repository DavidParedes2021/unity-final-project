using System;
using Unity.VisualScripting;
using UnityEngine;

public class Sniper : Gun
{

    public Sprite zoomImage;
    protected override void DefineInitialState(Ammunition ammunitionToSetUp)
    {
        base.DefineInitialState(ammunitionToSetUp);
        ammunitionToSetUp.MaxBulletsInCannon = 7;
        ammunitionToSetUp.AddAmmo(initialAmmoCount);
        remainingFireRate = fireRate;
    }

    public override void ZoomIn(Camera camera)
    {
        base.ZoomIn(camera);
        EventController.UIController.setZoomImage(zoomImage);
    }
    public override void ZoomOut(Camera camera)
    {
        base.ZoomOut(camera);
        EventController.UIController.setZoomImage(null);
    }
}
using System;
using UnityEngine;
public class Consumable : PickableObject
{
    public GameObject ConsumablePrefab { set; get; }
    public double BenefitAmount { set; get; }
    public enum ConsumableType
    {
        Food,
        Water
    }
    public ConsumableType consumableType;
    public Sprite consumableImage;

    private void Awake()
    {
        CollidableObject.AttachToScript(this.gameObject,nameof(Consumable));
    }

    public override void PickUp(Player player)
    {
        player.AddConsumable(this);
        // Disable or remove the consumable object from the scene
        gameObject.SetActive(false);
    }
}
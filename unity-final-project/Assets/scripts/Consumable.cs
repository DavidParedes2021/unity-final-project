using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
        BenefitAmount = Random.Range(10f, 40f);
    }

    public override void PickUp(Player player)
    {
        if (player.AddConsumable(this))
        {
           EventController.DestroyItem(this);
        }
    }

    public static void RequireConsumable(GameObject prefab)
    {
        if (prefab.GetComponent<Consumable>()==null)
        {
            throw new Exception("There is no Consumable script attached and is required!");
        }
    }
}
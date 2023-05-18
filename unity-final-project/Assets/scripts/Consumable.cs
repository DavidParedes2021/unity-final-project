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

    public override void PickUp(Player player)
    {
        switch (consumableType)
        {
            case ConsumableType.Food:
                ConsumeFood(player);
                break;
            case ConsumableType.Water:
                ConsumeWater(player);
                break;
            default:
                Debug.LogWarning("Invalid consumable type!");
                break;
        }

        // Disable or remove the consumable object from the scene
        gameObject.SetActive(false);
    }

    private void ConsumeFood(Player player)
    {
        player.AddFood(this);
    }

    private void ConsumeWater(Player player)
    {
        player.AddWater(this);
    }
}
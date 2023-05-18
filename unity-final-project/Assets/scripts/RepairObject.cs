using UnityEngine;

public class RepairObject : PickableObject
{
    public GameObject RepairObjectPrefab { get; set;}
    public enum BoatPart
    {
        Engine,
        Propeller,
        Rudder,
        Sail,
        Hull
    }

    public BoatPart boatPart;

    public override void PickUp(Player player)
    {
        if (player is not MainPlayer) {
            return;
        }

        MainPlayer mainPlayer = (MainPlayer)player;
        // Implement the logic for picking up a repair object
        // For example, add the boat part to the player's inventory
        mainPlayer.AddBoatPart(boatPart);
        
        // Disable or remove the repair object from the scene
        gameObject.SetActive(false);
    }
}
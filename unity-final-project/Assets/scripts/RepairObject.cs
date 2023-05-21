using System;
using System.Collections.Generic;
using UnityEngine;

public class RepairObject : PickableObject
{
    public GameObject RepairObjectPrefab { get; set;}

    public enum BoatPart
    {
        Engine,
        Propeller,
        Gasoline
    }

    public int amountNeeded;
    public int currentAmount;
    public BoatPart boatPart;
    private void Awake()
    {
        if (boatPart == BoatPart.Engine || boatPart==BoatPart.Propeller)
        {
            amountNeeded = 1;
            
        }else if (boatPart == BoatPart.Gasoline)
        {
            amountNeeded = 3;
        }
        else
        {
            throw new Exception("Unknown behavior for boat part: " + boatPart);
        }
    }

    public override void PickUp(Player player)
    {
        if (player is not MainPlayer) {
            return;
        }

        MainPlayer mainPlayer = (MainPlayer)player;
        // Implement the logic for picking up a repair object
        // For example, add the boat part to the player's inventory
        mainPlayer.AddBoatPart(this);
        
        // Disable or remove the repair object from the scene
        gameObject.SetActive(false);
    }

    public override bool Equals(object other)
    {
        return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) ||
                                                 other.GetType() == this.GetType() && Equals((RepairObject)other));
    }

    protected bool Equals(RepairObject other)
    {
        return base.Equals(other) && boatPart == other.boatPart;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), (int)boatPart);
    }
}
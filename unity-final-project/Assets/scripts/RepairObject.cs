using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RepairObject : PickableObject
{
    
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
        CollidableObject.AttachToScript(this.gameObject,nameof(RepairObject));
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

    public RepairObject SetBoatPart(BoatPart newBoatPart)
    {
        this.boatPart = newBoatPart;
        return this;
    }
    public override void PickUp(Player player)
    {
        if (player is not MainPlayer mainPlayer) {
            return;
        }

        // Implement the logic for picking up a repair object
        // For example, add the boat part to the player's inventory
        if (mainPlayer.AddBoatPart(this))
        {
            gameObject.SetActive(false);
        }
        else
        {
            EventController.DestroyItem(this);
        }

    }
    public static bool HasAllBoatParts(List<RepairObject> repairObjects)
    {
        // Get all values of the BoatPart enum
        var boatPartValues = Enum.GetValues(typeof(BoatPart)).Cast<BoatPart>();

        // Check if RepairObjects contains at least one element of each BoatPart type
        return boatPartValues.All(boatPart => repairObjects.Any(obj => obj.boatPart == boatPart));
    }
    public override bool Equals(object other)
    {
        return !ReferenceEquals(null, other) && (ReferenceEquals(this, other) ||
                                                 other.GetType() == this.GetType() && Equals((RepairObject)other));
    }

    private bool Equals(RepairObject other)
    {
        return base.Equals(other) && boatPart == other.boatPart;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), (int)boatPart);
    }

    public static void RequireRepairObject(GameObject prefab)
    {
        if (prefab.GetComponent<RepairObject>()==null)
        {
            throw new Exception("There is not RepairObject script attached and is required!");
        }
    }

    public static string getNameOf(BoatPart boatPart1)
    {
        switch (boatPart1)
        {
            case BoatPart.Engine:
                return "Engine";
            case  BoatPart.Gasoline:
                return "Gasoline";
            case BoatPart.Propeller:
                return "Propeller";
            default:
                throw new Exception("Unknown behaviour for " + boatPart1);
        }
    }

    public void AddBoatPart(int i)
    {
        if (i < 0)
        {
            throw new Exception("Added amount mus be > 0");
        }
        currentAmount += i;
    }

    public bool IsComplete()
    {
        return currentAmount >= amountNeeded;
    }
}
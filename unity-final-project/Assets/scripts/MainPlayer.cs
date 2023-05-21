using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class MainPlayer : Player
{
    private AgentController _agentController;
    public HashSet<RepairObject> BoatParts { set; get; } = new HashSet<RepairObject>();
    public override void AddAmmunition(Ammunition otherAmmunition)
    {

        for (var i = 0; i < Weapons.Count; i++)
        {
            if (Weapons[i].MergeAmmunition(otherAmmunition))
            {
                return;
            }
        }
    }

    public override void AddFood(Consumable consumable)
    {
        Life += consumable.BenefitAmount;
        consumable.BenefitAmount = 0;
    }

    public override void AddWater(Consumable consumable)
    {
        Stamina += consumable.BenefitAmount;
        consumable.BenefitAmount = 0;
    }

    public override void AddWeapon(Weapon weapon)
    {
        for (var i = 0; i < Weapons.Count; i++)
        {
            if (Weapons[i].GetType() == weapon.GetType())
            {
                return;
            }
        }
        if (CurrentWeapon == null)
        {
            CurrentWeapon = weapon;
        }
        Weapons.Add(weapon);
    }
    public void AddBoatPart(RepairObject boatPart){
        if (!BoatParts.Contains(boatPart))
        {
            BoatParts.Add(boatPart);
        }
    }
}
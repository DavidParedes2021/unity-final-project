using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainPlayer : Player
{
    public HashSet<RepairObject> BoatParts = new();

    protected override void Awake()
    {
        base.Awake();
        CollidableObject.AttachToScript(this.gameObject,nameof(MainPlayer));
        
        foreach (var boatPart in Enum.GetValues(typeof(RepairObject.BoatPart)).Cast<RepairObject.BoatPart>())
        {
            BoatParts.Add(new GameObject(RepairObject.getNameOf(boatPart)).AddComponent<RepairObject>().SetBoatPart(boatPart));
        }

        maxLife = 100;
        Life = 100;
        
        maxStamina = 100;
        Stamina = 100;
    }

    public override void AddAmmunition(Ammunition otherAmmunition)
    {
        
        if (CurrentWeapon!=null)
        {
            CurrentWeapon.MergeAmmunition(otherAmmunition);
        }
    }
    protected override void AddFood(Consumable consumable)
    {
        Life += consumable.BenefitAmount;
        consumable.BenefitAmount = 0;
    }

    protected override void AddWater(Consumable consumable)
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
using System;
using UnityEngine;
public class Zombie : Player
{
    protected override void Awake()
    {
        base.Awake();
        CollidableObject.AttachToScript(this.gameObject,nameof(Zombie));
    }

    public override void AddAmmunition(Ammunition otherAmmunition)
    {
        throw new System.NotImplementedException();
    }

    protected override void AddFood(Consumable consumable)
    {
        throw new System.NotImplementedException();
    }

    protected override void AddWater(Consumable consumable)
    {
        throw new System.NotImplementedException();
    }

    public override void AddWeapon(Weapon weapon)
    {
        throw new System.NotImplementedException();
    }

    public void Attack(MainPlayer otherMainPlayer)
    {
        
    }
}
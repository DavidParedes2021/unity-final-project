using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public double Life { set; get; }
    public double Stamina { set; get; }
    public double maxStamina;
    public double maxLife;
    public double WalkVelocity { set; get; }
    public double JumpForce { set; get; }
    public Weapon CurrentWeapon { set; get; }
    protected List<Weapon> Weapons { set; get; }
    protected List<Consumable> Consumables { set; get; }

    public abstract void AddAmmunition(Ammunition otherAmmunition);

    public abstract void AddFood(Consumable consumable);

    public abstract void AddWater(Consumable consumable);

    public abstract void AddWeapon(Weapon weapon);

    public double GetStaminaPercentage()
    {
        return Stamina / maxStamina;
    }

    public double GetLifePercentage()
    {
        return Life / maxLife;
    }
}
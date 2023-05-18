using System.Collections.Generic;

public abstract class Player
{
    protected double Life { set; get; }
    protected double Stamina { set; get; }
    protected double WalkVelocity { set; get; }
    protected double JumpForce { set; get; }
    protected Weapon CurrentWeapon { set; get; }
    protected List<Weapon> Weapons { set; get; }
    protected List<Consumable> Consumables { set; get; }

    public abstract void AddAmmunition(Ammunition otherAmmunition);

    public abstract void AddFood(Consumable consumable);

    public abstract void AddWater(Consumable consumable);

    public abstract void AddWeapon(Weapon weapon);
}
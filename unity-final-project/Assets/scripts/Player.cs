using System;
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
    public List<Weapon> Weapons = new List<Weapon>();
    public List<Consumable> Consumables = new List<Consumable>();
    public EventController EventController;
    public abstract void AddAmmunition(Ammunition otherAmmunition);

    protected virtual void Awake()
    {
        Weapons ??= new List<Weapon>();
        var newWeapons = new List<Weapon>(Weapons);
        Weapons.RemoveRange(0,Weapons.Count);
        foreach (var weapon in newWeapons)
        {
            var weaponInstantiated = Instantiate(weapon);
            AddWeapon(weaponInstantiated);
        }
        Consumables ??= new List<Consumable>();
        var newConsumables = new List<Consumable>(Consumables);
        Consumables.RemoveRange(0,Consumables.Count);
        foreach (var consumable in newConsumables)
        {
            var instantiatedConsumable = Instantiate(consumable);
            AddConsumable(instantiatedConsumable);
        }
    }
    public void SwitchToNextWeapon()
    {
        if (Weapons.Count == 0)
        {
            return;
        }

        int currentIndex = Weapons.IndexOf(CurrentWeapon);
        int nextIndex = (currentIndex + 1) % Weapons.Count;
        CurrentWeapon = Weapons[nextIndex];
    }

    public void SwitchToPreviousWeapon()
    {
        if (Weapons.Count == 0)
        {
            return;
        }

        int currentIndex = Weapons.IndexOf(CurrentWeapon);
        int previousIndex = currentIndex - 1;
        if (previousIndex < 0)
        {
            previousIndex = Weapons.Count - 1;
        }
        CurrentWeapon = Weapons[previousIndex];
    }

    public virtual void attachToEventControlelr(EventController controller)
    {
        foreach (var weapon in Weapons)
        {
            weapon.AttachToEventController(controller);
        }

        foreach (var consumable in Consumables)
        {
            consumable.AttachToEventController(controller);
        }
    }
    public void AddConsumable(Consumable consumable)
    {
        switch (consumable.consumableType)
        {
            case Consumable.ConsumableType.Food:
                AddFood(consumable);
                break;
            case Consumable.ConsumableType.Water:
                AddWater(consumable);
                break;
            default:
                throw new NotImplementedException("Unknown Behaviour for: " + consumable);
        }
    }

    public void TakeDamage(Bullet bullet)
    {
        TakeDamage(bullet.damage);
    }
    public void TakeDamage(float damage)
    {
        if (damage < 0)
        {
            throw new Exception("Damage must be positive");
        }
        Life -= damage;
    }
    protected abstract void AddFood(Consumable consumable);

    protected abstract void AddWater(Consumable consumable);

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
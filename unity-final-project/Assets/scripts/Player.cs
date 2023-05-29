using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Player : MonoBehaviour
{
    public double Life { set; get; }
    public double Stamina { set; get; }
    public double maxStamina;
    public double maxLife;
    public float staminaLossRatePerSecond = 15f;
    public float staminaRecoverRatePerSecond = 5f;
    
    public float lifeRecoverRatePerSecond = 5f;
    public double WalkVelocity;
    public Weapon CurrentWeapon { set; get; }
    public List<Weapon> Weapons = new List<Weapon>();
    public List<Consumable> Consumables = new List<Consumable>();
    [FormerlySerializedAs("EC")] public EC EC;
    protected int MaxConsumablesAmount = 4;
    protected virtual void Awake()
    {
        Weapons ??= new List<Weapon>();
        var newWeapons = new List<Weapon>(Weapons);
        Weapons.RemoveRange(0,Weapons.Count);
        foreach (var weapon in newWeapons)
        {
            var weaponInstantiated = Instantiate(weapon);
            weaponInstantiated.PickUp(this);
        }
        Consumables ??= new List<Consumable>();
        var newConsumables = new List<Consumable>(Consumables);
        Consumables.RemoveRange(0,Consumables.Count);
        foreach (var consumable in newConsumables)
        {
            var instantiatedConsumable = Instantiate(consumable);
            AddConsumable(instantiatedConsumable);
        }
        Life = maxLife;
        Stamina = maxStamina;
    }
    public void SwitchToNextWeapon()
    {
        if (WaitUntilReloadReleased()) return;
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
        if (WaitUntilReloadReleased()) return;
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


    private bool WaitUntilReloadReleased()
    {
        if (CurrentWeapon.isReloading())
        {
            EC.notifyEvent(EC.NotificationType.ScreenMessage, "Espera a terminar recarga", 1f);
            return true;
        }

        return false;
    }
    public virtual void attachToEventControlelr(EC controller)
    {
        foreach (var weapon in Weapons)
        {
            weapon.AttachToEventController(controller);
        }

        foreach (var consumable in Consumables)
        {
            consumable.AttachToEventController(controller);
        }

        this.EC = controller;
    }

    public bool AddConsumable(Consumable consumable)
    {
        if (Consumables.Count == MaxConsumablesAmount)
        {
            return false;
        }

        Consumables.Add(consumable);
        return true;
    }
    public bool UseConsumable(int slot)
    {
        if (slot >= Consumables.Count)
        {
            return false;
        }
        Consumable consumable = Consumables[slot];
        switch (consumable.consumableType)
        {
            case Consumable.ConsumableType.Food:
                UseFood(consumable);
                break;
            case Consumable.ConsumableType.Water:
                UseWater(consumable);
                break;
            default:
                throw new NotImplementedException("Unknown Behaviour for: " + consumable);
        }

        Consumables.Remove(consumable);
        return true;
    }

    public virtual void AddAmmunition(Ammunition otherAmmunition)
    {
        
        if (CurrentWeapon!=null)
        {
            CurrentWeapon.MergeAmmunition(otherAmmunition);
        }
    }
    protected virtual void UseFood(Consumable consumable)
    {
        Life += consumable.BenefitAmount;
        consumable.BenefitAmount = 0;
    }

    protected virtual void UseWater(Consumable consumable)
    {
        Stamina += consumable.BenefitAmount;
        consumable.BenefitAmount = 0;
    }

    public virtual bool AddWeapon(Weapon weapon)
    {
        for (var i = 0; i < Weapons.Count; i++)
        {
            if (Weapons[i].GetType() == weapon.GetType())
            {
                Weapons[i].MergeAmmunition(weapon.ammunition);
                return false;
            }
        }
        if (CurrentWeapon == null)
        {
            CurrentWeapon = weapon;
        }
        Weapons.Add(weapon);
        return true;
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
        if (Life < 0)
        {
            EC.DestroyItem(this);
        }
    }
    public double GetStaminaPercentage()
    {
        return Stamina / maxStamina;
    }

    public double GetLifePercentage()
    {
        return Life / maxLife;
    }
}
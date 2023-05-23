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

    private AgentController _agentController;

    private bool _isShooting=false;
    protected override void Awake()
    {
        base.Awake();
        CollidableObject.AttachToScript(this.gameObject,nameof(MainPlayer));
        
        foreach (var boatPart in Enum.GetValues(typeof(RepairObject.BoatPart)).Cast<RepairObject.BoatPart>())
        {
            BoatParts.Add(new GameObject(RepairObject.getNameOf(boatPart)).AddComponent<RepairObject>().SetBoatPart(boatPart));
        }

        _agentController = U.GetOrAddComponent<AgentController>(this.gameObject);
        
        maxLife = 100;
        Life = 100;
        
        maxStamina = 100;
        Stamina = 100;
    }

    public override void attachToEventControlelr(EventController controller)
    {
        base.attachToEventControlelr(controller);
        foreach (var repairObject in BoatParts)
        {
            repairObject.AttachToEventController(controller);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.ReloadAmmo();
            }
        }

        // Handle weapon trigger input (Mouse Left Click)
        //Until the button is released, trigger 
        if (Input.GetMouseButtonDown(0))
        {
            _isShooting = true;
        }

        if (_isShooting && CurrentWeapon != null)
        {
            CurrentWeapon.Trigger(gameObject,transform.position+3*transform.forward,GetLookingDirection());
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isShooting = false;
        }
        // Cambiar de arma usando la rueda del ratón
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta > 0f)
        {
            SwitchToNextWeapon();
        }
        else if (scrollDelta < 0f)
        {
            SwitchToPreviousWeapon();
        }
        
    }

    private Vector3 GetLookingDirection()
    {
        return _agentController.GetCamera().transform.forward;
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
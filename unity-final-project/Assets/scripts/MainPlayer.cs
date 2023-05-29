using System;
using System.Collections;
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
    
    private Coroutine _statisticsRestoreCoroutine;
    private bool _isShooting;
    public float fov;
    private bool isZoomed=false;
    protected override void Awake()
    {
        base.Awake();
        CollidableObject.AttachToScript(this.gameObject,nameof(MainPlayer));
        
        foreach (var boatPart in Enum.GetValues(typeof(RepairObject.BoatPart)).Cast<RepairObject.BoatPart>())
        {
            BoatParts.Add(new GameObject(RepairObject.getNameOf(boatPart)).AddComponent<RepairObject>().SetBoatPart(boatPart));
        }

        _agentController = U.GetOrAddComponent<AgentController>(this.gameObject);
        
        
        lifeRecoverRatePerSecond = (float)(maxLife * 0.03);//Recover 3% each second
        
        staminaRecoverRatePerSecond = (float)(maxLife * 0.05);//Recover 5% each second
        staminaLossRatePerSecond = staminaRecoverRatePerSecond * 4;//Stamina reduced 15% each second;

        _agentController.OnRun += ReduceStamina;
    }

    private void Start()
    {
        _agentController.GetCamera().fieldOfView = fov;
        _statisticsRestoreCoroutine = StartCoroutine(RestoreStatisticsCoroutine());
    }

    
    private void OnDestroy()
    {
        // Stop the stamina restore coroutine when the player is destroyed
        if (_statisticsRestoreCoroutine != null)
        {
            StopCoroutine(_statisticsRestoreCoroutine);
        }
    }
    private IEnumerator RestoreStatisticsCoroutine()
    {
        while (true)
        {
            // Restore stamina over time
            Stamina += staminaRecoverRatePerSecond * 0.1;
            Life += lifeRecoverRatePerSecond * 0.1;

            // Clamp the stamina value to the maximum
            Stamina = Mathf.Clamp((float)Stamina, float.MinValue,(float) maxStamina);
            
            // Clamp the stamina value to the maximum
            Life = Mathf.Clamp((float)Life, -10,(float) maxLife);

            // Wait for a short duration before restoring stamina again
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void ReduceStamina(float deltaTime)
    {
        double previousStamina = Stamina;
        Stamina -= staminaLossRatePerSecond*deltaTime;
        Stamina = Mathf.Clamp((float)Stamina, -staminaLossRatePerSecond,(float) maxStamina);
        if (previousStamina >= 0 && Stamina < 0) {
            SoundsManager.PlayClipAndDestroy(this.gameObject,EC.RM.SM.breathingSound);
        }
        if (Stamina <= 0)
        {
            Stamina = -3*staminaLossRatePerSecond;
        }
    }


    public bool HasAllBoatParts()
    {
        return BoatParts.All(repairObject => repairObject.IsComplete());
    }
    public List<RepairObject> ReturnCompletedBoatParts()
    {
        return BoatParts.Where(repairObject => repairObject.IsComplete()).ToList();
    }
    public override void attachToEventControlelr(EC controller)
    {
        base.attachToEventControlelr(controller);
        foreach (var repairObject in BoatParts)
        {
            repairObject.AttachToEventController(controller);
        }
    }

    private void Update()
    {
        if (Math.Abs(_agentController.GetCamera().fieldOfView - fov) > 0.1 && !isZoomed)
        {
            _agentController.GetCamera().fieldOfView = fov;
            EC.UIController.setZoomImage(null);
        }
        if (Stamina <= 0)
        {
            _agentController.DisableRun();
        }
        else
        {
            _agentController.EnableRun();
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.ReloadAmmo(true);
            }
        }

        for (int i = 0; i < MaxConsumablesAmount; i++) {
            if (Input.GetKeyUp(KeyCode.Alpha0 + i))
            {
                if (UseConsumable(i - 1))
                {
                    SoundsManager.PlayClipAndDestroy(this.gameObject,EC.RM.SM.perkSound);
                }
            }
        }
        // Handle weapon trigger input (Mouse Left Click)
        //Until the button is released, trigger 
        if (Input.GetMouseButtonDown(0))
        {
            _isShooting = true;
        }
        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = true;
            CurrentWeapon.ZoomIn(_agentController.GetCamera());
        }
        if (Input.GetMouseButtonUp(1))
        {
            isZoomed = false;
            CurrentWeapon.ZoomOut(_agentController.GetCamera());
        }

        if (_isShooting && CurrentWeapon != null)
        {
            CurrentWeapon.Trigger(gameObject,_agentController.GetCamera().transform.position+_agentController.GetCamera().transform.forward,GetLookingDirection());
        }
        if (Input.GetMouseButtonUp(0))
        {
            _isShooting = false;
        }
        // Cambiar de arma usando la rueda del ratón
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        switch (scrollDelta)
        {
            case > 0f:
                SwitchToNextWeapon();
                RestoreZoom();
                break;
            case < 0f:
                SwitchToPreviousWeapon();
                RestoreZoom();
                break;
        }
        
    }

    private void RestoreZoom()
    {
        isZoomed = false;
        _agentController.GetCamera().fieldOfView = fov;
        EC.UIController.setZoomImage(null);
    }

    private Vector3 GetLookingDirection()
    {
        return _agentController.GetCamera().transform.forward;
    }

    public bool AddBoatPart(RepairObject boatPart){
        foreach (var repairObject in BoatParts)
        {
            if (repairObject.boatPart == boatPart.boatPart)
            {
                repairObject.AddBoatPart(1);
                return true;
            }
        }
        BoatParts.Add(boatPart);
        return true;
    }

}
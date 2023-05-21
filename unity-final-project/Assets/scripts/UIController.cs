using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI coordinatesText;
    public TextMeshProUGUI boatStateText;
    public Image currentWeaponImage;
    public TextMeshProUGUI ammunitionCountText;
    public Slider healthSlider;
    public Slider staminaSlider;
    public Image[] inventorySlots;
    public Image compassBackgroundImage;
    public Image compassImage;
    public GameObject minimap;

    public EventController eventController;
    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MainPlayer mainPlayer = eventController.MainPlayer;

        UpdateCoordinates(mainPlayer);
        UpdateBoatState(mainPlayer);
        UpdateCurrentWeapon(mainPlayer);
        UpdateHealthSlider(mainPlayer);
        UpdateStaminaSlider(mainPlayer);
    }

    private void UpdateCoordinates(MainPlayer mainPlayer)
    {
        string coordS = mainPlayer.gameObject.transform.position.ToString("F2");
        if (!coordinatesText.GetParsedText().Equals(coordS))
        {
            coordinatesText.SetText(coordS);
        }
    }

    private void UpdateBoatState(MainPlayer mainPlayer)
    {
        StringBuilder boatStateBuilder = new StringBuilder();
        foreach (RepairObject boatPart in mainPlayer.BoatParts)
        {
            int currentAmount = boatPart.currentAmount;
            int amountNeeded = boatPart.amountNeeded;
            boatStateBuilder.AppendLine($"{boatPart}: {currentAmount}/{amountNeeded}");
        }

        if (!boatStateBuilder.ToString().Equals(boatStateText.GetParsedText()))
        {
            boatStateText.text = boatStateBuilder.ToString();
        }
    }

    private void UpdateCurrentWeapon(MainPlayer mainPlayer)
    {
        Image currentWeaponI = mainPlayer.CurrentWeapon.weaponImage;
        if (currentWeaponImage.sprite != currentWeaponI.sprite)
        {
            currentWeaponImage.sprite = currentWeaponI.sprite;
        }
        string ammoS = mainPlayer.CurrentWeapon.ammunition.getStringR();
        if (!ammunitionCountText.GetParsedText().Equals(ammoS))
        {
            ammunitionCountText.SetText(ammoS);
        }
    }

    private void UpdateHealthSlider(MainPlayer mainPlayer)
    {
        if (Math.Abs(healthSlider.minValue) > 0.1)
        {
            healthSlider.minValue = 0;
        }
        if (Math.Abs(healthSlider.maxValue - mainPlayer.maxLife) > 0.1)
        {
            healthSlider.maxValue = (float)mainPlayer.maxLife;
        }
        if (Math.Abs(healthSlider.value - mainPlayer.Life) > 0.1)
        {
            healthSlider.value = (float)mainPlayer.Life;
        }
    }

    private void UpdateStaminaSlider(MainPlayer mainPlayer)
    {
        if (Math.Abs(staminaSlider.minValue) > 0.1)
        {
            staminaSlider.minValue = 0;
        }
        if (Math.Abs(staminaSlider.maxValue - mainPlayer.maxLife) > 0.1)
        {
            staminaSlider.maxValue = (float)mainPlayer.maxLife;
        }
        if (Math.Abs(staminaSlider.value - mainPlayer.Life) > 0.1)
        {
            staminaSlider.value = (float)mainPlayer.Life;
        }
    }
}

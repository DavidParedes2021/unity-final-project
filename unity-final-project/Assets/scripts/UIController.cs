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
    public Compass Compass;
    public MinimapController MinimapController;
    public TextMeshProUGUI messageText;
    public EventController eventController;

    private MainPlayer _mainPlayer;
    private int maxInventorySlots = 4;

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        _mainPlayer = eventController.MainPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCoordinates(_mainPlayer);
        UpdateBoatState(_mainPlayer);
        UpdateCurrentWeapon(_mainPlayer);
        UpdateHealthSlider(_mainPlayer);
        UpdateStaminaSlider(_mainPlayer);
        UpdateInventorySlots(_mainPlayer);
    }

    private void UpdateInventorySlots(MainPlayer mainPlayer)
    {
        List<Consumable> mainPlayerConsumables = mainPlayer.Consumables;
        for (var i = 0; i < maxInventorySlots; i++)
        {
            if (i < mainPlayerConsumables.Count)
            {
                if (inventorySlots[i].overrideSprite != mainPlayerConsumables[i].consumableImage)
                {
                    inventorySlots[i].overrideSprite = mainPlayerConsumables[i].consumableImage;
                }
            }
            else
            {
                inventorySlots[i].overrideSprite = null;
            }
           
        }
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
            boatStateBuilder.AppendLine($"{RepairObject.getNameOf(boatPart.boatPart)}: {currentAmount}/{amountNeeded}");
        }

        if (!boatStateBuilder.ToString().Equals(boatStateText.GetParsedText()))
        {
            boatStateText.text = boatStateBuilder.ToString();
        }
    }

    private void UpdateCurrentWeapon(MainPlayer mainPlayer)
    {
        if (mainPlayer.CurrentWeapon == null)
        {
            if (currentWeaponImage.overrideSprite != null)
            {
                currentWeaponImage.overrideSprite = null;
            }
            return;
        }
        Sprite currentWeaponSprite = mainPlayer.CurrentWeapon.weaponImage;
        if (currentWeaponImage.overrideSprite != currentWeaponSprite)
        {
            currentWeaponImage.overrideSprite = currentWeaponSprite;
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
        if (Math.Abs(staminaSlider.maxValue - mainPlayer.maxStamina) > 0.1)
        {
            staminaSlider.maxValue = (float)mainPlayer.maxLife;
        }
        if (Math.Abs(staminaSlider.value - mainPlayer.Stamina) > 0.1)
        {
            staminaSlider.value = (float)mainPlayer.Stamina;
        }
    }

    public void WriteMessage(string message, float displayTime)
    {
        StartCoroutine(ShowMessage(message, displayTime));
    }

    private IEnumerator ShowMessage(string message, float displayTime)
    {
        messageText.text = message;
        messageText.enabled = true;

        yield return new WaitForSeconds(displayTime * 0.5f); // Wait for 90% of the display time

        float blinkInterval = (float)(displayTime*0.5/10f); // Time interval for blinking (in seconds)
        float blinkDuration = displayTime * 0.5f; // Duration for blinking (10% of the display time)

        float blinkTime = 0f;
        while (blinkTime < blinkDuration)
        {
            messageText.enabled = !messageText.enabled; // Toggle the text visibility

            yield return new WaitForSeconds(blinkInterval);

            blinkTime += blinkInterval;
        }

        messageText.enabled = false; // Ensure the text is not visible at the end of blinking
    }
}

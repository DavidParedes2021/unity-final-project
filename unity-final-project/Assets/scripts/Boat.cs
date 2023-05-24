using System;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boat : MonoBehaviour
{
    private MainPlayer mainPlayer;
    public float timeSinceLastMessage = 0;

    public void AttachToPlayer(MainPlayer newMainPlayer)
    {
        this.mainPlayer = newMainPlayer;
    }
    private void Update()
    {
        timeSinceLastMessage += Time.deltaTime;
        if (Vector3.Distance(mainPlayer.transform.position, transform.position) < 5 && timeSinceLastMessage>10)
        {
            timeSinceLastMessage = 0;
            if (mainPlayer.HasAllBoatParts())
            {
                mainPlayer.EventController.notifyEvent(EventController.NotificationType.ScreenMessage,"Presiona P para reparar el bote",3f);
            }else {
                mainPlayer.EventController.notifyEvent(EventController.NotificationType.ScreenMessage,"No has reunido todas las partes del bote",3f);
            }
        }

        if (Input.GetKeyUp(KeyCode.P)) {
            if (mainPlayer.HasAllBoatParts())
            {
                mainPlayer.EventController.WinGame();
            }else {
                mainPlayer.EventController.notifyEvent(EventController.NotificationType.ScreenMessage,"No has reunido todas las partes del bote");
            }
        }
    }
}
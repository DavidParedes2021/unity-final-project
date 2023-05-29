using System;
using UnityEngine;

public class Boat : MonoBehaviour
{
    private MainPlayer mainPlayer;
    private float timeSinceLastMessage = 0;
    private bool isRepaired;

    private void Awake()
    {
        isRepaired = false;
    }

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
                mainPlayer.EC.notifyEvent(EC.NotificationType.ScreenMessage,"Presiona P para reparar el bote",3f);
            }else {
                mainPlayer.EC.notifyEvent(EC.NotificationType.ScreenMessage,"No has reunido todas las partes del bote",3f);
            }
        }

        if (Input.GetKeyUp(KeyCode.P)) {
            if (mainPlayer.HasAllBoatParts())
            {
                isRepaired = true;
            }else {
                mainPlayer.EC.notifyEvent(EC.NotificationType.ScreenMessage,"No has reunido todas las partes del bote");
            }
        }
    }

    public static Boat requireBoatScript(GameObject gameObject)
    {
        var boat = gameObject.GetComponent<Boat>();
        if (boat==null)
        {
            throw new Exception("A boat script is required and it is not present!");
        }

        return boat;
    }

    public bool IsRepaired()
    {
        return isRepaired;
    }
}
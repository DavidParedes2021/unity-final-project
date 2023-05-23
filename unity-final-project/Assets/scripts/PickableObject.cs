using System;
using UnityEngine;

public abstract class PickableObject : MonoBehaviour
{
    protected virtual void Start()
    {
        
    }

    public void AttachToEventController(EventController otherEventController)
    {
        this.EventController = otherEventController;
    }

    public EventController EventController { get; set; }
    private string _objectName;
    public abstract void PickUp(Player player);
}
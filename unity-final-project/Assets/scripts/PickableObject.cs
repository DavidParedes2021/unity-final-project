using System;
using UnityEngine;

public abstract class PickableObject : MonoBehaviour
{
    protected virtual void Start()
    {
        
    }

    public void AttachToEventController(EC otherEc)
    {
        this.Ec = otherEc;
    }

    public EC Ec { get; set; }
    private string _objectName;
    public abstract void PickUp(Player player);
}
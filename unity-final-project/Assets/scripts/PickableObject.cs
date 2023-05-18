using UnityEngine;

public abstract class PickableObject : MonoBehaviour
{
    
    public EventController EventController { get; set; }
    private string _objectName;
    public abstract void PickUp(Player player);
}
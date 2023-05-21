using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EventController : MonoBehaviour
{
        public enum NotificationType
        {
                ScreenMessage
        }

        public UIController UIController;
        public List<Zombie> Zombies { get; set; }
        public MainPlayer MainPlayer;
        public List<Consumable> Consumables { get; set; }
        public List<Weapon> Weapons { get; set; }
        public ResourcesManager ResourcesManager { get; set; }
        void Start()
        {
                
        }

        public void notifyEvent(NotificationType notificationType,string message){
        }

        public Vector3 getLookDirectionVector()
        {
                throw new System.NotImplementedException();
        }
}
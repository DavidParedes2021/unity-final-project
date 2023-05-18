using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EventController : MonoBehaviour
{
        public enum NotificationType
        {
                ScreenMessage
        }
        private UiController _uiController;
        private List<Zombie> _zombies;
        private MainPlayer _mainPlayer;
        private List<Consumable> _consumibles;
        private List<Weapon> _weapons;
        public ResourcesManager resourcesManager;
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
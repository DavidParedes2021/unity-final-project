using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class EventController : MonoBehaviour
{
        
        public enum NotificationType
        {
                ScreenMessage
        }
        public void notifyEvent(NotificationType notiType, string message)
        {
               notifyEvent(notiType,message,2f);
        }
        public void notifyEvent(NotificationType notiType, string message, float time)
        {
                if (notiType == NotificationType.ScreenMessage)
                {
                        UIController.WriteMessage(message, time);
                }
        }
        public UIController UIController;
        public List<Zombie> Zombies { get; set; }
        public MainPlayer MainPlayer { get; set; }
        public List<Consumable> Consumables { get; set; }
        public List<Weapon> Weapons { get; set; }

        public List<RepairObject> RepairObjects;
        
        public List<Ammunition> Ammunitions; 
        
        public ResourcesManager ResourcesManager;

        public Boat Boat { get; set; }

        public int maxZombies=100;
        public float spawnZombieRateInSeconds=1f;

        public int missionStatus { get; set; }
        public const int MissionNotStarted = -1;
        public const int FindBoatPartsStatus = 0;
        public const int RepairBoatStatus = 1;

        private void Awake()
        {
                missionStatus = MissionNotStarted;
                
                InstantiateListHolders();
                SpawnGO();
        }
        
        private void Start()
        {
                StartCoroutine(SpawnZombiesCoroutine());
        }

        private void Update()
        {
                if (missionStatus == MissionNotStarted)
                {
                        missionStatus = FindBoatPartsStatus;  
                        RequireTargetsForCompass(UIController.Compass);
                        notifyEvent(NotificationType.ScreenMessage,"Encuentra las piezas del barco!");
                }else if (MainPlayer.HasAllBoatParts() && missionStatus==FindBoatPartsStatus)
                {
                        missionStatus = RepairBoatStatus;
                        RequireTargetsForCompass(UIController.Compass);
                        notifyEvent(NotificationType.ScreenMessage,"Encuentra el barco y reparalo!");
                }else if (Boat.IsRepaired()) {
                        WinGame();
                }
        }

        private void SpawnGO()
        {
                SpawnMainPlayer();
                SpawnBoat();
                for (int i = 0; i < 50; i++)
                {
                        SpawnAmmunition();
                }

                for (int i = 0; i < 25; i++)
                {
                        SpawnWeapon();
                }
                
                for (int i = 0; i < 50; i++)
                {
                        SpawnConsumables();
                }

                for (int i = 0; i < 10; i++)
                {
                        SpawnRepairObject(RepairObject.BoatPart.Engine);
                }

                for (int i = 0; i < 20; i++)
                {
                        SpawnRepairObject(RepairObject.BoatPart.Propeller);
                }

                for (int i = 0; i < 30; i++)
                {
                        SpawnRepairObject(RepairObject.BoatPart.Gasoline);
                }
        }
        private void SpawnBoat()
        {
                Vector3 randomBoatPosition = GetRandomTerrainPosition(minElevation:5,maxElevation:20);
                var boatInstantiated = Instantiate(ResourcesManager.boatPrefab, randomBoatPosition, Quaternion.identity);
                var boat = Boat.requireBoatScript(boatInstantiated);
                this.Boat = boat;
                this.Boat.AttachToPlayer(MainPlayer);
        }

        private void SpawnConsumables()
        {
                GameObject prefab = ResourcesManager.chooseRandomConsumable();
                Consumable.RequireConsumable(prefab);
                
                // Get a random position on the terrain
                Vector3 randomPosition = GetRandomTerrainPosition(ResourcesManager.currentTerrain,MainPlayer.transform.position);

                // Spawn the repair object at the random position
                GameObject instConsumable = Instantiate(prefab, randomPosition, Quaternion.identity);
                Consumable consumable = instConsumable.GetComponent<Consumable>();
                
                consumable.AttachToEventController(this);

                Consumables.Add(consumable);
        }

        private void SpawnRepairObject(RepairObject.BoatPart boatPart)
        {
                GameObject prefab = ResourcesManager.GetBoatPartPrefab(boatPart);
                RepairObject.RequireRepairObject(prefab);
                
                // Get a random position on the terrain
                Vector3 randomPosition = GetRandomTerrainPosition(ResourcesManager.currentTerrain,MainPlayer.transform.position,lowerLimitDistance:150,upperLimitDistance:300);

                // Spawn the repair object at the random position
                GameObject repairObj = Instantiate(prefab, randomPosition, Quaternion.identity);
                RepairObject repairObject = repairObj.GetComponent<RepairObject>();
                repairObject.boatPart = boatPart;
                
                repairObject.AttachToEventController(this);

                RepairObjects.Add(repairObject);
        }

        private void InstantiateListHolders()
        {
                Zombies ??= new List<Zombie>();
                Ammunitions ??= new List<Ammunition>();
                Weapons ??= new List<Weapon>();
                Consumables ??= new List<Consumable>();
                RepairObjects ??= new List<RepairObject>();
                Consumables ??= new List<Consumable>();
        }


        private void SpawnMainPlayer()
        {
                Vector3 randomTerrainPosition = GetRandomTerrainPosition(ResourcesManager.currentTerrain);
                GameObject mainPlayerPrefab = ResourcesManager.mainPlayerPrefab;

                GameObject mainPlayerObj = Instantiate(mainPlayerPrefab, randomTerrainPosition, Quaternion.identity);
                MainPlayer mainPlayer = U.GetOrAddComponent<MainPlayer>(mainPlayerObj);
                MainPlayer = mainPlayer;
                MainPlayer.attachToEventControlelr(this);
        }
        private void SpawnAmmunition()
        {
                Vector3 randomTerrainPosition = GetRandomTerrainPosition(ResourcesManager.currentTerrain,lowerLimitDistance:10,upperLimitDistance:20);
                GameObject ammunitionPrefab = ResourcesManager.ammunitionPrefab;

                GameObject ammunitionGo = Instantiate(ammunitionPrefab, randomTerrainPosition, Quaternion.identity);
                Ammunition ammunition = U.GetOrAddComponent<Ammunition>(ammunitionGo);
                ammunition.AttachToEventController(this);
                Ammunitions.Add(ammunition);
        }
        private void SpawnWeapon()
        {
                Vector3 randomTerrainPosition = GetRandomTerrainPosition(ResourcesManager.currentTerrain,lowerLimitDistance:10,upperLimitDistance:20);
                GameObject weaponPrefab = ResourcesManager.chooseRandomWeapon();

                GameObject weaponGo = Instantiate(weaponPrefab, randomTerrainPosition, Quaternion.identity);
                Weapon weapon = Weapon.RequireWeapon(weaponGo);
                weapon.AttachToEventController(this);
                Weapons.Add(weapon);
        }

        private IEnumerator SpawnZombiesCoroutine()
        {
                while (true)
                {
                        yield return new WaitForSeconds(spawnZombieRateInSeconds);
                        if (Zombies.Count < maxZombies)
                        {
                                SpawnZombie();
                        }
                }
        } 

        private void SpawnZombie()
        {
                // Get a random position on the terrain
                Vector3 randomPosition = GetRandomTerrainPosition(ResourcesManager.currentTerrain,MainPlayer.transform.position,lowerLimitDistance:100,minElevation:35);

                // Spawn the zombie at the random position
                GameObject zombieObj = Instantiate(ResourcesManager.zombiePrefab, randomPosition, Quaternion.identity);
                Zombie zombie = U.GetOrAddComponent<Zombie>(zombieObj);
                zombie.attachToEventControlelr(this);
                Zombies.Add(zombie);
        }

        public Vector3 GetRandomTerrainPosition(Vector3 centralPosition = default,
                float lowerLimitDistance = 0f, float upperLimitDistance = float.MaxValue / 2, float minElevation = 5f,float maxElevation = 100f)
        {
                return GetRandomTerrainPosition(ResourcesManager.currentTerrain,centralPosition, lowerLimitDistance, upperLimitDistance,
                        minElevation);
        }

        private static Vector3 GetRandomTerrainPosition(Terrain currentTerrain, Vector3 centralPosition = default,
                float lowerLimitDistance = 0f, float upperLimitDistance = float.MaxValue / 2, float minElevation = 5f, float maxElevation = 100f)
        {
                Vector3 terrainSize = currentTerrain.terrainData.size;
                float randomX, randomZ;
                float terrainHeight = 0f;

                // Loop until a suitable position is found
                do
                {
                        randomX = Random.Range(0f, terrainSize.x);
                        randomZ = Random.Range(0f, terrainSize.z);

                        // Get the height at the random position
                        terrainHeight = currentTerrain.SampleHeight(new Vector3(randomX, 0f, randomZ));
                        
                        // Check if centralPosition is specified
                        if (centralPosition != Vector3.zero)
                        {
                                // Calculate the distance between randomPosition and centralPosition
                                float distance = Vector3.Distance(new Vector3(randomX, 0f, randomZ), centralPosition);

                                // Check if the distance is within the specified limits
                                if (distance < lowerLimitDistance || distance > upperLimitDistance)
                                        continue;
                        }

                } while (terrainHeight<maxElevation && terrainHeight > minElevation);

                // Set the random position with the correct height
                Vector3 randomPosition = new Vector3(randomX, terrainHeight+7, randomZ);

                return randomPosition;
        }
        

        public void DestroyItem(Ammunition ammunition)
        {
                Ammunitions.Remove(ammunition);
                Destroy(ammunition.gameObject);
        }
        public void DestroyItem(RepairObject repairObject)
        {
                RepairObjects.Remove(repairObject);
                Destroy(repairObject.gameObject);
        }
        public void DestroyItem(Weapon weapon)
        {
                Weapons.Remove(weapon);
                Destroy(weapon.gameObject);
        }
        public void DestroyItem(Consumable consumable)
        {
                Consumables.Remove(consumable);
                Destroy(consumable.gameObject);
        }
        public void DestroyItem(Player player)
        {
                switch (player)
                {
                        case MainPlayer mainPlayer:
                                EndGame();
                                break;
                        case Zombie zombie:
                                Zombies.Remove(zombie);
                                Destroy(zombie.gameObject);
                                break;
                        default:
                                throw new Exception("Unknown behaviour for:" + nameof(player));
                }
        }

        private void EndGame()
        {
                throw new NotImplementedException();
        }

        public void WinGame()
        {
                throw new NotImplementedException();
        }

        public void RequireTargetsForCompass(Compass compass)
        {
                //List<RepairObject> randomSampleRepairObjects = repairObjects.OrderBy(item => Random.Range(0, repairObjects.Count)).Take(sampleSize).ToList();
                List<GameObject> repairObjectsEngine = new List<GameObject>();
                List<GameObject> repairObjectsPropeller = new List<GameObject>();
                List<GameObject> repairObjectsGasoline = new List<GameObject>();
                foreach (var repairObject in RepairObjects)
                {
                        switch (repairObject.boatPart)
                        {
                                case RepairObject.BoatPart.Engine:
                                        repairObjectsEngine.Add(repairObject.gameObject);
                                        break;
                                case RepairObject.BoatPart.Propeller:
                                        repairObjectsPropeller.Add(repairObject.gameObject);
                                        break;
                                case RepairObject.BoatPart.Gasoline:
                                        repairObjectsGasoline.Add(repairObject.gameObject);
                                        break;
                                default:
                                        throw new ArgumentOutOfRangeException("Unknown behaviour for the repair object: "+repairObject);
                        }
                }
                repairObjectsEngine = repairObjectsEngine.Take((int)(repairObjectsEngine.Count * 0.1)+1).ToList();
                repairObjectsPropeller = repairObjectsPropeller.Take((int)(repairObjectsPropeller.Count * 0.1)+1).ToList();
                repairObjectsGasoline = repairObjectsGasoline.Take((int)(repairObjectsGasoline.Count * 0.1)+1).ToList();
                compass.RemoveTargets();
                switch (missionStatus)
                {
                        case FindBoatPartsStatus:
                                compass.AddTargets(repairObjectsEngine,Compass.GetIdByColor(Color.yellow));
                                compass.AddTargets(repairObjectsPropeller,Compass.GetIdByColor(Color.blue));
                                compass.AddTargets(repairObjectsGasoline,Compass.GetIdByColor(Color.red));
                
                                compass.AddTargets(new List<GameObject> {Boat.gameObject},Compass.GetIdByColor(Color.black));
                                break;
                        case RepairBoatStatus:
                                compass.AddTargets(new List<GameObject> {Boat.gameObject},Compass.GetIdByColor(Color.black));
                                break;
                }
                
        }
}
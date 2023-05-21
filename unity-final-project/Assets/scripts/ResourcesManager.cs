using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourcesManager:MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject zombiePrefab;
    public Terrain currentTerrain;
    public GameObject mainPlayerPrefab;
    public GameObject machineGunPrefab;
    public GameObject gunPrefab;
    public GameObject shotgunPrefab;
    private List<GameObject> _weapons; 
    public GameObject ammunitionPrefab;
    public GameObject engineBoatPartPrefab;
    public GameObject propellerBoatPartPrefab;
    public GameObject gasolineBoatPartPrefab;

    private void Awake()
    {
        _weapons = new List<GameObject>();
        _weapons.Add(machineGunPrefab);
        _weapons.Add(gunPrefab);
        _weapons.Add(shotgunPrefab);
    }

    public GameObject chooseRandomWeapon()
    {
        return _weapons[Random.Range(0, _weapons.Count)];
    }
    public  GameObject GetBoatPartPrefab(RepairObject.BoatPart boatPart)
    {
        GameObject prefab = null;
        
        switch (boatPart)
        {
            case RepairObject.BoatPart.Engine:
                prefab = engineBoatPartPrefab;
                break;
            case RepairObject.BoatPart.Propeller:
                prefab = propellerBoatPartPrefab;
                break;
            case RepairObject.BoatPart.Gasoline:
                prefab = gasolineBoatPartPrefab;
                break;
            default:
                throw new Exception("Unknown behaviour for : "+ boatPart);
        }

        return prefab;
    }
}
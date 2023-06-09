﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ResourcesManager:MonoBehaviour
{
    public List<GameObject> zombiesPrefabs;
    public GameObject bulletPrefab;
    public Terrain currentTerrain;
    public GameObject mainPlayerPrefab;
    public GameObject machineGunPrefab;
    public GameObject gunPrefab;
    public GameObject shotgunPrefab;
    public GameObject sniperPrefab;
    public GameObject grenadeLauncherPrefab;
    private List<GameObject> _weapons; 
    public GameObject ammunitionPrefab;
    public GameObject engineBoatPartPrefab;
    public GameObject propellerBoatPartPrefab;
    public GameObject gasolineBoatPartPrefab;
    public GameObject boatPrefab;
    [FormerlySerializedAs("SoundsManager")] public SoundsManager SM;
    
    [FormerlySerializedAs("food")] public GameObject foodPrefab;
    [FormerlySerializedAs("water")] public GameObject waterPrefab;
    private List<GameObject> consumablesPrefabs;

    public GameObject chooseRandomZombie()
    {
        return zombiesPrefabs[Random.Range(0, zombiesPrefabs.Count)];
    }
    public GameObject chooseRandomWeapon()
    {
        if (_weapons == null || _weapons.Count==0) {
            _weapons=new List<GameObject> {machineGunPrefab,gunPrefab,shotgunPrefab,sniperPrefab,grenadeLauncherPrefab};
        }
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

    public GameObject chooseRandomConsumable()
    {
        if (consumablesPrefabs == null || consumablesPrefabs.Count==0) {
            consumablesPrefabs=new List<GameObject> {foodPrefab,waterPrefab};
        }
        return consumablesPrefabs[Random.Range(0, consumablesPrefabs.Count)];
    }
}
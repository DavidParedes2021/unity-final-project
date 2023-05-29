using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Zombie : Player
{
    private NavMeshAgent _agent;
    private float _timeSinceLastObjSeen;
    private GameObject enemyObj;
    public Vector3 _randomTerrainPosition;
    private AudioSource zombieMainAudioSource;
    private bool hasRandomDestination = false;
    protected override void Awake()
    {
        Weapons = new List<Weapon>() { new GameObject().AddComponent<ZombieAcid>()};
        base.Awake();
        zombieMainAudioSource = SoundsManager.GetNewASC(this.gameObject);
        CollidableObject.AttachToScript(this.gameObject,nameof(Zombie));
        
        _agent = U.GetOrAddComponent<NavMeshAgent>(gameObject);
        _agent.radius = 0.25f;
        _agent.height = 1;
        
        _timeSinceLastObjSeen = 6f;
    }

    private void Start()
    {
        if (EC == null)
        {
            Debug.LogError("The zombie need a reference to an EC to define trajectory, etc.");
        }
        zombieMainAudioSource.clip = U.RandomElement(EC.RM.SM.zombieNearAudios);
        zombieMainAudioSource.loop = true;
        zombieMainAudioSource.volume = 0.5f;
        zombieMainAudioSource.spatialBlend = 1f;
        zombieMainAudioSource.spread = 180;
        zombieMainAudioSource.maxDistance = 15f;
        enemyObj = EC.MainPlayer.gameObject;
        StartCoroutine(PlayZombieSounds());
        GetRandomDestination();
    }

    private IEnumerator PlayZombieSounds()
    {
        while (true)
        {
            var clipLength = zombieMainAudioSource.clip.length;
            zombieMainAudioSource.Play();
            yield return new WaitForSeconds(clipLength);
            zombieMainAudioSource.Stop();
            yield return new WaitForSeconds(Random.Range(10f,60f));
        }
    }
    private void Update()
    {
        MoveInPath();
        VerifyObjInSight();
    }

    private void VerifyObjInSight()
    {
        // Calculamos la dirección hacia el objetivo
        Vector3 directionToTarget = enemyObj.transform.position - transform.position;

        // Comprobamos si el objetivo está dentro del campo de visión del enemigo
        if (Vector3.Angle(transform.forward, directionToTarget) < 60/*Field Of View*/ / 2f)
        {
            // Comprobamos si no hay obstáculos entre el enemigo y el objetivo
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit))
            {
                zombieMainAudioSource.PlayOneShot(U.RandomElement(EC.RM.SM.zombieInSightOfPlayer));
                if (hit.transform == enemyObj.transform) {
                    // El objetivo está a la vista del enemigo
                    _timeSinceLastObjSeen = 0;
                }
            }
            if (Vector3.Distance(_agent.destination, enemyObj.transform.position) <= 4f)
            {
                TriggerCurrentWeapon();
            }
        }
    }

    private void MoveInPath()
    {
        _timeSinceLastObjSeen += Time.deltaTime;
        if (_timeSinceLastObjSeen is >= 0 and < 5f)
        {
            transform.LookAt(enemyObj.transform);
            if ((int)(_timeSinceLastObjSeen)%2==0)
            {
                _agent.speed = (float)WalkVelocity;
                _agent.destination = enemyObj.transform.position;
                hasRandomDestination = false;
            }
        }
        else
        {
            if (!hasRandomDestination) {
                GetRandomDestination();
            }
        }
        if(hasRandomDestination)//Move to Random Place
        {
            if (Vector3.Distance(_agent.destination, transform.position)<=10f) {
           
                GetRandomDestination();
            }
        }
    }

    private void GetRandomDestination()
    {
        _randomTerrainPosition = EC.GetRandomTerrainPosition(minElevation: 25);
        
        _agent.destination = new Vector3(_randomTerrainPosition.x,_randomTerrainPosition.y+1,_randomTerrainPosition.z);
        _agent.speed = (float)WalkVelocity;
        hasRandomDestination = true;
    }


    public override void AddAmmunition(Ammunition otherAmmunition)
    {
        throw new System.NotImplementedException();
    }

    protected override void UseFood(Consumable consumable)
    {
        throw new System.NotImplementedException();
    }

    protected override void UseWater(Consumable consumable)
    {
        throw new System.NotImplementedException();
    }


    public void Attack(MainPlayer otherMainPlayer)
    {
        TriggerCurrentWeapon();
        SoundsManager.PlayClipAndDestroy( this.gameObject,U.RandomElement(EC.RM.SM.zombieAttacking));
    }

    private void TriggerCurrentWeapon()
    {
        CurrentWeapon.Trigger(this.gameObject, this.transform.position, this.transform.forward);
    }
}
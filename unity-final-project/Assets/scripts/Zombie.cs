using System;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : Player
{
    private NavMeshAgent _agent;
    private float _timeSinceLastObjSeen;
    private GameObject enemyObj;
    public Vector3 _randomTerrainPosition;

    protected override void Awake()
    {
        base.Awake();
        CollidableObject.AttachToScript(this.gameObject,nameof(Zombie));
        
        _agent = U.GetOrAddComponent<NavMeshAgent>(gameObject);
        _agent.radius = 0.25f;
        _agent.height = 1;
        
        _timeSinceLastObjSeen = 6f;
    }

    private void Start()
    {
        if (EventController == null)
        {
            Debug.LogError("The zombie need a reference to an EventController to define trajectory, etc.");
        }

        enemyObj = EventController.MainPlayer.gameObject;
        GetRandomDestination();
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
        if (Vector3.Angle(transform.forward, directionToTarget) < 45/*Field Of View*/ / 2f)
        {
            // Comprobamos si no hay obstáculos entre el enemigo y el objetivo
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit))
            {
                if (hit.transform == enemyObj.transform)
                {
                    // El objetivo está a la vista del enemigo
                    _timeSinceLastObjSeen = 0;
                }
            }
        }else
        {
            // El objetivo está fuera del campo de visión del enemigo
        }

    }

    private void MoveInPath()
    {
        _timeSinceLastObjSeen += Time.deltaTime;
        if (_timeSinceLastObjSeen is >= 0 and < 5f)
        {
            transform.LookAt(enemyObj.transform);
            if (Vector3.Distance(_agent.destination, transform.position)>=5f)
            {
                _agent.destination = enemyObj.transform.position;
            }
        }
        else
        {
            if (_agent.destination == enemyObj.transform.position)
            {
                GetRandomDestination();
            }
        }
        if(_agent.destination!=enemyObj.transform.position)//Move to Random Place
        {
            if (Vector3.Distance(_agent.destination, transform.position)<=2f) {
           
                GetRandomDestination();
            }
        }
        else
        {
            if (Vector3.Distance(_agent.destination, transform.position) <= 2f)//Move to Obj
            {
                //Atack?
            }
        }
    }

    private void GetRandomDestination()
    {
        _randomTerrainPosition = EventController.GetRandomTerrainPosition(minElevation: 25);
        
        _agent.destination = new Vector3(_randomTerrainPosition.x,_randomTerrainPosition.y+1,_randomTerrainPosition.z);
        _agent.speed = (float)WalkVelocity;
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

    public override bool AddWeapon(Weapon weapon)
    {
        throw new System.NotImplementedException();
    }

    public void Attack(MainPlayer otherMainPlayer)
    {
        
    }
}
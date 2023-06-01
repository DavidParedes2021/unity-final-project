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
    private float _timeSinceLastUpdateEnemyPath;
    private GameObject enemyObj;
    public Vector3 _randomTerrainPosition;
    private AudioSource zombieMainAudioSource;
    private bool hasRandomDestination = false;
    public Animator animator;
    public bool hasZombiePath;
    private static readonly int IdleNProperty = Animator.StringToHash("idle n");
    private static readonly int WalkNProperty = Animator.StringToHash("walk n");
    private static readonly int AttackNProperty = Animator.StringToHash("attack n");
    private static readonly int VelocityProperty = Animator.StringToHash("velocity");
    private static readonly int IsMovingProperty = Animator.StringToHash("isMoving");
    private static readonly int IsInAlertProperty = Animator.StringToHash("isInAlert");
    private static readonly int IsAttackingNProperty = Animator.StringToHash("isAttacking");
    
    protected override void Awake()
    {
        var zombieAcid = GetComponent<ZombieAcid>();
        if (zombieAcid==null)
        {
            Debug.LogError("The zombie needs a reference to Zombie Acid");
        }else if (animator == null)
        {
            Debug.LogError("The zombie needs a reference to an Animation Controller");
        }
        CurrentWeapon = zombieAcid;
        base.Awake();
        zombieMainAudioSource = SoundsManager.GetNewASC(this.gameObject);
        CollidableObject.AttachToScript(this.gameObject,nameof(Zombie));
        
        _agent = U.GetOrAddComponent<NavMeshAgent>(gameObject);
        
        _timeSinceLastObjSeen = 1000f;
    }

    private void Start()
    {
        if (EC == null)
        {
            Debug.LogError("The zombie need a reference to an EC to define trajectory, etc.");
        }
        if (_agent == null)
        {
            Debug.LogError("Destroying Zombie, missing agent!");
            Destroy(gameObject);
            return;
        }

        _timeSinceLastUpdateEnemyPath = 0;
        //_agent.updatePosition = false;
        CurrentWeapon.AttachToEventController(EC);
        zombieMainAudioSource.clip = U.RandomElement(EC.RM.SM.zombieNearAudios);
        zombieMainAudioSource.loop = true;
        zombieMainAudioSource.volume = 0.5f;
        zombieMainAudioSource.spatialBlend = 1f;
        zombieMainAudioSource.spread = 360;
        zombieMainAudioSource.maxDistance = 15f;
        enemyObj = EC.MainPlayer.gameObject;
        StartCoroutine(PlayZombieSounds());
        StartCoroutine(ShowAnimations());
        GetRandomDestination();
    }
    void OnAnimatorMove ()
    {
        // Update position to agent position
        //transform.position = _agent.nextPosition;
    }
    private IEnumerator PlayZombieSounds()
    {
        while (true)
        {
            var clipLength = zombieMainAudioSource.clip.length;
            zombieMainAudioSource.Play();
            yield return new WaitForSeconds(clipLength);
            zombieMainAudioSource.Stop();
            yield return new WaitForSeconds(Random.Range(60f,3*60f));
        }
    }
    private void Update()
    {
        MoveInPath();
        VerifyObjInSight();
    }

    private IEnumerator ShowAnimations()
    {
        var maxIdleN = 2;
        var maxWalkN = 3;
        var maxAttackN = 2;
        while (true)
        {
            var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (currentAnimatorStateInfo.normalizedTime >= 1f)
            {
                animator.Play("Zombie Slow Walk");
                yield return new WaitForSeconds(0.1f);
            }
            animator.SetInteger(WalkNProperty,Random.Range(0,maxWalkN));
            animator.SetInteger(IdleNProperty,Random.Range(0,maxIdleN));
            animator.SetInteger(AttackNProperty,Random.Range(0,maxAttackN));
            var distanceToPlayer = Vector3.Distance(_agent.transform.position, enemyObj.transform.position);
            if (isEnemyInVisibleRangeTime()) {
                if (distanceToPlayer <= 3f) {
                    animator.SetBool(IsAttackingNProperty,true);
                    yield return new WaitForSeconds(Random.Range(1f,2f));
                    animator.SetBool(IsAttackingNProperty,false);
                }
                animator.SetBool(IsMovingProperty,true);
                float velocity = Random.Range(5f, 10f);
                animator.SetFloat(VelocityProperty,velocity);
                _agent.speed = velocity;
                yield return new WaitUntil(() => !isEnemyInVisibleRangeTime());
                animator.SetBool(IsMovingProperty,false);
            }else if (distanceToPlayer>=5f && distanceToPlayer <= 10f && Random.Range(0,1f)<=0.5){
                animator.SetBool(IsMovingProperty,false);
                animator.SetBool(IsInAlertProperty,true);
                animator.SetFloat(VelocityProperty,0f);
                _agent.speed = 0f;
                yield return new WaitForSeconds(Random.Range(1f,distanceToPlayer / 3));
            }else {
                if (Random.Range(0, 1f) < 0.2)
                {
                    animator.SetBool(IsMovingProperty,false);
                    animator.SetBool(IsInAlertProperty,true);
                    animator.SetFloat(VelocityProperty,0f);
                    _agent.speed = 0f;
                    yield return new WaitForSeconds(Random.Range(1f, 2f));
                }
                else
                {
                    float velocity = Random.Range(1f, 10f);
                    animator.SetBool(IsMovingProperty,true);
                    animator.SetBool(IsInAlertProperty,false);
                    animator.SetFloat(VelocityProperty,velocity);
                    _agent.speed = velocity;
                    yield return new WaitForSeconds(Random.Range(2f,6f));
                }
            }
        }
    }

    private void VerifyObjInSight()
    {
        // Calculamos la dirección hacia el objetivo
        Vector3 directionToTarget = enemyObj.transform.position - transform.position;

        // Comprobamos si el objetivo está dentro del campo de visión del enemigo
        if (Vector3.Angle(transform.forward, directionToTarget) < 55/*Field Of View*/ / 2f)
        {
            // Comprobamos si no hay obstáculos entre el enemigo y el objetivo
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit))
            {
                if (hit.transform == enemyObj.transform && hit.distance<50f) {
                    if (!isEnemyInVisibleRangeTime())
                    {
                        zombieMainAudioSource.PlayOneShot(U.RandomElement(EC.RM.SM.zombieInSightOfPlayer));
                        _timeSinceLastObjSeen = 0.0f;
                    }
                }
                if (Vector3.Distance(transform.position, enemyObj.transform.position) <= 6f)
                {
                    TriggerCurrentWeapon();
                }
            }
        }
    }

    private IEnumerator MoveToPlayerCoroutine()
    {
        while (true)
        {
            _agent.destination = enemyObj.transform.position;
            hasRandomDestination = false;
            hasZombiePath = true;
            _timeSinceLastUpdateEnemyPath = 0;
            yield return new WaitForSeconds(5 + Random.Range(-0.9f, 0.9f));
        }
    }

    private void MoveInPath()
    {
        
        _timeSinceLastObjSeen += Time.deltaTime;
        _timeSinceLastUpdateEnemyPath += Time.deltaTime;
        return;
        if (isEnemyInVisibleRangeTime())
        {
            transform.LookAt(enemyObj.transform);
            if (hasZombiePath == false) {
                _agent.destination = enemyObj.transform.position;
                hasRandomDestination = false;
                hasZombiePath = true;
                _timeSinceLastUpdateEnemyPath = 0;
            }
            
            if (_timeSinceLastUpdateEnemyPath>2.5f + Random.Range(0.25f,0.5f) || Vector3.Distance(_agent.destination, enemyObj.transform.position) > 3f) {
                _agent.destination = enemyObj.transform.position;
                hasZombiePath = true;
                hasRandomDestination = false;
                _timeSinceLastUpdateEnemyPath = 0;;
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

    private bool isEnemyInVisibleRangeTime()
    {
        return _timeSinceLastObjSeen is >= 0 and < 10f;
    }

    private void GetRandomDestination()
    {
        _randomTerrainPosition = EC.GetRandomTerrainPosition(minElevation: 25);
        _agent.destination = new Vector3(_randomTerrainPosition.x,_randomTerrainPosition.y+1,_randomTerrainPosition.z);
        hasRandomDestination = true;
        hasZombiePath = false;
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
        otherMainPlayer.TakeDamage(20f);
        TriggerCurrentWeapon();
        SoundsManager.PlayClipAndDestroy( this.gameObject,U.RandomElement(EC.RM.SM.zombieAttacking));
    }

    private void TriggerCurrentWeapon()
    {
        CurrentWeapon.Trigger(this.gameObject, this.transform.position, this.transform.forward);
    }

    public NavMeshAgent GetAgent()
    {
        return _agent;
    }
}
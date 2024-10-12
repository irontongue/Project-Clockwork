using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AIBase : EnemyInfo
{

    protected GameObject player;
    [SerializeField] public NavMeshAgent agent;
    [Header("Attack Settings")]

    [SerializeField, TabGroup("Base AI")] protected float attackRange;
    [SerializeField, TabGroup("Base AI")] protected float seccondsBetweenAttacks;
    [SerializeField, TabGroup("Base AI")] protected float damage = 1;
    [SerializeField, TabGroup("Base AI")] protected float attackDelay = 0.1f;
    [Header("Audio Settings")]
    [SerializeField, TabGroup("Base AI")] AudioClip[] attackSounds;
    [Header("Agent Settings")]
    [SerializeField, TabGroup("Base AI")] protected LayerMask walkableMask;
    [Header("AttackAnim")]
    [SerializeField, TabGroup("Base AI")] protected Sprite baseSprite, attackSprite;
    [SerializeField, TabGroup("Base AI")] float seccondsBetweenMovementUpdates = 0.25f, randonVarianceForMovementUpdate = 0.02f;
    protected float trueSeccondsBetweenMovementUpdates;
    protected float lastTimeSinceMovementUpdate;
    public EnemySpawner spawner;
    AudioSource source;
 
    

    

     public bool aiBuisy = false;
 
    protected float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, PlayerMovement.playerPosition);
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected override void Start()
    {
        base.Start();
        trueSeccondsBetweenMovementUpdates = Random.Range(seccondsBetweenAttacks * (1 - randonVarianceForMovementUpdate), seccondsBetweenAttacks * (1 + randonVarianceForMovementUpdate));
        player = FindAnyObjectByType<PlayerMovement>().gameObject;
        aiBuisy = false;
        source = GetComponent<AudioSource>();
        BatchSpriteLooker.AddLooker(transform);
    }
 

    protected override void Update()
    {
       // transform.LookAt(new Vector3(player.transform.position.x,0f, player.transform.position.z));
        base.Update();
       // transform.LookAt(player.transform.position);

        if (pathingToPoint)
        {
            if (agent.remainingDistance > 0.1f)
                return;

            aiBuisy = false;
        }

        lastTimeSinceMovementUpdate -= Time.deltaTime;
       
    }
    bool pathingToPoint;
    public void PathToPoint(Vector3 point)
    {
        if(!ReadyToMove())
            return;

        aiBuisy = true;
        agent.SetDestination(point);
        
    }
    bool ReadyToMove()
    {
        if(lastTimeSinceMovementUpdate <= 0)
        {
            lastTimeSinceMovementUpdate = trueSeccondsBetweenMovementUpdates;
            return true;
        }
        return false;
    }
    protected bool PathToPlayer(float minDistance)
    {
        if(DistanceToPlayer() > minDistance  && ReadyToMove() )
        {
        
            agent.SetDestination(player.transform.position);
            return false;
           
        }  
     
        return true;
    }

    protected void PathAwayFromPlayer(float runDistance)
    {
        if(!ReadyToMove())
            return;
        NavMesh.SamplePosition(transform.position + (Vector3.back * runDistance), out NavMeshHit hit, runDistance, walkableMask);
        agent.SetDestination(hit.position);
        
    }
    protected bool CanDamagePlayer()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
            return true;

        return false;
    }
  
    protected void AttackEffects()
    {
        try
        {
            source.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length - 1)]);
        }
        catch { }
    }
   
    protected virtual void DamagePlayer()
    {
        spriteRenderer.sprite = attackSprite;
        Invoke("FinalizeDamage", attackDelay);
        
    }

    protected virtual void FinalizeDamage()
    {
        AttackEffects();
        spriteRenderer.sprite = baseSprite;
        player.GetComponent<PlayerDamageHandler>().Damage(damage, transform);
    }
    protected override void DeathEvent()
    {
        BatchSpriteLooker.AddLooker(transform);
        player.GetComponent<PlayerLevelUpManager>().ReciveEXP(EXP);
        spawner.EnemyKilled();
        base.DeathEvent();
    }
    readonly Vector3 zeroPosition = new Vector3(-100,-100,-100);
    public override void DecomissionObject()
    {
        base.DecomissionObject();
        // agent.enabled = false;
        // aiBuisy = true;
        // spriteRenderer.enabled = false;
       
        gameObject.SetActive(false);
    }
    public override void ReuseObject()
    {
        HealHealth(maxHealth);
        damageFlashTime = 1;

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AIBase : EnemyInfo
{
    public enum Pole {Null, North, South, East, West}

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
    [SerializeField, TabGroup("Base AI")] float seccondsBetweenMovementUpdates = 0.25f, randonVarianceForMovementUpdate = 0.02f; // how often does the ai repath to the player. 
    [Header("RandomMovement")]
    [SerializeField, TabGroup("Base AI")] bool usePolarOffset;
    [SerializeField, TabGroup("Base AI")] float distanceBeforeConvergingOnPlayer;// if using the pole offsets, how close to the player untill you stop moving towards the offset position
    [SerializeField, TabGroup("Base AI")] Vector2 poleOffsets;// decides how far the enemy moves to the N W S E of the player, if using pole offsets
    protected float trueSeccondsBetweenMovementUpdates; // this is the seccondsbetweenmovementupdates, but with the random offset added, so its not recaculated constantly
    protected float lastTimeSinceMovementUpdate;
   
    public EnemySpawner spawner;
    AudioSource source;
    
   [ReadOnly] public Pole pole;

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
        player = PlayerMovement.playerRef.gameObject;
        aiBuisy = false;
        source = GetComponent<AudioSource>();
        BatchSpriteLooker.AddLooker(transform);
    }

    protected override void Update()
    {
        base.Update();

        if (pathingToPoint)
        {
            if (agent.remainingDistance > 0.1f)
                return;

            aiBuisy = false;
        }

        lastTimeSinceMovementUpdate -= Time.deltaTime;
       
    }
    bool pathingToPoint;
    // this allows you to path to a custom set point
    public void PathToPoint(Vector3 point)
    {
        if(!ReadyToMove())
            return;

        aiBuisy = true;
        agent.SetDestination(point);
        
    }
    //see if the ai is ready to move or not
    bool ReadyToMove()
    {
        if(lastTimeSinceMovementUpdate <= 0)
        {
            lastTimeSinceMovementUpdate = trueSeccondsBetweenMovementUpdates;
            return true;
        }
        return false;
    }
    Vector3 targetPos;
    float distanceToPlayer;
    //paths to the player, returns true if not pathing to player, false if so.... why did i do it like this
    protected bool PathToPlayer(float minDistance, float randomVariance = 0)
    {
        distanceToPlayer = DistanceToPlayer();

        if (distanceToPlayer > minDistance && ReadyToMove())
        {
            if (distanceToPlayer < distanceBeforeConvergingOnPlayer) 
                targetPos = PlayerMovement.playerPosition;
            else
                targetPos = GetPlayerOffset();

            if (randomVariance == 0)
            {
                agent.SetDestination(targetPos);
                return false;
            }

   
            targetPos.x += Random.Range(-randomVariance, randomVariance);
            targetPos.z += Random.Range(-randomVariance, randomVariance);
            agent.SetDestination(targetPos);
            return false;
        }  
     
        return true;
    }
    Vector3 modifyedPlayerVector;
 
    Vector3 GetPlayerOffset() // if using the pole offset, this adds the offset, if not it just returns the player position
    {
        modifyedPlayerVector = PlayerMovement.playerPosition;
        switch (pole)
        {
            case Pole.North:
                modifyedPlayerVector.x += poleOffsets.y;
                return modifyedPlayerVector;
            case Pole.South:
                modifyedPlayerVector.x -= poleOffsets.y;
                return modifyedPlayerVector;
            case Pole.East:
                modifyedPlayerVector.x += poleOffsets.x;
                return modifyedPlayerVector;
            case Pole.West:
                modifyedPlayerVector.x -= poleOffsets.x;
                return modifyedPlayerVector;
            case Pole.Null:
                return PlayerMovement.playerPosition;

        }
        return PlayerMovement.playerPosition;
    }
    public void RandomisePolarOffset()
    {
        if (!usePolarOffset)
            return;

        pole = (Pole)Random.Range(1, 4);
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
        RandomisePolarOffset();
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
        try
        {
            spawner.EnemyKilled();
        }
        catch { }
        
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
        flashTimer = 1;

    }


}

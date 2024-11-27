using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AIBase : EnemyInfo
{
    public enum Pole {Null, North, South, East, West}

    protected GameObject player;
    public NavMeshAgent agent;
    public bool agentless;

    [Header("Attack Settings")]

    [SerializeField, TabGroup("Attack")] protected float attackRange;
    [SerializeField, TabGroup("Attack")] protected float seccondsBetweenAttacks;
    [SerializeField, TabGroup("Attack")] protected float damage = 1;
    [SerializeField, TabGroup("Attack")] protected float attackDelay = 0.1f;
    //[Header("Audio Settings")]
    [SerializeField, TabGroup("Effects")] AudioClip[] attackSounds;
    [SerializeField, TabGroup("Effects")] GameObject spawnParticleSystem;
   // [Header("Agent Settings")]
    [SerializeField, TabGroup("Settings")] protected LayerMask walkableMask;
  //  [Header("AttackAnim")]
    [SerializeField, TabGroup("Effects")] protected Sprite baseSprite, attackSprite;
    [SerializeField, TabGroup("Movement")] float seccondsBetweenMovementUpdates = 0.25f, randonVarianceForMovementUpdate = 0.02f; // how often does the ai repath to the player. 
   // [Header("RandomMovement")]
    [SerializeField, TabGroup("Movement")] bool usePolarOffset;
    [SerializeField, TabGroup("Movement")] float distanceBeforeConvergingOnPlayer;// if using the pole offsets, how close to the player untill you stop moving towards the offset position
    [SerializeField, TabGroup("Movement")] Vector2 poleOffsets;// decides how far the enemy moves to the N W S E of the player, if using pole offsets
    protected float trueSeccondsBetweenMovementUpdates; // this is the seccondsbetweenmovementupdates, but with the random offset added, so its not recaculated constantly
    protected float lastTimeSinceMovementUpdate;
   
    [TabGroup("RunTimeOnly")]public EnemySpawner spawner;
    [SerializeField]AudioSource source;
    
   [ReadOnly, TabGroup("RunTimeOnly")] public Pole pole;

    [TabGroup("RunTimeOnly")]public bool aiBuisy = false;

    [TabGroup(("Audio"))] [SerializeField] AudioClip[] spawnSounds, deathSounds;
    [TabGroup("Audio"), SerializeField] AudioClip[] spawnerSpawnSound;
 
    protected float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, PlayerMovement.playerPosition);
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    ParticleSystem _particleSystem;
    protected override void Start()
    {
      
        base.Start();
        identifyer = enemyType.ToString();
        trueSeccondsBetweenMovementUpdates = Random.Range(seccondsBetweenMovementUpdates * (1f - randonVarianceForMovementUpdate), seccondsBetweenMovementUpdates * (1f + randonVarianceForMovementUpdate));
        player = PlayerMovement.playerRef;
        aiBuisy = false;
        source = GetComponent<AudioSource>();
        BatchSpriteLooker.AddLooker(transform);
        if(!agentless)
            agent.speed = speed;

        lastTimeSinceFiredProjectile = projectileFireRate;

        if (spawnParticleSystem != null)
            _particleSystem = Instantiate(spawnParticleSystem, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        else
            print(enemyType.ToString() + " Does not have a spawn particle");
        
    }
    protected float lastTimeSinceFiredProjectile;
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
        lastTimeSincePolarOffset += Time.deltaTime;

        if (!useProjectile)
            return;

        if (DistanceToPlayer() < minRangeToThrowProjectile)
            return;

        lastTimeSinceFiredProjectile -= Time.deltaTime;

        if (lastTimeSinceFiredProjectile > 0)
            return;

        lastTimeSinceFiredProjectile = projectileFireRate;
        float random = Random.Range(0, 100);

        if (random > projectileFireChance)
            return;
    
        FireProjectile();

 
       
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
                print("Moving");
                return false;
            }

   
            targetPos.x += Random.Range(-randomVariance, randomVariance);
            targetPos.z += Random.Range(-randomVariance, randomVariance);
            agent.SetDestination(targetPos);
            return false;
        }  
     
        return true;
    }


    [SerializeField, TabGroup("Movement")] float timeBetweenPolarOffsets;
    float lastTimeSincePolarOffset;
    Vector3 lastOffset;
    Vector3 GetPlayerOffset() // if using the pole offset, this adds the offset, if not it just returns the player position
    {
        if (!usePolarOffset)
            return player.transform.position;
        if (lastTimeSincePolarOffset < timeBetweenPolarOffsets)
            return player.transform.position + lastOffset;

  
 
        lastTimeSincePolarOffset = 0;
        lastOffset.x = 0;
        lastOffset.z = 0;
        switch (pole)
        {
            case Pole.South:
            case Pole.North:
                lastOffset.x += Random.Range(-poleOffsets.y,poleOffsets.y);
                break;

            case Pole.West:
            case Pole.East:
                lastOffset.z += Random.Range(-poleOffsets.x, poleOffsets.x);
                break;

            case Pole.Null:
                return PlayerMovement.playerPosition;

        }
        return player.transform.position + lastOffset;
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

    protected void PlayDeathSound()
    {
      
        if (source == null || deathSounds != null)
            return;
        if (EnemySoundManager.NoiseReady(enemyType, true))
            AudioSource.PlayClipAtPoint(deathSounds[Random.Range(0, deathSounds.Length - 1)],transform.position, GlobalSettings.audioVolume * 2);



    }
    public void PlaySpawnSound()
    {
        if (source == null)
            return;
        if (EnemySoundManager.NoiseReady(enemyType, false))
            source.PlayOneShot(spawnSounds[Random.Range(0, spawnSounds.Length - 1)], GlobalSettings.audioVolume);
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
    [SerializeField] GameObject expOrb;
    [SerializeField] bool spawnEXPorb;
    protected override void DeathEvent()
    {
        BatchSpriteLooker.AddLooker(transform);
        if(spawnEXPorb)
        {
            GameObject g = Instantiate(expOrb, transform.position, Quaternion.identity);
            g.GetComponent<EXPOrb>().expValue = EXP;
        }    
        else
            player.GetComponent<PlayerLevelUpManager>().ReciveEXP(EXP);
        PlayDeathSound();
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
        if(_particleSystem != null)
        {
            _particleSystem.transform.position = transform.position;
            _particleSystem.Play();
        }
        try
        {
            source.PlayOneShot(spawnerSpawnSound[Random.Range(0, spawnerSpawnSound.Length)], GlobalSettings.audioVolume);
        }
        catch
        {
            print("spawnerSpawnSound not set for" + enemyType.ToString());
        }
        


      
     

    }


    Projectile spawnedProjectile;

   
    [SerializeField][TabGroup("Projectile")] protected bool useProjectile;
    [SerializeField][TabGroup("Projectile")] protected float projectileFireRate;
    [SerializeField][TabGroup("Projectile")] protected float projectileFireChance;
    [SerializeField][TabGroup("Projectile")] protected float projectileDamage, projectileSpeed;
    [SerializeField][TabGroup("Projectile")] protected float minRangeToThrowProjectile;
    [SerializeField][TabGroup("Projectile")] protected string projectileType;
    [SerializeField][TabGroup("Projectile")] protected int projectileCount = 1;
    [SerializeField][TabGroup("Projectile")] protected float projectileSpawnOffset;

    Vector3 offset;
    protected void FireProjectile()
    {
        AttackEffects();

        if (projectileCount <= 1)
        {
            SpawnProjectile(Vector3.zero);
            return;
        }
        
        for(int i = 0 - (projectileCount / 2); i < (projectileCount / 2) + 1; i++)
        {
            offset = projectileSpawnOffset * transform.right * (projectileSpawnOffset * i);
            SpawnProjectile(offset);
        }

    }
   
    void SpawnProjectile(Vector3 spawnOffset)
    {
        
        spawnedProjectile = ObjectPooler.RetreiveObject(projectileType).GetComponent<Projectile>(); //Instantiate(projectile, transform.position, transform.rotation).GetComponent<Projectile>();
        spawnedProjectile.gameObject.SetActive(true);
        spawnedProjectile.transform.position = transform.position + spawnOffset;
        spawnedProjectile.transform.LookAt(player.transform.position + spawnOffset);
        spawnedProjectile.damage = projectileDamage;
        spawnedProjectile.speed = projectileSpeed;
        spawnedProjectile.origin = gameObject;

        spriteRenderer.sprite = baseSprite;
    }


}

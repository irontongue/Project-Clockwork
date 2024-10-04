using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class AIBase : EnemyInfo
{

    protected GameObject player;
    [SerializeField] protected NavMeshAgent agent;
    [Header("Attack Settings")]

    [SerializeField, TabGroup("Base AI")] protected float attackRange;
    [SerializeField, TabGroup("Base AI")] protected float seccondsBetweenAttacks;
    [SerializeField, TabGroup("Base AI")] protected float damage = 1;
    [Header("Audio Settings")]
    [SerializeField, TabGroup("Base AI")] AudioClip[] attackSounds;
    [Header("Agent Settings")]
    [SerializeField, TabGroup("Base AI")] protected LayerMask walkableMask;
    EnemySpawner spawner;
    AudioSource source;
    

    

     public bool aiBuisy = false;
 
    protected float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }
    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().gameObject;
        aiBuisy = false;
        spawner = FindAnyObjectByType<EnemySpawner>();
        source = GetComponent<AudioSource>();
    }
 

    protected virtual void Update()
    {
        //transform.LookAt(new Vector3(player.transform.position.x,0f, player.transform.position.z));
        transform.LookAt(player.transform.position);

        if (pathingToPoint)
        {
            if (agent.remainingDistance > 0.1f)
                return;

            aiBuisy = false;
        }
       
    }
    bool pathingToPoint;
    public void PathToPoint(Vector3 point)
    {
        aiBuisy = true;
        agent.SetDestination(point);
        
    }
    protected bool PathToPlayer(float minDistance)
    {
        if(DistanceToPlayer() > minDistance)
        {
            agent.SetDestination(player.transform.position);
            return false;
           
        }  
     
        return true;
    }

    protected void PathAwayFromPlayer(float runDistance)
    {
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
        AttackEffects();

        player.GetComponent<PlayerDamageHandler>().Damage(damage,transform);
        
    }
    protected override void DeathEvent()
    {
        player.GetComponent<PlayerLevelUpManager>().ReciveEXP(EXP);
        spawner.EnemyKilled();
        base.DeathEvent();
    }

}

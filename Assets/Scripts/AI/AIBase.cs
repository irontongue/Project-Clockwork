using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBase : EnemyInfo
{

    GameObject player;
    [SerializeField] protected NavMeshAgent agent;
    [Header("Attack Settings")]

    [SerializeField] protected float attackRange;
    [SerializeField] protected float seccondsBetweenAttacks;
    [Header("Agent Settings")]
    [SerializeField] LayerMask walkableMask;

    protected bool aiBuisy;
    float lastTimeSinceAttack;
    protected float DistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.transform.position);
    }
    protected virtual void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().gameObject;
  
    }
 

    protected virtual void Update()
    {
        if(pathingToPoint)
        {
            if (agent.remainingDistance > 0.1f)
                return;

            aiBuisy = false;
        }
        lastTimeSinceAttack -= Time.deltaTime;
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
    protected bool ReadyToAttack()
    {
        if (lastTimeSinceAttack > 0)
            return false;
        return true;
    }

    protected void DamagePlayer()
    {
        lastTimeSinceAttack = seccondsBetweenAttacks;
    }
}

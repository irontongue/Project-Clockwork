using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class RangedEnemy : AIBase
{
    enum State { Idle, ApproachingPlayer,RuningFromPlayer, Attacking, Reposition}
    [SerializeField]State state = State.Idle;

  
    [Header("Attack")]
    [SerializeField,TabGroup("Ranged AI")] float afterAttackTimer; // how long do i wait after an attack to do somthing
    [Header("Approach Settings")]
    [SerializeField, TabGroup("Ranged AI")] float findNewPositionRangeFromPlayer; // if the player goes out of range, how close should i get to the player
    [SerializeField, TabGroup("Ranged AI")] float findNewPositionRangeRandomMultiplyer; // the +- multiplyer on the range
    [SerializeField, TabGroup("Ranged AI")] float seekPlayerRandomError;// offset distance for how close the ai gets to the play +- units
    [Header("Flee Settings")]
    [SerializeField, TabGroup("Ranged AI")] float playerTooCloseRange; // how close does the player have to get till i run
    [SerializeField, TabGroup("Ranged AI")] float findNewPosRange; // how far away should i look for a new range if the player aproaches me
    [SerializeField, TabGroup("Ranged AI")] int findNewPosTrys; // how many times should i search for a new pos


  
  


    [Header("Reposition Settings")]
    [SerializeField, TabGroup("Ranged AI")] float repositionDistance;
    [SerializeField, TabGroup("Ranged AI")] float timeInRepositionState;
    [SerializeField, TabGroup("Ranged AI")] int findPositionTries;
    [SerializeField, TabGroup("Ranged AI")] float cannotSeePlayerRepositionMultiplyer;
    //  [SerializeField] float cantSeePlayerTriesBeforeLeavingAttack;

    bool couldNotSeePlayer = false;

    protected override void Update()
    {
       // transform.LookAt(player.transform); // dont want this to be here, but for some reason the batch sprite looker doesnt work on the AI
        base.Update();

        if (aiBuisy || !agent.enabled)
            return;

        timeSinceLastAttack -= Time.deltaTime;
        timeSinceLastReposition -= Time.deltaTime;
        currentAfterAttackDelay -= Time.deltaTime;
        if (currentAfterAttackDelay > 0)
            return;
        switch (state)
        {
            
            case State.Idle:
                Idle();
                break;
            case State.RuningFromPlayer:
                RunFromPlayer();
                break;
            case State.ApproachingPlayer:
                GetInRangeOfPlayer();
                break;
            case State.Attacking:
                if (timeSinceLastAttack > 0)
                    return;
                Attacking();
                break;
            case State.Reposition:
                if (couldNotSeePlayer)
                    Repositioning(cannotSeePlayerRepositionMultiplyer);
                else
                    Repositioning();
                break;
        }
    }
    float playerDistance;
    void Idle()
    {
        playerDistance = DistanceToPlayer();
        if (playerDistance < playerTooCloseRange)
        {
            findingNewPos = false;
            state = State.RuningFromPlayer;
        }
        else if (playerDistance > attackRange)
        {
            startedMovingToPlayer = false;
            state = State.ApproachingPlayer;
        }
        else if(timeSinceLastAttack >= 0)
        {     
            if (!hasRepositioned)
                state = State.Reposition;
            attacked = false;
        }
        else
            state = State.Attacking;
    }
    bool findingNewPos;
    Vector3 newPos;

    private void RunFromPlayer()
    {
        RunFromPlayer(transform);
    }
    NavMeshHit hit;
    void RunFromPlayer(Transform transform)
    {
        if (!findingNewPos)
        {
            findingNewPos = true;
            int i = 0;

            while (i < findNewPosTrys)
            {
                Vector3 testPos = new Vector3((transform.position.x ) + (transform.forward.x * -findNewPosRange), transform.position.y, (transform.position.z) + (transform.forward.z * -findNewPosRange));
                NavMesh.SamplePosition(testPos, out hit, findNewPosRange, walkableMask);
               
                if (DistanceToPlayer() < Vector3.Distance(transform.position, hit.position))
                {
                    newPos = hit.position;
                    
                    agent.isStopped = false;
                    agent.SetDestination(newPos);
                 
                   
                    break;                 
                }
                i++;
            }
            if (!findingNewPos)
            {
                state = State.Attacking;
                return;
            }
        }

        if (agent.remainingDistance > 1)
            return;

        agent.isStopped = true;
        state = State.Attacking;
    }
    bool startedMovingToPlayer;
    float chosenRange;
    
    void GetInRangeOfPlayer()
    {
        if(!startedMovingToPlayer)
        {
            startedMovingToPlayer = true;
            chosenRange = findNewPositionRangeFromPlayer * Random.Range(1 - findNewPositionRangeRandomMultiplyer, 1 + findNewPositionRangeRandomMultiplyer);
            agent.isStopped = false;
        }
        PathToPlayer(chosenRange, seekPlayerRandomError);
        if(agent.remainingDistance < chosenRange)
        {
            agent.isStopped = true;
            state = State.Attacking;
        }
    }
    bool attacked;
    float timeSinceLastAttack = 0;
    
    //float noVisionTries;


    bool hasRepositioned;
    protected override void FinalizeDamage()
    {
        AttackEffects();
        FireProjectile();

        spriteRenderer.sprite = baseSprite;
    }
    float currentAfterAttackDelay;
    Vector3 vectorToPlayer;
    void Attacking()
    {
        if(playerDistance > attackRange)
        {
            state = State.Idle;
            return;
        }
        agent.isStopped = true;
        
        if (!attacked)
        {
            vectorToPlayer.x = PlayerMovement.playerPosition.x - transform.position.x;
            vectorToPlayer.y = PlayerMovement.playerPosition.y - transform.position.y;
            vectorToPlayer.z = PlayerMovement.playerPosition.z - transform.position.z;
            vectorToPlayer.Normalize();
            if (!Physics.Raycast(transform.position, vectorToPlayer, out RaycastHit hit, attackRange) || !hit.transform.CompareTag("Player"))
            {
                couldNotSeePlayer = true;
                state = State.Reposition;
                agent.isStopped = false;
                attacked = false;
                return;
            }
        
               
   
            DamagePlayer();
            attacked = true;
            state = State.Idle;
            agent.isStopped = false;
            timeSinceLastAttack = seccondsBetweenAttacks;
            currentAfterAttackDelay = afterAttackTimer;
            hasRepositioned = false;
        }
    }

    Vector3 randomPos;
    bool repositioning;
    int i = 0;
    NavMeshHit navMeshHit;
    float timeSinceLastReposition;
    void Repositioning(float distanceMultiplyer = 1)
    {
        if(!repositioning)
        {
            i = 0;
            while(i < findPositionTries)
            {
                randomPos.x = transform.position.x + Random.Range(-repositionDistance, repositionDistance) * distanceMultiplyer;
                randomPos.y = transform.position.y;
                randomPos.z = transform.position.z + Random.Range(-repositionDistance, repositionDistance) * distanceMultiplyer;

                if (NavMesh.SamplePosition(randomPos, out navMeshHit, repositionDistance, walkableMask))
                {
                    repositioning = true;
                    agent.SetDestination(navMeshHit.position);
                    timeSinceLastReposition = timeInRepositionState;
                    return;
                }
                i++;
            }
        }
        if (timeSinceLastReposition < 0)
        {
            hasRepositioned = true;
            repositioning = false;
            state = State.Idle;
        }
    }
}

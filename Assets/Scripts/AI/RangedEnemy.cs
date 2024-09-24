using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class RangedEnemy : AIBase
{
    enum State { Idle, FindingNewPos,RuningFromPlayer, Attacking }
    [SerializeField]State state = State.Idle;

  
    [Header("Attack")]
    [SerializeField,TabGroup("Ranged AI")] float afterAttackTimer; // how long do i wait after an attack to do somthing
    [Header("Approach Settings")]
    [SerializeField, TabGroup("Ranged AI")] float findNewPositionRangeFromPlayer; // if the player goes out of range, how close should i get to the player
    [SerializeField, TabGroup("Ranged AI")] float findNewPositionRangeRandomMultiplyer; // the +- multiplyer on the range
    [Header("Flee Settings")]
    [SerializeField, TabGroup("Ranged AI")] float playerTooCloseRange; // how close does the player have to get till i run
    [SerializeField, TabGroup("Ranged AI")] float findNewPosRange; // how far away should i look for a new range if the player aproaches me
    [SerializeField, TabGroup("Ranged AI")] int findNewPosTrys; // how many times should i search for a new pos

    [SerializeField, TabGroup("Ranged AI")] GameObject projectile;
    [SerializeField, TabGroup("Ranged AI")] float projectileSpeed;
  //  [SerializeField] float cantSeePlayerTriesBeforeLeavingAttack;


    protected override void Update()
    {
        base.Update();
        if (aiBuisy || !agent.enabled)
            return;
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.RuningFromPlayer:
                RunFromPlayer();
                break;
            case State.FindingNewPos:
                GetInRangeOfPlayer();
                break;
            case State.Attacking:
                Attacking();
                break;
        }
    }

    void Idle()
    {
      
        if (DistanceToPlayer() < playerTooCloseRange)
        {
            findingNewPos = false;
            state = State.RuningFromPlayer;
        }
        else if (DistanceToPlayer() > attackRange)
        {
            startedMovingToPlayer = false;
            state = State.FindingNewPos;
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

    void RunFromPlayer(Transform transform)
    {
     
        if (!findingNewPos)
        {
            findingNewPos = true;
            int i = 0;

            while (i < findNewPosTrys)
            {
                Vector2 randomPos = Random.insideUnitCircle * findNewPosRange;
                Vector3 testPos = new Vector3((transform.position.x ) + (transform.forward.x * -findNewPosRange), transform.position.y, (transform.position.z) + (transform.forward.z * -findNewPosRange));
                NavMesh.SamplePosition(testPos, out NavMeshHit hit, findNewPosRange, walkableMask);
               
                if (DistanceToPlayer() < Vector3.Distance(transform.position, hit.position))
                {
                    newPos = hit.position;
                    
                    agent.isStopped = false;
                    agent.SetDestination(newPos);
                    print(newPos);
                   
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
        agent.SetDestination(player.transform.position);
        if(agent.remainingDistance < chosenRange)
        {
            agent.isStopped = true;
            state = State.Attacking;
        }
    }
    bool attacked;
    float timeSinceLastAttack;
    //float noVisionTries;

    protected override void DamagePlayer()
    {
        AttackEffects();
        GameObject spawnedProjectile = Instantiate(projectile, transform.position, transform.rotation);
        spawnedProjectile.transform.LookAt(player.transform.position);
        spawnedProjectile.GetComponent<Projectile>().damage = damage;
        spawnedProjectile.GetComponent<Projectile>().speed = -projectileSpeed;
        print(spawnedProjectile.name);
    }
    void Attacking()
    {
        print("Attacking");
        if(DistanceToPlayer() > attackRange)
        {
            state = State.Idle;
            return;
        }
        agent.isStopped = true;
        if (!attacked)
        {
            print("tryattack");
            if (!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange))
            {
                print("Failed to find player");
                state = State.Idle;
                agent.isStopped = false;
                attacked = false;
                return;
                // ill impiment a better function for this later. 
            }

     
            if (!hit.transform.GetComponentInParent<PlayerMovement>())
                return;
            

            DamagePlayer();
        
            attacked = true;
            timeSinceLastAttack = afterAttackTimer;
        }

        timeSinceLastAttack -= Time.deltaTime;

        if (timeSinceLastAttack < 0)
        {
            attacked = false;
            state = State.Idle;
      
        }
    }
}

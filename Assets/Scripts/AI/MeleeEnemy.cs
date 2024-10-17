using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MeleeEnemy : AIBase
{
    protected enum State { Idle, Chasing, Attacking}

    [ShowInInspector, ReadOnly, TabGroup("RunTimeOnly")] protected State state = State.Idle;

    [SerializeField, TabGroup("Attack")] float afterAttackTimer;
 
   
    protected override void Update()
    {
        base.Update();
        if (aiBuisy || !agent.enabled)
            return;

        switch(state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Chasing:
                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
        }
    }
    protected void Idle()
    {
        if (DistanceToPlayer() > attackRange)
            state = State.Chasing;
        else
            state = State.Attacking;
    }
    protected void Chasing()
    {
        PathToPlayer(attackRange);
        if (DistanceToPlayer() < attackRange)
            state = State.Attacking;
    }
    bool attacked;
    float timeSinceLastAttack;
    protected void Attacking()
    {
        agent.isStopped = true;
        agent.destination = transform.position;
        if(!attacked)
        {
            DamagePlayer();
            attacked = true;
            timeSinceLastAttack = afterAttackTimer;
        }

        timeSinceLastAttack -= Time.deltaTime;

        if(timeSinceLastAttack < 0)
        {
            attacked = false;
            agent.isStopped = false;
            state = State.Idle;
        }
    }
}

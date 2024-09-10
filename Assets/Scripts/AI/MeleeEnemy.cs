using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : AIBase
{
    enum State { Idle, Chasing, Attacking}
    State state = State.Idle;

    [SerializeField] float afterAttackTimer;

    protected override void Update()
    {
        base.Update();
        if (aiBuisy)
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

        print(state);
    }

    void Idle()
    {
        if (DistanceToPlayer() > attackRange)
            state = State.Chasing;
        else
            state = State.Attacking;
    }
    void Chasing()
    {
        
        agent.isStopped = false;
        if (PathToPlayer(attackRange))
            state = State.Attacking;
    }
    bool attacked;
    float timeSinceLastAttack;
    void Attacking()
    {
        agent.isStopped = true;
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
            state = State.Idle;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SlugMeleeEnemy : MeleeEnemy
{
    [SerializeField, TabGroup("Slug")] float moveCycleRate;
    float maxSpeed;

    protected override void Start()
    {
        base.Start();
        maxSpeed = speed;
    }
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
            case State.Chasing:
                speed = Mathf.PingPong(Time.time * moveCycleRate, maxSpeed) ;
                agent.speed = speed;
                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
        }



    }
}

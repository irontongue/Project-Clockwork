using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SlugMeleeEnemy : MeleeEnemy
{
    [SerializeField, TabGroup("Slug")] float moveCycleRate;
    [SerializeField, TabGroup("Slug")] float moveCycleMultiplyer;
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
                float oscillation =  Mathf.Sin(Time.time * moveCycleRate) * moveCycleMultiplyer;
                agent.speed = Mathf.Clamp( speed + oscillation, speed, Mathf.Infinity);

                Chasing();
                break;
            case State.Attacking:
                Attacking();
                break;
        }



    }

    public static float EaseOutExpo(float x)
    {
        return x == 1 ? 1 : Mathf.Pow(2, -10 * x);
    }
    public static float EaseOutBack(float x)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(x - 1, 3) + c1 * Mathf.Pow(x - 1, 2);
    }
    public static float EaseInExpo(float x)
    {
        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    }
    public static float EaseOutQuart(float x)
    {
        return 1 - Mathf.Pow(1 - x, 4);
    }




}



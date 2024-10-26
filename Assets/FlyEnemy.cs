using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class FlyEnemy : AIBase
{
    [SerializeField, TabGroup("FLY")] float flightHeight;
    [SerializeField, TabGroup("FLY")] float distanceFromGroundVariance;
    [SerializeField, TabGroup("FLY")] float randomMoveSpeedVariance;
    [SerializeField, TabGroup("FLY")] float verticalSpeed = 0.25f;

    PlayerDamageHandler playerDamageHandler;
    float currentAttackTimer;

    protected override void Start()
    {
        base.Start();
        playerDamageHandler = FindAnyObjectByType<PlayerDamageHandler>();
        BatchSpriteLooker.AddLooker(transform);
        speed += Random.Range(-randomMoveSpeedVariance, randomMoveSpeedVariance);
      
    }
    protected override void Update()
    {
        currentAttackTimer -= Time.deltaTime;
        if (Vector3.Distance(transform.position, playerDamageHandler.transform.position) < attackRange)
        {
            if (currentAttackTimer > 0)
                return;
            AttackPlayer();
        }
        else
            FlyToPlayer();

            
    }
    float distFromGround;
    Vector3 velocity;
    void FlyToPlayer()
    {
        velocity = Vector3.zero;
        Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 25);
        distFromGround = transform.position.y - hit.point.y;
        velocity += transform.forward;

        if (distFromGround < flightHeight - distanceFromGroundVariance)
            velocity.y = verticalSpeed;
        else if (distFromGround > flightHeight + distanceFromGroundVariance)
            velocity.y = -verticalSpeed;

        transform.position += velocity * speed * Time.deltaTime;
    }
    
    void AttackPlayer()
    {
        currentAttackTimer = seccondsBetweenAttacks;
       // playerDamageHandler.Damage(damage, transform);
       DamagePlayer();
    }
}

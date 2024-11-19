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
    [SerializeField, TabGroup("FLY")] float movingUpSpeedMultiplyer = 0.5f;
    [SerializeField, TabGroup("FLY")] ParticleSystem attackParticleSystem;

    PlayerDamageHandler playerDamageHandler;
    float currentAttackTimer;
    float currentAfterAttackTimer;

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
        currentAfterAttackTimer -= Time.deltaTime;
        if (currentAfterAttackTimer > 0)
            return;
        if (Vector3.Distance(transform.position, playerDamageHandler.transform.position) < attackRange)
        {
            if (currentAttackTimer > 0)
                return;
            AttackPlayer();
        }
        else
            FlyToPlayer();


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
    float distFromGround;
    Vector3 velocity;
    float speedMultiplyer;
    void FlyToPlayer()
    {
        velocity = Vector3.zero;
        Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 25,walkableMask);
        distFromGround = transform.position.y - hit.point.y;
        velocity += transform.forward;

        if (distFromGround < flightHeight - distanceFromGroundVariance)
        {
            velocity.y = verticalSpeed;
            speedMultiplyer = movingUpSpeedMultiplyer;
        }
        else
            speedMultiplyer = 1;
          
        if (distFromGround > flightHeight + distanceFromGroundVariance)
        {
            velocity.y = -verticalSpeed;
        }
         

        transform.position += velocity * speed * speedMultiplyer * Time.deltaTime;
    }
    
    void AttackPlayer()
    {
        currentAttackTimer = seccondsBetweenAttacks;
       // playerDamageHandler.Damage(damage, transform);
        DamagePlayer();
        attackParticleSystem.Play();
        currentAfterAttackTimer = attackDelay;
    }
}

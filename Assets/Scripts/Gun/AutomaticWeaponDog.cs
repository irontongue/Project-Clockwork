using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticWeaponDog : AutomaticCompanionBase
{
    CompanionState state;
    EnemyInfo target;
    float attackTimer;
    protected override void Start()
    {
        base.Start(); 
    }

    // Update is called once per frame

    //Find a target
    //Move to the target
    //Attack the target
    //Return to the player
    //repete
    protected override void Update()
    {
        base.Update();
        spriteRenderer.transform.LookAt(playerTransform.position);
        switch (state) 
        {
            case CompanionState.FindTarget:
                target = GetFirstEnemyInfrontOfPlayer(7, 14, enemyLayerMask);
                if(target != null)
                {
                    state = CompanionState.MoveToTarget;
                    break;
                }
                MoveToPlayerFront(stats.moveSpeed, stats.followDistance);
                break;
            case CompanionState.MoveToTarget:
                if(MoveToTarget(stats.moveSpeed, stats.followDistance, target.transform.position))
                {
                    state = CompanionState.Attack;
                }
                break;
            case CompanionState.Return:
                if (MoveToPlayerFront(stats.moveSpeed, stats.followDistance))
                {
                    state = CompanionState.FindTarget;
                }
                break;
            case CompanionState.Attack:
                Attack();
                break;
        }
    }
    
    void Attack()
    {
        spriteRenderer.sprite = spriteAttack;
        if (!Timer(ref attackTimer, 4))
            return;
        spriteRenderer.sprite = spriteBase;
        if(target != null || target.enabled == false)
            target.DealDamage(stats.damage);
        state = CompanionState.Return;
    }

}

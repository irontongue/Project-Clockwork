using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticWeaponDog : AutomaticCompanionBase
{
    CompanionState state;
    EnemyInfo target;
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

        switch (state) 
        {
            case CompanionState.FindTarget:
                target = GetFirstEnemyInfrontOfPlayer(7, 14, enemyLayerMask);
                if(target != null)
                {
                    state = CompanionState.Attack;
                    break;
                }
                MoveToPlayerFront(stats.moveSpeed, stats.followDistance);
                break;
            case CompanionState.Attack:

                break;
            case CompanionState.Return:
                MoveToPlayerFront(stats.moveSpeed, stats.followDistance);

                break;
        }
    }

}

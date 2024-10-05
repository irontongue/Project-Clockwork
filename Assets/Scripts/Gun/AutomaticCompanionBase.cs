using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticCompanionBase : AutomaticWeaponBase
{
    protected enum CompanionState {FindTarget, Attack, Return}
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        playerTransform = FindAnyObjectByType<PlayerDamageHandler>().transform;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
    LayerMask enviornmentLayerMask = 1 >> 0 | 1 >> 8;
    protected void MoveToPlayerFront(float speed, float distanceFromPlayer)
    {
        RaycastHit hit;
        if(Physics.Raycast(playerTransform.position + playerTransform.forward * distanceFromPlayer, Vector3.down, out hit, enviornmentLayerMask))
        {
            transform.position = Vector3.MoveTowards(transform.position,hit.point, speed);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position + playerTransform.forward * distanceFromPlayer, speed);

        }

    }


}

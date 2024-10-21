using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AutomaticCompanionBase : AutomaticWeaponBase
{
    protected enum CompanionState {FindTarget,MoveToTarget, Attack, Return}
    [SerializeField]protected Sprite spriteBase;
    [SerializeField]protected Sprite spriteAttack;
    protected SpriteRenderer spriteRenderer;
    [SerializeField]LayerMask enviornmentLayerMask = 1 >> 0 | 1 >> 8;
    Vector3 yZeroFive = new Vector3(0,0.5f,0);
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        if(!(spriteRenderer = GetComponent<SpriteRenderer>()))
        {
            if(!(spriteRenderer = GetComponentInChildren<SpriteRenderer>()))
            {
                Debug.LogWarning(this + ": " + transform.name + ": Is missing a spriteRenderer Reference");
            }
        }
        playerTransform = FindAnyObjectByType<PlayerDamageHandler>().transform;
    }

    // Update is called once per frame

    protected bool MoveToPlayerFront(float speed, float distanceFromPlayer)
    {
        //if (Vector3.Distance(transform.position, playerTransform.position) < distanceFromPlayer)
        //{
        //    return true;
        //}
        //RaycastHit hit = RayCastDown(playerTransform.position + playerTransform.forward * distanceFromPlayer);
        //if (hit.transform != null)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position,hit.point + yZeroFive, speed);
        //    hit = RayCastDown(transform.position);
        //    if(hit.transform != null) 
        //    {
        //        transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        //    }
        //}
        //else
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, playerTransform.position + playerTransform.forward * distanceFromPlayer, speed);
        //}
        //return false;
        return MoveToTarget(speed, distanceFromPlayer, playerTransform.position + playerTransform.forward * distanceFromPlayer);
    }
    protected bool MoveToTarget(float speed, float distanceToPosition, Vector3 position)
    {
        if(Vector3.Distance(transform.position, position) < distanceToPosition) 
        {
            return true;
        }
        RaycastHit hit = RayCastDown(position);
        if(hit.transform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position + yZeroFive, hit.point, speed);
            hit = RayCastDown(transform.position);
            if (hit.transform != null)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
        }
        else
        { 
            transform.position = Vector3.MoveTowards(transform.position, position, speed);
        }
        return false;
    }
    RaycastHit RayCastDown(Vector3 position)
    {
        RaycastHit hit;
        Physics.Raycast(position, Vector3.down, out hit, enviornmentLayerMask);
        return hit;
    }



}

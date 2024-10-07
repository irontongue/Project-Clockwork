using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticCompanionBase : AutomaticWeaponBase
{
    protected enum CompanionState {FindTarget,MoveToTarget, Attack, Return}
    [SerializeField]protected Sprite spriteBase;
    [SerializeField]protected Sprite spriteAttack;
    protected SpriteRenderer spriteRenderer;
    [SerializeField]LayerMask enviornmentLayerMask = 1 >> 0 | 1 >> 8;
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
    protected override void Update()
    {
        base.Update();
    }
    protected void MoveToPlayerFront(float speed, float distanceFromPlayer)
    {
        RaycastHit hit = RayCastDown(playerTransform.position + playerTransform.forward * distanceFromPlayer);
        if (hit.transform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,hit.point, speed);
            transform.position = new Vector3(transform.position.x, RayCastDown(transform.position).point.y, transform.position.z);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position + playerTransform.forward * distanceFromPlayer, speed);
        }

    }
    protected bool MoveToTarget(float speed, float DistanceToPosition, Vector3 position)
    {
        if(Vector3.Distance(transform.position, position) < DistanceToPosition) 
        {
            return true;
        }
        RaycastHit hit = RayCastDown(position);
        if(hit.transform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, speed);
            transform.position = new Vector3(transform.position.x, RayCastDown(transform.position).point.y, transform.position.z);
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

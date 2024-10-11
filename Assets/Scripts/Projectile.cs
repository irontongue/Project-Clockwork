using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Net;

public class Projectile : PoolObject
{
    public float damage;
    public float speed;
    public GameObject origin;
    public bool active;

    private void Start()
    {
        BatchSpriteLooker.AddLooker(transform);
        BatchProjectileManager.AddProjectile(this);
    }
    public override void ReuseObject()
    {
        base.ReuseObject();
        gameObject.SetActive(true);
    }
    public override void DecomissionObject()
    {
        base.DecomissionObject();
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerDamageHandler handler = other.GetComponent<PlayerDamageHandler>();
        if (handler == null)
            return;

        handler.Damage(damage, origin.transform);
        //BatchSpriteLooker.RemoveLooker(transform);
        //BatchProjectileManager.RemoveProjectile(this);
        DecomissionObject();
        //Destroy(gameObject);

    }
}

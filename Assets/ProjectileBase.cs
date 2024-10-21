using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : PoolObject
{
    [SerializeField] WeaponType weaponType;
    [SerializeField] protected LayerMask excludePlayerLM;
    [SerializeField] protected LayerMask enemiesLM;
    [SerializeField] protected DamageType damageType;
    [SerializeField] public float projectileSpeed;
    [SerializeField] protected GameObject PFXPrefab;
    protected WeaponStats stats;
    private void Start() {
        stats = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(weaponType);
    }
    protected virtual void Move()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, projectileSpeed * Time.deltaTime, excludePlayerLM))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = transform.position + transform.forward * Time.deltaTime * projectileSpeed;
        }
    }
    
    protected void Explode(float radius, float damage, LayerMask layerMask)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, layerMask);
        if(hits.Length > 0)
        {
            foreach(Collider hit in hits)
            {
                hit.GetComponent<EnemyDamageHandler>().DealDamage(damage, damageType);
            }
        }
    }

}

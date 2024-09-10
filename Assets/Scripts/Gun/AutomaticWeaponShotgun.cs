using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class AutomaticWeaponShotgun : AutomaticWeaponBase
{
    Animator anim;
    EnemyInfo enemyInfo;
    float updateTime = 0;
    [SerializeField]ParticleSystem muzzleFlashPFX;
    [SerializeField]ParticleSystem bulletHitPFX;
    LayerMask enemyLayer = 1 << 3;
    LayerMask enemyBodyLayer = 1 << 7;
    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        if (muzzleFlashPFX == null)
        {
            Debug.LogWarning("On Shotgun, Assign MuzzleFlashPFX, trying to assign Now");
            muzzleFlashPFX = GetComponentInChildren<ParticleSystem>();
            if (muzzleFlashPFX == null) Debug.LogError("On Shotgun, There is no MuzzleFlashPFX to assign");
        }
    }
    //private void OnDrawGizmos()
    //{
    //    if(playerTransform != null)
    //    {
    //        Vector3 dir = (enemyInfo.transform.position - playerTransform.position).normalized;
    //        Gizmos.DrawRay(playerTransform.position, dir * 100);
    //    }
    //    if(hit.point != null)
    //    {
    //        Gizmos.DrawCube(hit.point, new Vector3(0.1f, 0.1f,0.1f));
    //    }
    //}
    void Update()
    {
        if (firing)
            return;
        if(onCooldown)
        {
            if(Timer(ref coolDownTime, stats.coolDown))
                onCooldown = false;
            else
                return;
        }
        if (!Timer(ref updateTime, 10))
            return;

        enemyInfo = GetFirstEnemyInfrontOfPlayer(stats.range +5, 2, enemyLayerMask);
        if(enemyInfo == null)
        {
            print("enemyInfoWasNull");
            return;
        }
        
        onCooldown = true;
        firing = true;
        anim.SetTrigger("Shoot");
        transform.LookAt(enemyInfo.transform);
    }
    public override void Shoot(int iterator = 0)
    {
        ShootAtTarget(iterator == 0);
    }
            RaycastHit hit;
    public void ShootAtTarget(bool playFX = true)
    {
        if(playFX)
            muzzleFlashPFX.Play(true);
        Vector3 dir = (enemyInfo.transform.position - playerTransform.position).normalized;
        for(int i = 0; i < stats.numberOfBullets; i++)
        {
            Quaternion randomRot = Quaternion.Euler(Random.Range(0, stats.bulletSpread), Random.Range(0, stats.bulletSpread), 0);
            Vector3 randDir = dir;//randomRot * dir;
            Physics.Raycast(playerTransform.position, randDir, out hit);
            if(hit.transform != null)
            {
                if(1 << hit.transform.gameObject.layer == enemyLayer) // 8 == 1<<3 == enemy layer // 128 == Body layer
                {
                    hit.transform.GetComponent<EnemyInfo>().damageHandler.DealDamage(stats.damage);
                }
                else if(1 << hit.transform.gameObject.layer == enemyBodyLayer)
                {
                    hit.transform.GetComponent<EnemyBody>().DealDamage(stats.damage) ;
                }
                Instantiate(bulletHitPFX, hit.point, Quaternion.identity).transform.LookAt(transform);
            }
        }
    }

    public void FinishAnimation()
    {
        firing = false;
    }

}

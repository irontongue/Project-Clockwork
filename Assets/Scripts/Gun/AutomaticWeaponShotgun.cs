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
    [SerializeField]ParticleSystem terrainHitPFX;
    [SerializeField]ParticleSystem bloodHitPFX;
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
    bool resetRotation = false;
    void Update()
    {
        if (firing)
        {
            if(resetRotation == false)resetRotation = true;
            //transform.LookAt(enemyInfo.transform); // TO DO: Need to add Min/Max rotation here
            return;
        }
        if (!UpdateCoolDown())
            return;
        if (resetRotation)
        {
            resetRotation = false;
            transform.LookAt(cam.transform.forward);
        }
        if (!Timer(ref updateTime, 10))
            return;

        enemyInfo = GetFirstEnemyInfrontOfPlayer(stats.range, stats.boxCheckWidth, enemyLayerMask);
        if(enemyInfo == null)
        {
            print("enemyInfoWasNull");
            return;
        }
        
        onCooldown = true;
        firing = true;
        anim.SetTrigger("Shoot");
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
            Vector3 randDir = randomRot * dir;
            Physics.Raycast(playerTransform.position, randDir, out hit);
            if(hit.transform != null)
            {
                EnemyDamageHandler dh;
                if(dh = hit.transform.GetComponent<EnemyDamageHandler>())
                {
                    dh.DealDamage(stats.damage);
                    Instantiate(bloodHitPFX, hit.point, Quaternion.identity).transform.LookAt(transform);

                }
                else
                {
                    Instantiate(terrainHitPFX, hit.point, Quaternion.identity).transform.LookAt(transform);
                }
            }
        }
    }

    public void FinishAnimation()
    {
        firing = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public enum ShotgunUpgrades { }

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
    List<TrailRenderer> trailRendererPool = new();
    [SerializeField]GameObject bulletTrailRendererPrefab;
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
        if(GameState.GamePaused)
            return;
        if (firing)
        {
            if(resetRotation == false)resetRotation = true;
            transform.LookAt(enemyInfo.transform); // TO DO: Need to add Min/Max rotation here
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
        if(stats.numberOfBullets > trailRendererPool.Count)
        {
            for(int i = stats.numberOfBullets - trailRendererPool.Count; i > 0; i--) 
            {
                trailRendererPool.Add(Instantiate(bulletTrailRendererPrefab).GetComponent<TrailRenderer>());
            }
        }
        for(int i = 0; i < stats.numberOfBullets; i++)
        {
            /// Random Spread Shot
            //Quaternion randomRot = Quaternion.Euler(Random.Range(0, stats.bulletSpread * 0.5f), Random.Range(0, stats.bulletSpread * 0.5f), 0);
            //Vector3 randDir = randomRot * dir;
            //Physics.Raycast(playerTransform.position, randDir, out hit);
            ///Horizontal Spread Shot
            Quaternion rot = Quaternion.Euler(0,-(stats.bulletSpread * stats.numberOfBullets * 0.5f) + stats.bulletSpread * i, 0);
            Vector3 direction = rot * dir;
            Physics.Raycast(playerTransform.position, direction, out hit, excludePlayerLayerMask);

            if (hit.transform != null)
            {
                StartCoroutine(MoveTrail(trailRendererPool[i], hit.point));
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
            else
            {
                StartCoroutine(MoveTrail(trailRendererPool[i], transform.position + 100 * direction));
            }

        }

    }

    public void FinishAnimation()
    {
        firing = false;
    }
    IEnumerator MoveTrail(TrailRenderer trailRenderer, Vector3 destination)
    {
        trailRenderer.transform.position = transform.position;
        trailRenderer.gameObject.SetActive(true);
        yield return null;
        trailRenderer.transform.position = destination;
        yield return new WaitForSeconds(trailRenderer.time);
        trailRenderer.gameObject.SetActive(false);

    }

}

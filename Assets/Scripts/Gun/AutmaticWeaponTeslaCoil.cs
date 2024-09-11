using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutmaticWeaponTeslaCoil : AutomaticWeaponBase
{
    float updateTime = 0;
    [SerializeField]LineRenderer lineRen;
    [SerializeField] Transform lightningMuzzlePosition;
    [SerializeField] float lightningActiveTime = 0.5f;
    EnemyInfo enemyInfo;
    protected override void Start()
    {
        base.Start();
        if(lineRen = null) 
        {
            Debug.LogWarning("On Tesla Coil: Line Renderer needs to be assigned");
        }
        if(lightningMuzzlePosition = null)
        {
            Debug.LogWarning("On Tesla Coil: Transform - Lightning Muzzle Position has not been assigned, using this transform instead");
            lightningMuzzlePosition = transform;
        }
    }

    // Update is called once per frame
    float lightningTime = 0;
    void Update()
    {
        if(firing)
        {
            if(!Timer(ref lightningTime, lightningActiveTime))
                return;
            lineRen.gameObject.SetActive(false);
            firing = false;
        }
        if (!UpdateCoolDown()) // wait for weapon cool down
            return;
        if(!Timer(ref updateTime, 10f)) // Stop physics checks from going every frame when missing
            return;
        
        if(!(enemyInfo = GetFirstEnemyInfrontOfPlayer(stats.range, stats.range, enemyLayerMask))) // Get Enemy inside a square infron to the player
            return;

        onCooldown = true;
        firing = true;
        ZapEnemy();

    }

    void ZapEnemy()
    {
        int remainingBounces = stats.bounces;
        EnemyInfo info = enemyInfo;
        lineRen.positionCount = remainingBounces + 2; // 2 for the inital line between the tesla coil and enemy
        enemyInfo.DealDamage(stats.damage);

        lineRen.SetPosition(0, lightningMuzzlePosition.position);
        lineRen.SetPosition(1, enemyInfo.transform.position);

        for (int i = 2; i < remainingBounces + 2; i++) 
        {
            info = GetAdjacentEnemyInCircle(3, info.transform.position);
            if (info == null)
                break;
            info.DealDamage(stats.damage);
            lineRen.SetPosition(i, info.transform.position);
        }
        lineRen.gameObject.SetActive(true);
    }
    /// <summary>
    /// Loops i times to try and get the pseudo nearest enemy
    /// </summary>
    /// <param name="iterations"></param>
    /// <param name="center"></param>
    /// <returns></returns>
    EnemyInfo GetAdjacentEnemyInCircle(int iterations, Vector3 center)
    {
        EnemyInfo enemyInfo;
        for(int i = 0; i < iterations; i++) 
        {
            float distance = stats.bounceRange / iterations;
            enemyInfo = GetEnemyInSphere(distance, center, enemyLayerMask);
            if(enemyInfo)
                return enemyInfo;
        }
        return null;
    }
}

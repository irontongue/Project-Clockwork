using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeslaCoilUpgrades { }
public class AutmaticWeaponTeslaCoil : AutomaticWeaponBase
{
    float updateTime = 0;
    [SerializeField]LineRenderer lineRen;
    [SerializeField] Transform lightningMuzzlePosition;
    [SerializeField] float lightningActiveTime = 5f;
    [SerializeField] ParticleSystem smokePFX;
    EnemyInfo enemyInfo;
    protected override void Start()
    {
        base.Start();
        if(lineRen == null) 
        {
            Debug.LogWarning("On Tesla Coil: Line Renderer needs to be assigned");
        }
        if(lightningMuzzlePosition == null)
        {
            Debug.LogWarning("On Tesla Coil: Transform - Lightning Muzzle Position has not been assigned, using this transform instead");
            lightningMuzzlePosition = transform;
        }
    }

    // Update is called once per frame
    float lightningTime = 0;
    protected override void Update()
    {
        base.Update();
        if(firing)
        {
            UpdateLineRenPositons();
            if(!Timer(ref lightningTime, lightningActiveTime))
                return;
            lineRen.gameObject.SetActive(false);
            firing = false;
        }
        if (!UpdateCoolDown()) // wait for weapon cool down
            return;
        if(!Timer(ref updateTime, 0.1f)) // Stop physics checks from going every frame when missing
            return;
        
        if(!(enemyInfo = GetFirstEnemyInfrontOfPlayer(stats.range, stats.boxCheckWidth, enemyLayerMask))) // Get Enemy inside a square infron to the player
            return;

        onCooldown = true;
        firing = true;
        ZapEnemy();

    }
    //TO DO:
    //ON hit Keep track of enemes previously hit
    //Line ren positions need to be set every frame
    //line ren positions are for some reason
    List<Transform> enemyTransforms = new List<Transform>();
    void ZapEnemy()
    {
        print("Zap Enemy");
        enemyTransforms.Clear(); // Reset Enemy List

        int remainingBounces = stats.bounces;
        EnemyInfo info = enemyInfo;
        lineRen.positionCount = remainingBounces + 1; // 2 for the inital line between the tesla coil and enemy

        lineRen.SetPosition(0, lightningMuzzlePosition.position);
        lineRen.SetPosition(1, enemyInfo.transform.position);

        enemyInfo.DealDamage(stats.damage);
        enemyTransforms.Add(info.transform);

        //lineRen.SetPosition(0, lightningMuzzlePosition.position);
        //lineRen.SetPosition(1, enemyInfo.transform.position);

        for (int i = 2; i < remainingBounces + 2; i++) 
        {
            info = GetAdjacentEnemyInCircle(3, info.transform.position);
            if (info == null)
                break;
            enemyTransforms.Add(info.transform);
            info.DealDamage(stats.damage);
            Instantiate(smokePFX, info.transform.position, Quaternion.identity);
            //lineRen.SetPosition(i, info.transform.position);
        }
        if(enemyTransforms.Count < lineRen.positionCount - 1) 
        {
            lineRen.positionCount = enemyTransforms.Count + 1;
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
        for(int i = iterations; i > 0; i--) 
        {
            float distance = stats.bounceRange / i;
            enemyInfo = GetEnemyInSphere(distance, center, enemyLayerMask, enemyTransforms);
            if(enemyInfo)
                return enemyInfo;
        }
        return null;
    }
    void UpdateLineRenPositons()
    {
        lineRen.SetPosition(0, lightningMuzzlePosition.position);
        for(int i = 1; i < enemyTransforms.Count; i++)
        {
            print("i: " + i);
            lineRen.SetPosition(i, enemyTransforms[i].position);
        }
    }
}

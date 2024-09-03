using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class AutomaticWeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;
    protected WeaponStats stats;
    protected Camera cam;
    Transform playerTransform;
    
    
    virtual protected void Start()
    {
        playerTransform = transform.root;
        cam = Camera.main;
        stats = FindAnyObjectByType<AllWeaponStats>().GetWeaponStat(weaponType);
    }
    /// <summary>
    /// returns the first GameObject infront of the crosshair
    /// returns null if no objects present
    /// takes distance in units otherwise infinite distance
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    protected GameObject RayCastForwardGameObject(LayerMask layerMask,float distance = float.MaxValue)
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance,layerMask))
        {
            return hit.transform.gameObject;
        }
        return null;
    }

    /// <summary>
    /// Returns all enemies  within a radius
    /// Useful for explosions
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="position"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    protected EnemyInfo[] GetAllEnemiesInSphere(float radius,Vector3 position, LayerMask layerMask)
    {
        Collider[]hits = Physics.OverlapSphere(position, radius, layerMask);
        EnemyInfo[] info = new EnemyInfo[hits.Length];
        if(hits.Length == 0)
            return null;
        for (int i = 0; i < hits.Length; i++)
        {
            info[i] = hits[i].GetComponent<EnemyInfo>(); 
        }
        return info;
    }
    /// <summary>
    /// Returns GetFirstEnemyInDirection in the forward vector
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    protected EnemyInfo GetFirstEnemyInfrontOfPlayer(float distance, float size, LayerMask layerMask)
    {
        return GetFirstEnemyInDirection(1, distance, size, layerMask);
    }
    /// <summary>
    /// Returns GetFirstEnemyInDirection in the backward vector
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    protected EnemyInfo GetFirstEnemyBehindPlayer(float distance, float size, LayerMask layerMask)
    {
        return GetFirstEnemyInDirection(-1, distance, size, layerMask);

    }
    /// <summary>
    /// Gets the first enemy in the forward or backward vector (direction = 1 for forward) (direction = -1 for backward)
    /// uses sphereCast
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="size"></param>
    /// <returns>First enemy in the forward or backward vector</returns>
    protected EnemyInfo GetFirstEnemyInDirection(int direction, float distance, float size, LayerMask layerMask)
    {
        if (direction != 1 && direction != -1)
        {
            Debug.LogWarning($"GetFirstEnemyInDirection: direction = {direction}, setting to forward (1)");
            direction = 1;
        }

        Collider hit = Physics.OverlapBox(playerTransform.position + direction * size * transform.forward, new Vector3(size, size, size), playerTransform.rotation, layerMask)[0];

        if(hit)
            return hit.GetComponent<EnemyInfo>();
        else
            return null;
    }
    /// <summary>
    /// Deal Damage to a single Enemy
    /// </summary>
    /// <param name="enemyInfo"></param>
    /// <param name="amount"></param>
    protected void DamageEnemy(EnemyInfo enemyInfo, float amount)
    {
        enemyInfo.damageHandler.DealDamage(amount);
    }
    /// <summary>
    /// Deal damage to multple enemies
    /// loops through a given array
    /// </summary>
    /// <param name="enemyInfos"></param>
    /// <param name="amount"></param>
    protected void DamageMultipleEnemies(EnemyInfo[] enemyInfos, float amount)
    {
        foreach(EnemyInfo info in enemyInfos)
        {
            info.damageHandler.DealDamage(amount);
        }
    }
    /// <summary>
    /// Helper function
    /// ticks _time up by deltaTime and speed.
    /// </summary>
    /// <param name="time"> Reference to own timer float </param>
    /// <param name="speed">1 = 1 second, 2 = 0.5f second</param>
    /// <returns>true if time >= 1 </returns>
    protected bool Timer(ref float _time, float speed)
    {
        _time += Time.deltaTime * speed;
        if(_time >= 1)
        {
            _time = 0;
            return true;
        }
        return false;
    }

}

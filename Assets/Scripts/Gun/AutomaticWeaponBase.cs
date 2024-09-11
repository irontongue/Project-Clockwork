using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class AutomaticWeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponType weaponType;
    protected WeaponStats stats;
    protected Camera cam;
    protected Transform playerTransform;
    [SerializeField]protected LayerMask enemyLayerMask = 8;
    [SerializeField] protected LayerMask excludePlayerLayerMask = ~(1<<6);
    protected LayerMask everythingEnemy = 1 << 3 | 1 << 7;

    protected bool onCooldown = false;
    protected float coolDownTime = 0;

    protected bool firing = false;
    protected struct EnemyHitPacket
    {
        public EnemyInfo enemyInfo;
        public Vector3 hitPosition;
    }
    protected struct EnemiesHitPacket
    {
        public EnemyInfo[] enemyInfos;
        public Vector3[] hitPositions;
    }
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
    protected RaycastHit RayCastForwardRayHit(LayerMask layerMask, float distance = float.MaxValue)
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, distance, layerMask);
        return hit;
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
    /// Returns a single enemy within a radius
    /// </summary>
    /// <param name="radius"></param>
    /// <param name="position"></param>
    /// <param name="layerMask"></param>
    /// <returns></returns>
    protected EnemyInfo GetEnemyInSphere(float radius, Vector3 position, LayerMask layerMask)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius, layerMask);
        if (hits.Length == 0)
            return null;
        return hits[0].GetComponent<EnemyInfo>();
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
    float s = 0;
    float d = 0;
    private void OnDrawGizmos()
    {
        //if(playerTransform)
        //Gizmos.DrawCube(playerTransform.position + 1 * d * playerTransform.forward, new Vector3(s, s, d));
    }
    /// <summary>
    /// Gets the first enemy in the forward or backward vector (direction = 1 for forward) (direction = -1 for backward)
    /// uses sphereCast
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <param name="size">Horizontal size</param>
    /// <returns>First enemy in the forward or backward vector</returns>
    protected EnemyInfo GetFirstEnemyInDirection(int direction, float distance, float size, LayerMask layerMask)
    {
        if (direction != 1 && direction != -1)
        {
            Debug.LogWarning($"GetFirstEnemyInDirection: direction = {direction}, setting to forward (1)");
            direction = 1;
        }
        size = size * 0.5f;
        distance = distance * 0.5f;
        s = size;
        d = distance;
        Collider[] hit = Physics.OverlapBox(playerTransform.position + (direction * distance * playerTransform.forward), new Vector3(size, size, distance), Quaternion.identity, layerMask); // WRONG WRONG WRONG FIX FIX FIX
        
        if (hit.Length == 0)
        {
            return null;
        }
        
        RaycastHit rayHit = new RaycastHit();
        foreach(Collider col in hit)
        {
            if (Physics.Raycast(playerTransform.position, (col.transform.position - playerTransform.position).normalized, out rayHit, float.MaxValue, excludePlayerLayerMask))
            { break; }
        }
        if (!rayHit.transform)
            return null;
        if (1 << rayHit.transform.gameObject.layer == (int)enemyLayerMask)
        {
            return rayHit.transform.GetComponent<EnemyInfo>();
        }
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
        enemyInfo.DealDamage(amount);
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
            info.DealDamage(amount);
        }
    }
    /// <summary>
    /// base to fire any weapon and should be overwritten.
    /// call base.Shoot() at the end of the overided statement to loop any weapon to shoot multiple bullets.
    /// </summary>
    /// <param name="iterator"></param>
    public virtual void Shoot(int iterator = 1)
    {
        if(iterator == stats.numberOfBullets)
        {
            return;
        }

        iterator++;
        Shoot(iterator);
    }
    /// <summary>
    /// simple Cooldown function to be put at the start of the weapon Update Loop
    /// set onCooldown to true after firing the weapon
    /// e.g.
    /// if(UpdateCoolDown())
    ///     return;
    /// </summary>
    /// <returns>true when onCooldown == false</returns>
    protected bool UpdateCoolDown()
    {
        if (onCooldown)
        {
            if (Timer(ref coolDownTime, stats.coolDown))
                onCooldown = false;
        }
        return !onCooldown;
    }
    /// <summary>
    /// Helper function
    /// ticks _time up by deltaTime and speed.
    /// </summary>
    /// <param name="_time"> Reference to own timer float </param>
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

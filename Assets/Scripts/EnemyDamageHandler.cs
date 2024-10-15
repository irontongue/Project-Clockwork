using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// Depricated
    /// </summary>
public class EnemyDamageHandler : PoolObject
{

    [HideInInspector] public EnemyInfo mainBodyInfo;

    /// <summary>
    /// Damages the enemy
    /// </summary>
    /// <param name="damage"></param>
    public virtual bool DealDamage(float amount, BodyPart bodyPart = BodyPart.Body, DamageType damageType = DamageType.None)
    {
        return false;
    }
    public virtual bool DealDamage(float amount, DamageType damageType = DamageType.None)
    {
        return false;
    }
    public virtual bool DealDamage(float amount)
    {
        return false;
    }
}

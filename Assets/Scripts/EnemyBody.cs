using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart {Head, Body}
public class EnemyBody : EnemyDamageHandler
{
    EnemyInfo info;
    [SerializeField] BodyPart bodyPart;

    void Start()
    {
        info = GetComponentInParent<EnemyInfo>();    
    }
    public override void DealDamage(float damage, BodyPart bodyPart = BodyPart.Body, DamageType damageType = DamageType.None)
    {
        info.DealDamage(damage, this.bodyPart, damageType);
    }

}

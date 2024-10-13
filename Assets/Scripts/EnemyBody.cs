using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart {Head, Body}
public class EnemyBody : EnemyDamageHandler
{
    [SerializeField] BodyPart bodyPart;

    void Start()
    {
        mainBodyInfo = GetComponentInParent<EnemyInfo>();    
    }
    public override bool DealDamage(float damage, BodyPart bodyPart = BodyPart.Body, DamageType damageType = DamageType.None)
    {
        mainBodyInfo.DealDamage(damage, this.bodyPart, damageType);
        return true;
    }

}

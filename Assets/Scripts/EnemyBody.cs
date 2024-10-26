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
        print("washit");
        mainBodyInfo.DealDamage(damage, this.bodyPart, damageType);
        return true;
    }
    public override bool DealDamage(float damage, DamageType damageType = DamageType.None)
    {
        print("?");
        mainBodyInfo.DealDamage(damage, bodyPart, damageType);
        return true;
    }
    public override bool DealDamage(float damage)
    {
        print("?");
        mainBodyInfo.DealDamage(damage);
        return true;
    }

}

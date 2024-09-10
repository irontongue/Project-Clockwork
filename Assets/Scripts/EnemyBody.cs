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
    public void DealDamage(float damage)
    {
        info.DealDamage(damage, bodyPart);
    }

}

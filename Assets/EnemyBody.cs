using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart {Head, Body}
public class EnemyBody : MonoBehaviour
{
    EnemyInfo info;
    [SerializeField] BodyPart bodyPart;

    void Start()
    {
        info = GetComponentInParent<EnemyInfo>();    
    }
    public void DealDamage(float damage)
    {
        info.damageHandler.DealDamage(damage, bodyPart);
    }

}

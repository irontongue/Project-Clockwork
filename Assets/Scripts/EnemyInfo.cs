using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// This script is used as an access point for all scripts enemy related
/// as well as the enemy stats
/// Damage handling, Agent, AI Behaviour
/// </summary>
public enum EnemyType { a, b, c }

public class EnemyInfo : EnemyDamageHandler
{
    #region Stats
    [Header("Stats")]
    [TabGroup("Base AI")] public EnemyType enemyType;
    [TabGroup("Base AI")] public float maxHealth = 10f;
    [TabGroup("Base AI")] public float health = 10f;
    [TabGroup("Base AI")] public float speed = 5f;
    [TabGroup("Base AI")] public float EXP = 1f;
    #endregion

    #region Damage Handling
    /// <summary>
    /// Damages the enemy
    /// </summary>
    /// <param name="damage"></param>
    public override void DealDamage(float amount, BodyPart bodyPart = BodyPart.Body)
    {
        if (bodyPart == BodyPart.Head)
            amount *= 2;
        ChangeHealth(-amount);
    }
    /// <summary>
    /// Heals the enemy
    /// </summary>
    /// <param name="amount"></param>
    public void HealHealth(float amount)
    {
        ChangeHealth(amount);
    }
    /// <summary>
    /// Directly Changes the health on enemy info
    /// </summary>
    /// <param name="amount"></param>
    void ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        if (health == 0)
        {
            DeathEvent();
        }
    }
    /// <summary>
    /// If enemy health hits 0 this is called
    /// What happens when the enemy dies
    /// </summary>
    virtual protected void DeathEvent()
    {
        gameObject.SetActive(false);
    }
    #endregion
}

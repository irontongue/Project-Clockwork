using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageHandler : MonoBehaviour
{
    EnemyInfo enemyInfo;
    void Start()
    {
        enemyInfo = GetComponent<EnemyInfo>();
        if (!enemyInfo)
            Debug.LogWarning("EnemyInfo Is Missing on this Enemy");
    }
    /// <summary>
    /// Damages the enemy
    /// </summary>
    /// <param name="damage"></param>
    public void DealDamage(float amount)
    {
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
        enemyInfo.health = Mathf.Clamp(enemyInfo.health + amount, 0, enemyInfo.maxHealth);
        if (enemyInfo.health == 0)
        {
            DeathEvent();
        }
    }
    /// <summary>
    /// If enemy health hits 0 this is called
    /// What happens when the enemy dies
    /// </summary>
    void DeathEvent()
    {
        print("DeathEvent");
        gameObject.SetActive(false);
    }
    
}

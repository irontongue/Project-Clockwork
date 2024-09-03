using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used as an access point for all scripts enemy related
/// as well as the enemy stats
/// Damage handling, Agent, AI Behaviour
/// </summary>
public enum EnemyType { a, b, c }

public class EnemyInfo : MonoBehaviour
{
    [Header("Stats")]
    public EnemyType enemyType;
    public float maxHealth = 10f;
    public float health = 10f;
    public float speed = 5f;

    [Header("Scripts")]
    public EnemyDamageHandler damageHandler;
}

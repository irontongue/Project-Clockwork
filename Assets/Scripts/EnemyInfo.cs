using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

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
    [TabGroup("Base AI")] public Image healthBarImage;
    [TabGroup("Base AI")] public SpriteRenderer spriteRenderer;
    [TabGroup("Base AI")] public float damageFlashTime = 1;
    float flashTimer = 1;
    [HideInInspector] public int fireStacks = 0;
    Color grey = new Color(0.75f, 0.75f, 0.75f);

    protected virtual void Start()
    {
        if(!(spriteRenderer = GetComponent<SpriteRenderer>()))
        {
            Debug.LogWarning(this + " Sprite Renderer is not assigned");
        }
        
    }

    #endregion

    #region Damage Handling
    /// <summary>
    /// Damages the enemy
    /// </summary>
    /// <param name="damage"></param>
    public override void DealDamage(float amount, BodyPart bodyPart = BodyPart.Body, DamageType damage = DamageType.None)
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
        if(healthBarImage != null)
        {
            healthBarImage.fillAmount = health / maxHealth;
        }
        if (health == 0)
        {
            DeathEvent();

            spriteRenderer.color = Color.white;
            StopAllCoroutines();
            return;
        }
        if (flashTimer < 1)
        {
            flashTimer = 0;
        }
        else
        {
            StartCoroutine(FlashSprite());
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

    IEnumerator FlashSprite()
    {
        flashTimer = 0;

        while (flashTimer < 1)
        {
            spriteRenderer.color = Color.Lerp(Color.white, grey, Mathf.Sin(flashTimer * 24));
            flashTimer += Time.deltaTime;
            yield return null;
        }
    }
}

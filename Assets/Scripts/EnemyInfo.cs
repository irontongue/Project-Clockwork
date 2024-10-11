using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEditor;

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
    [TabGroup("Base AI")] public float damageFlashTime = 1;
    [TabGroup("Base AI")] public float healthPotionDropChance = 0.1f;
     #endregion

    [Header("Prefabs")]
    [TabGroup("Base AI")] public GameObject deathPFX;
    [TabGroup("Base AI")] public GameObject healthPotionPrefab;
    [Header("References")]
    [TabGroup("Base AI")] public Image healthBarImage;
    [TabGroup("Base AI")] public SpriteRenderer spriteRenderer;
    [TabGroup("Base AI"), SerializeField] Vector3 floatingTextSpawnOffset;
    SpriteMaskUpdate spriteMaskUpdate;
    //Logic
    float flashTimer = 1;
    Color grey = new Color(0.75f, 0.75f, 0.75f);
    [HideInInspector] public int fireStacks = 0;



    protected virtual void Start()
    {
        if(!(spriteRenderer = GetComponent<SpriteRenderer>()))
        {
            Debug.LogWarning(this + " Sprite Renderer is not assigned");
        }
        if(!(spriteMaskUpdate = GetComponent<SpriteMaskUpdate>()))
        {
            Debug.LogWarning(this + " SpriteMaskUpdate is not Present on: " + gameObject.name);
        }

    }
    protected virtual void Update()
    {
        FlashSprite(); // Set FlashTimer to be 0 to restart;
    }
    /// <summary>
    /// Lerps the sprite renderer colour based on flashTimer
    /// </summary>
    void FlashSprite()
    {
        if(flashTimer < 1)
        {
            spriteRenderer.color = Color.Lerp(Color.white, grey, Mathf.Sin(flashTimer * 24));
            flashTimer += Time.deltaTime;
        }
    }
   

    #region Damage Handling
    /// <summary>
    /// Damages the enemy
    /// </summary>
    /// <param name="damage"></param>
    public override void DealDamage(float amount, BodyPart bodyPart = BodyPart.Body, DamageType damage = DamageType.None)
    {
        Color color = Color.white;
        if (bodyPart == BodyPart.Head)
        {
            amount *= 2;
            color = GlobalStaticReference.critColour;
        }
        FloatingTextManager.SpawnFloatingText(transform.position + floatingTextSpawnOffset, amount, color);
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
        float percent = health / maxHealth;
        if(healthBarImage != null)
        {
            healthBarImage.fillAmount = percent;
        }
        if (health == 0)
        {
            DeathEvent();
            if (deathPFX != null)
                Instantiate(deathPFX, transform.position,deathPFX.transform.rotation);
            if(healthPotionPrefab != null)
            {
                float r = Random.Range(0f, 1f);
                if (r < healthPotionDropChance)
                    Instantiate(healthPotionPrefab, transform.position, Quaternion.identity);
            }
            spriteRenderer.color = Color.white;
            StopAllCoroutines();
            return;
        }
        if(spriteMaskUpdate  != null) 
        {
            spriteMaskUpdate.SetMaskPercentage(percent);
        }
        flashTimer = 0;

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
    private void OnDrawGizmosSelected()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white; // Set the text color
        style.alignment = TextAnchor.MiddleCenter; // Center the text
        style.fontSize = 12;
        Handles.Label(transform.position + floatingTextSpawnOffset, "F_TXT", style);
    }

}

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
   // [Header("Stats")]
    //[TabGroup("Base AI")] public EnemyType enemyType;
    [TabGroup("DamageHandler")] public float maxHealth = 10f;
    [TabGroup("DamageHandler")] public float health = 10f;
    [TabGroup("Movement")] public float speed = 5f;
    [HideInInspector] public float EXP = 1f;
    [TabGroup("DamageHandler")] public float damageFlashTime = 1;
    [TabGroup("Loot")] public float healthPotionDropChance = 0.1f;
     #endregion

   // [Header("Prefabs")]
    [TabGroup("Effects")] public GameObject deathPFX;
    [TabGroup("Loot")] public GameObject healthPotionPrefab;
   // [Header("References")]
    [TabGroup("DamageHandler")] public GameObject healthBarPivot;
    [TabGroup("DamageHandler")] public SpriteRenderer spriteRenderer;
    [TabGroup("DamageHandler"), SerializeField] Vector3 floatingTextSpawnOffset;
    SpriteMaskUpdate spriteMaskUpdate;
    //Logic
    protected float flashTimer = 1;
    Color grey = new Color(0.75f, 0.75f, 0.75f);
    [HideInInspector] public int fireStacks = 0;
    bool zapped = false;
    Material material;



    protected virtual void Start()
    {
        mainBodyInfo = this; //This is to give the weapons the correct reference (to check things like onFire etc), for when they headshot the AI. 
        if(!(spriteRenderer = GetComponent<SpriteRenderer>()))
        {
            Debug.LogWarning(this + " Sprite Renderer is not assigned");
        }
        if(!(spriteMaskUpdate = GetComponent<SpriteMaskUpdate>()))
        {
            Debug.LogWarning(this + " SpriteMaskUpdate is not Present on: " + gameObject.name);
        }
        material = spriteRenderer.material;

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
            if(zapped && flashTimer < 0.25f)
            {
                material.SetInt("_Zapped", Mathf.Abs(Mathf.RoundToInt(Mathf.Sin(flashTimer * 24))));
            }
            else if(zapped)
            {
                zapped = false;
                material.SetInt("_Zapped", 0);
            }
        }
    }
   

    #region Damage Handling
    /// <summary>
    /// Damages the enemy
    /// returns true if is Headshot
    /// </summary>
    /// <param name="damage"></param>
    public override bool DealDamage(float amount, DamageType damageType = DamageType.None)
    {
        DealDamage(amount, BodyPart.Body, damageType);
        print("a");
        return false;
    }
    public override bool DealDamage(float amount)
    {
        DealDamage(amount, BodyPart.Body, DamageType.None);
        print("b");
        return false;
    }
    public override bool DealDamage(float amount, BodyPart bodyPart = BodyPart.Body, DamageType damageType = DamageType.None)
    {
        print("c");
        Color color = Color.white;
        if (bodyPart == BodyPart.Head)
        {
            amount *= 2;
            color = GlobalStaticReference.critColour;
        }
        FloatingTextManager.SpawnFloatingText(transform.position + floatingTextSpawnOffset, amount, color);
        ChangeHealth(-amount);
        if(damageType == DamageType.Lightning)
        {
            zapped = true;
        }
        return false;
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
    Vector3 scale = new(1, 1, 1);
    void ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        float percent = health / maxHealth;
        if(healthBarPivot != null)
        {
            scale.x = percent;
            healthBarPivot.transform.localScale = scale;
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
            //StopAllCoroutines();
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

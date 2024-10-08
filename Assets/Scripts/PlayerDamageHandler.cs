using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerDamageHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject deathUI;
    [SerializeField] float healthRegenSpeed = 0.5f;
    [SerializeField] float seccondsSinceLastHitToHeal = 2f;

    float timeSinceLastHit;

    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        
    }
    public void Damage(float damage, Transform damagerTransform)
    {
        currentHealth -= damage;
        UpdateHealthUI();
        DamageFlash(damagerTransform);
        if (currentHealth <= 0)
            DeathEvent();

        timeSinceLastHit = 0;

        

        
    }
    private void Update()
    {
        if (GameState.GamePaused)
            return;
        UIDamageFade();
        timeSinceLastHit += Time.deltaTime;

        if (timeSinceLastHit < seccondsSinceLastHitToHeal)
            return;

        currentHealth += healthRegenSpeed * Time.deltaTime;

        if (currentHealth >= maxHealth)
            currentHealth = maxHealth;

        UpdateHealthUI();
    }

    void DamageFlash(Transform damageLocation)
    {
        float attackAngle = (Vector3.Angle(damageLocation.forward, transform.forward) * 2);
    
        if (attackAngle < 110)
        {
            back = true;
            backImage.color = new(backImage.color.r, backImage.color.g, backImage.color.b, 1);
        }
          
        else if (attackAngle < 235)
        {
            if (transform.forward.x < damageLocation.transform.forward.x)
            {
                left = true;
                leftImage.color = new(leftImage.color.r, leftImage.color.g, leftImage.color.b, 1);
            }
          
            else
            {
                right = true;
                rightImage.color = new(rightImage.color.r, rightImage.color.g, rightImage.color.b, 1);
            }
         
        }
            
        else
        {
            front = true;
            frontImage.color = new(frontImage.color.r, frontImage.color.g, frontImage.color.b, 1);
        }
    }
    bool front, left, right, back;
    [SerializeField]  Image frontImage, leftImage, rightImage, backImage;
    [SerializeField] float fadeOutSpeed;
    Color color;
    void UIDamageFade()
    {
        
        if(front)
        {
            color = frontImage.color;
            color.a -= fadeOutSpeed  * Time.deltaTime; 
            frontImage.color = color;
      
        }
        if (left)
        {
            color = leftImage.color;
            color.a -= fadeOutSpeed  * Time.deltaTime;
            leftImage.color = color;
        }
        if (right)
        {
            color = rightImage.color;
            color.a -= fadeOutSpeed  * Time.deltaTime;
            rightImage.color = color;
        }
        if (back)
        {
            color = backImage.color;
            color.a -= fadeOutSpeed  * Time.deltaTime;
            backImage.color = color;
        }
    }

    void DeathEvent()
    {
        
        try
        {
            deathUI.SetActive(true);
            GamePauser.instance.PauseGame(true, gameObject);
        }
        catch 
        {
            print("DEATH UI NOT SET!");
        }
     

      //  Invoke("RestartLevel", 2);
    }
    [Header("HealthUI")]
    [SerializeField] Image healthBar;
    void UpdateHealthUI()
    {
        if (healthBar == null)
            return;
        // healthText.text = "HP: " + currentHealth + " / " + maxHealth;
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    void RestartLevel()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}

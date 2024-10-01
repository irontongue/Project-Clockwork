using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDamageHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHealth = 100;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] GameObject deathUI;

    float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        
    }
    public void Damage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthUI();
 
        if (currentHealth <= 0)
            DeathEvent();
    }

    void DeathEvent()
    {
        Time.timeScale = 0;
        try
        {
            deathUI.SetActive(true);
        }
        catch { }
     

        Invoke("RestartLevel", 2);
    }

    void UpdateHealthUI()
    {
        if (healthText == null)
            return;
        healthText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    void RestartLevel()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}

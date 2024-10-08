using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerLevelUpManager : MonoBehaviour
{
    [SerializeField] int[] expToLvlup;
    int maxLevel;
    float currentEXP;
    int currentLevel;
    public bool readyToLVLup;
    UpgradeManager upgradeManager;

    [SerializeField] TextMeshProUGUI levelUpReadyText;
    [SerializeField] Image expBar;
    [SerializeField] string levelUpMessage;
    [SerializeField] float textFlashSpeed;
    [SerializeField] float minTextOppacity;
    Color textColor;

    static public PlayerLevelUpManager instance;

    private void Start()
    {
        maxLevel = expToLvlup.Length;
        upgradeManager = FindAnyObjectByType<UpgradeManager>();
        textColor = levelUpReadyText.color;
        newTextColor = textColor;
        levelUpReadyText.text = "";

        expBar.fillAmount = 0;

        instance = this;
    }

    public void ReciveEXP(float amount)
    {
      
        currentEXP += amount;

        if (currentEXP >= expToLvlup[currentLevel])
        {
            readyToLVLup = true;
            levelUpReadyText.text = levelUpMessage;
        }
    
        expBar.fillAmount = (float)currentEXP / (float)expToLvlup[currentLevel];

    }
    Color newTextColor;
    private void Update()
    {

        if (!readyToLVLup || GameState.GamePaused)
            return;

        if (Input.GetKeyDown(KeyCode.X))
            LevelUp();
        //ping pong goes between 0, and x. i dont want the text to go completly away, so i decrease the minimum by y, and add it on to the result. so the new min is y.
       newTextColor.a =  Mathf.PingPong(Time.time * textFlashSpeed, 1 - minTextOppacity) + minTextOppacity;
       levelUpReadyText.color = newTextColor;
    }
    public void LevelUp()
    {
        GamePauser.instance.PauseGame(true,false,gameObject);
        readyToLVLup = false;
        currentEXP -= expToLvlup[currentLevel];
        currentLevel++;
        upgradeManager.StartUpgrade();
        levelUpReadyText.color = textColor;
    }

    public void LevelUpFinished()
    {
        levelUpReadyText.text = "";
    }
}

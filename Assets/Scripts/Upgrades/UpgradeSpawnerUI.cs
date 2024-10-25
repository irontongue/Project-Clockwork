using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class UpgradeSpawnerUI : MonoBehaviour
{

    [SerializeField] GameObject upgradePopupGameobject;
    [SerializeField] float spacingBetweenRowUpgrades;
    [SerializeField] float spacingBetweenCollumUpgrades;
    [SerializeField] Vector2 firstUpgradePos;
    [SerializeField] float upgradesPerRow;
    [SerializeField] GameObject canvas;

    [SerializeField] float waitTimeUntillCardsInteractable = 0.25f;
    UpgradeManager upgradeManager;

    

    private void Start()
    {
        upgradeManager = FindAnyObjectByType<UpgradeManager>();
        
    }

    bool upgradeIsWeapon;
    List<UpgradeButton> buttons = new();
    float timeSinceUpgradeStarted;
    int countStatsToDisplay;
    public void DisplayPopups(List<UpgradeLine> upgradesToDisplay)
    {
        timeSinceUpgradeStarted = Time.realtimeSinceStartup;
        GamePauser.instance.PauseGame(true,false, gameObject);
        buttons.Clear();
        UpgradeButton.ButtonDelegate upgradeDelegate = GainUpgrade; // give this to the button, so it can call back
    
        int rows = 0;
        int collums = 0;
      //  upgradeIsWeapon = upgradesToDisplay[0].isWeapon;
        foreach (UpgradeLine upgradeInfo in upgradesToDisplay)
        {
            UpgradeButton upgradeButton = Instantiate(upgradePopupGameobject).GetComponent<UpgradeButton>();

            upgradeButton.titleText.text = upgradeInfo.packet[upgradeInfo.levels].upgradeTitle;
            upgradeButton.bodyText.text = upgradeInfo.packet[upgradeInfo.levels].upgradeBody;
            
            upgradeButton.buttonDelegate = upgradeDelegate;
            countStatsToDisplay = upgradeInfo.statsToDisplay.Length;
            for (int i = 0; i < upgradeButton.statTexts.Length; i++)
            {
                if (i + 1 > countStatsToDisplay)
                {
                    upgradeButton.statTexts[i].text = "";
                    continue;
                }

                upgradeButton.statTexts[i].text = ConvertUpgradeInfoToString(upgradeInfo, i, upgradeInfo.packet[0].isWeapon);
            }

            upgradeButton.packetRef = upgradeInfo;
            RectTransform button = upgradeButton.GetComponent<RectTransform>();
            Vector2 setPos = firstUpgradePos;
            setPos.x += spacingBetweenRowUpgrades * rows;
            setPos.y -= spacingBetweenCollumUpgrades * collums;
            button.position = setPos;
            upgradeButton.transform.SetParent(canvas.transform, false);
            rows += 1;
            if(rows >= upgradesPerRow)
            {
                rows = 0;
                collums += 1;
            }
   
            buttons.Add(upgradeButton);
        }

      
    }

    string ConvertUpgradeInfoToString(UpgradeLine upgradeInfo, int index, bool newWeapon)
    {
        string message = "";
        string unit = "";
        bool subtract = false;
        float value = 0;
        switch (upgradeInfo.statsToDisplay[index])
        {
            case WeaponStatType.Damage:
                value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).damage;
                message += "Damage: ";
                break;
            case WeaponStatType.AttackSpeed:
                value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).attackSpeed;
                message += "Attack Speed: ";
                subtract = true;
                unit = "s";
                break;
            case WeaponStatType.Range:
                value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).range;
                message += "Attack Range: ";
                unit = "m";
                break;
            case WeaponStatType.CoolDown:
                value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).coolDown;
                message += "Cooldown: ";
                subtract = true;
                unit = "s";
                break;
            case WeaponStatType.Bounces:
                value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).bounces;
                message += "Bounces: ";
                break;
            case WeaponStatType.NumberOfBullets:
                value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).numberOfBullets;
                message += "Pellets: ";
                break;

        }
      
        if(newWeapon)
         message += value.ToString();
        else
            message += value + " -> " + (value + upgradeInfo.packet[upgradeInfo.levels].amountToAdd) + (subtract ? -value : value) + " ( " + upgradeInfo.packet[upgradeInfo.levels].amountToAdd +unit + " )";
        return message;
    }

   
    public void ClearUI()
    {
        foreach(UpgradeButton button in buttons)
        {
            Destroy(button.gameObject);
        }
    }
     

    public void GainUpgrade(UpgradeLine packet)
    {
    
        if (Time.realtimeSinceStartup < timeSinceUpgradeStarted + waitTimeUntillCardsInteractable)
            return;
        
        if (packet.packet[0].isWeapon)
            upgradeManager.UnlockWeapon(packet);
        else
            upgradeManager.UnlockUpgrade(packet);

        PlayerLevelUpManager.instance.LevelUpFinished();
   
   
        ClearUI();
        GamePauser.instance.PauseGame(false,false, gameObject);
    }

   


}

using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UpgradeSpawnerUI : MonoBehaviour
{

    [SerializeField] GameObject upgradePopupGameobject;
    [SerializeField] float spacingBetweenRowUpgrades;
    [SerializeField] float spacingBetweenCollumUpgrades;
    [SerializeField] Vector2 firstUpgradePos;
    [SerializeField] float upgradesPerRow;
    [SerializeField] GameObject canvas;
    [SerializeField] TextMeshProUGUI upgradeHeader;
    [SerializeField] string newWeaponHeaderText, weaponUpgradeHeaderText;
    

    [SerializeField] float waitTimeUntillCardsInteractable = 0.25f;
    UpgradeManager upgradeManager;

    [SerializeField] Sprite shotgunSprite, teslaSprite, sniperSprite, rocketLauncherSprite, revolverSprite;

    private void Start()
    {
        upgradeManager = FindAnyObjectByType<UpgradeManager>();
        upgradeHeader.text = "";

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
            upgradeButton.backGround.color = upgradeInfo.upgradeColor;
            upgradeButton.buttonDelegate = upgradeDelegate;

            switch(upgradeInfo.weaponType)
            {
                case WeaponType.Shotgun:
                    upgradeButton.weaponSprite.sprite = shotgunSprite;
                    break;
                case WeaponType.Sniper:
                    upgradeButton.weaponSprite.sprite = sniperSprite;
                    break;
                case WeaponType.TeslaCoil:
                    upgradeButton.weaponSprite.sprite = teslaSprite;
                    break;
                case WeaponType.MissleLaucher:
                    upgradeButton.weaponSprite.sprite = rocketLauncherSprite;
                    break;
                case WeaponType.PlayerRevolver:
                    upgradeButton.weaponSprite.sprite = revolverSprite;
                    break;
                  
                    
            }
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
            if (upgradeInfo.packet[0].isWeapon)
                upgradeHeader.text = newWeaponHeaderText;
            else
                upgradeHeader.text = weaponUpgradeHeaderText;
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
               
                if(upgradeInfo.weaponType == WeaponType.PlayerRevolver)
                    value = Revolver.instanse.damage;
                else
                    value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).damage;
                message += "Damage: ";
                break;
            case WeaponStatType.AttackSpeed:
                if (upgradeInfo.weaponType == WeaponType.PlayerRevolver)
                    value = Revolver.instanse.seccondsBetweenShots;
                else
                    value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).attackSpeed;
                message += "Attack Speed: ";
            //    subtract = true;
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
               // subtract = true;
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
            case WeaponStatType.ExplosionRadius:
                value = AllWeaponStats.allWeaponStatsInstance.GetWeaponStat(upgradeInfo.weaponType).explosionRadius;
                message += "Explosive Radius: ";
                break;
            case WeaponStatType.MagCapacity:
                value = Revolver.instanse.magCapacity;
                message += "Mag Capacity: ";
                break;

        }
      
        if(newWeapon)
         message += value.ToString();
        else
            message += value + " -> " + ( value + (subtract ? -upgradeInfo.packet[upgradeInfo.levels].amountToAdd : upgradeInfo.packet[upgradeInfo.levels].amountToAdd)).ToString() + " ( " + upgradeInfo.packet[upgradeInfo.levels].amountToAdd +unit + " )";
        return message;
    }

   
    public void ClearUI()
    {
        foreach(UpgradeButton button in buttons)
        {
            Destroy(button.gameObject);
        }

        upgradeHeader.text = "";
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

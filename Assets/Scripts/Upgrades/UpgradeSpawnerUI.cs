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


    UpgradeManager upgradeManager;

    private void Start()
    {
        upgradeManager = FindAnyObjectByType<UpgradeManager>();
    }

    bool upgradeIsWeapon;
    List<UpgradeButton> buttons = new();
    
    public void DisplayPopups(List<UpgradeLine> upgradesToDisplay)
    {
        GamePauser.instance.PauseGame(true, gameObject);
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
    public void ClearUI()
    {
        foreach(UpgradeButton button in buttons)
        {
            Destroy(button.gameObject);
        }
    }
     

    public void GainUpgrade(UpgradeLine packet)
    {
     
        if (packet.packet[0].isWeapon)
            upgradeManager.UnlockWeapon(packet);
        else
            upgradeManager.UnlockUpgrade(packet);

        PlayerLevelUpManager.instance.LevelUpFinished();
   
   
        ClearUI();
        GamePauser.instance.PauseGame(false, gameObject);
    }

   


}

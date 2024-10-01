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
    
    public void DisplayPopups(List<UpgradeInfoPacket> upgradesToDisplay)
    {
        GamePauser.instance.PauseGame(true);
        buttons.Clear();
        UpgradeButton.ButtonDelegate upgradeDelegate = GainUpgrade; // give this to the button, so it can call back
    
        int rows = 0;
        int collums = 0;
        upgradeIsWeapon = upgradesToDisplay[0].isWeapon;
        foreach (UpgradeInfoPacket upgradeInfo in upgradesToDisplay)
        {
            UpgradeButton upgradeButton = Instantiate(upgradePopupGameobject).GetComponent<UpgradeButton>();

            upgradeButton.titleText.text = upgradeInfo.upgradeTitle;
            upgradeButton.bodyText.text = upgradeInfo.upgradeBody;
            
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
     

    public void GainUpgrade(UpgradeInfoPacket packet)
    {
     
        if (upgradeIsWeapon)
            upgradeManager.UnlockWeapon(packet);
        else
        {
            upgradeManager.UnlockUpgrade(packet);
       
        }
            

  
        ClearUI();
        GamePauser.instance.PauseGame(false);
    }

   


}

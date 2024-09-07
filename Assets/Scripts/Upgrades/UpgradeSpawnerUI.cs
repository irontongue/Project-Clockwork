using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct UpgradeInfoPacket
{

    public string upgradeTitle;
    
    public string upgradeBody;

    public delegate void Upgrade();

    
}
public class UpgradeSpawnerUI : MonoBehaviour
{

    [SerializeField] GameObject upgradePopupGameobject;
    [SerializeField] float spacingBetweenRowUpgrades;
    [SerializeField] float spacingBetweenCollumUpgrades;
    [SerializeField] Vector2 firstUpgradePos;
    [SerializeField] float upgradesPerRow;
    [SerializeField] GameObject canvas;



   List< UpgradeButton> upgradeButtons;
   [SerializeField] UpgradeInfoPacket[] testPackets;
    private void Start()
    {
       // DisplayPopups(testPackets);
    }

    public void DisplayPopups(UpgradeInfoPacket[] upgradesToDisplay)
    {
        upgradeButtons = new List<UpgradeButton>();

        UpgradeButton.ButtonDelegate upgradeDelegate = GainUpgrade;
        int i = 0;

        int rows = 0;
        int collums = 0;
        foreach (UpgradeInfoPacket upgradeInfo in upgradesToDisplay)
        {
            UpgradeButton upgradeButton = Instantiate(upgradePopupGameobject).GetComponent<UpgradeButton>();

            upgradeButton.titleText.text = upgradeInfo.upgradeTitle;
            upgradeButton.bodyText.text = upgradeInfo.upgradeBody;
            
            upgradeButton.buttonDelegate = upgradeDelegate;

            upgradeButton.index = i;
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
            i++;

        }
    }

    public void GainUpgrade(int index)
    {
        print(index);
    }


}

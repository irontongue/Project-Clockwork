using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
[System.Serializable]
public struct UpgradeInfoPacket
{

    public string upgradeTitle;
    
    public string upgradeBody;

    public bool isWeapon;

    public delegate void Upgrade();

  [ShowIf("isWeapon")] public GameObject weaponToEnable;
   
}

public class UpgradeSpawnerUI : MonoBehaviour
{

    [SerializeField] GameObject upgradePopupGameobject;
    [SerializeField] float spacingBetweenRowUpgrades;
    [SerializeField] float spacingBetweenCollumUpgrades;
    [SerializeField] Vector2 firstUpgradePos;
    [SerializeField] float upgradesPerRow;
    [SerializeField] GameObject canvas;




    [SerializeField] List<UpgradeInfoPacket> upgradePackets = new();
    [SerializeField] List<UpgradeInfoPacket> autoWeapons = new();


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F2))
        {
            DisplayPopups(PickRandomUpgrades(true, 3));
        }
        if(Input.GetKeyDown(KeyCode.F3))
        {
            DisplayPopups(PickRandomUpgrades(false, 3));
        }
    }

    public void AddAutoWeapon(UpgradeInfoPacket packet)
    {

    }
   
 
    public List<UpgradeInfoPacket> PickRandomUpgrades(bool isWeapon, int countOfUpgrades)
    {
        List<UpgradeInfoPacket> chosenPackets = new();
        List<UpgradeInfoPacket> packetsToChooseFrom;
        if (isWeapon)
            packetsToChooseFrom = autoWeapons;
        else
            packetsToChooseFrom = upgradePackets;

        if (countOfUpgrades > packetsToChooseFrom.Count)
            countOfUpgrades = packetsToChooseFrom.Count;
        int loopCap = 0;

        while(chosenPackets.Count < countOfUpgrades)
        {
            loopCap++;
            if(loopCap >= 100)
            {
                print(loopCap);
                print( "PICK RANDOM UPGRADE INFINITE LOOP ");
                return null;
            }
           
            UpgradeInfoPacket packet = packetsToChooseFrom[Random.Range(0, packetsToChooseFrom.Count)];
    
            if (chosenPackets.Contains(packet))
                continue;
            
            chosenPackets.Add(packet);
        }

        return chosenPackets;
    }
    bool upgradeIsWeapon;
    List<UpgradeButton> buttons = new();
    
    public void DisplayPopups(List<UpgradeInfoPacket> upgradesToDisplay)
    {
        
        buttons.Clear();
        UpgradeButton.ButtonDelegate upgradeDelegate = GainUpgrade;
        int i = 0;
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
            i++;
            buttons.Add(upgradeButton);
        }

        TempPauseGame();
    }
    public void ClearUI()
    {
        foreach(UpgradeButton button in buttons)
        {
            Destroy(button.gameObject);
            print("GENM");
        }
    }
    public void TempPauseGame()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }
    public void TempUnpause()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }    

    public void GainUpgrade(UpgradeInfoPacket packet)
    {
        if (upgradeIsWeapon)
            UnlockWeapon(packet);
        else
            UnlockUpgrade(packet);

        TempUnpause();
        ClearUI();
    }

    public void UnlockWeapon(UpgradeInfoPacket packet)
    {
        autoWeapons.Remove(packet); 
    }
    public void UnlockUpgrade(UpgradeInfoPacket packet)
    {
        upgradePackets.Remove(packet);
    }


}

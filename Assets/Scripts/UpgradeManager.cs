using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


//this packet is used for both the upgrade and the weapon unlocks. 
public delegate void UpgradeDelagate();
[System.Serializable]
public struct UpgradeInfoPacket 
{

    public string upgradeTitle;

    public string upgradeBody;

    public bool isWeapon; // if this is for a weapon, dont use the delegate, but use the gameobject. Note: this is temporary.

    public UpgradeType upgradeType;

    [ShowIf("upgradeType", UpgradeType.Shotgun)]
    public ShotgunUpgrades shotgunUpgrades;

    [ShowIf("upgradeType", UpgradeType.TeslaCoil)]
    public TeslaCoilUpgrades teslaCoilUpgrades;

    [ShowIf("upgradeType", UpgradeType.Sniper)]
    public SniperUpgrades sniperUpgrades;

    [ShowIf("upgradeType", UpgradeType.WeaponStat)]
    public WeaponType weaponType;
    [ShowIf("upgradeType", UpgradeType.WeaponStat)]
    public WeaponStatType weaponStatType;

    [ShowIf("isWeapon")] public GameObject weaponToEnable;

    public float amountToAdd;

}
[System.Serializable]
public class UpgradeLine // this is a class and not a struct, since you cannot pass structs by refrence grrrrr
{
    public string packetName;
    public UpgradeInfoPacket[] packet;
    public int levels;
}

public class UpgradeManager : MonoBehaviour
{

    [SerializeField] UpgradeType weaponToEdit;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.Shotgun)] List<UpgradeLine> shotgunUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.Sniper)] List<UpgradeLine> sniperUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.TeslaCoil)] List<UpgradeLine> teslaCoilUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.Universal)] List<UpgradeLine> universalUpgrades;
    [SerializeField] List<UpgradeLine> upgradePackets = new();
    [SerializeField] List<UpgradeLine> autoWeapons = new();
    //[SerializeField] List<UpgradeInfoPacket> autoWeapons = new();

    [SerializeField] UpgradeSpawnerUI UpgradeSpawner; // this is what displays the cards
    AllWeaponStats allWeaponStats;
    private void Start()
    {
        allWeaponStats = FindObjectOfType<AllWeaponStats>();

        upgradePackets.AddRange(shotgunUpgrades);
        upgradePackets.AddRange(sniperUpgrades);
        upgradePackets.AddRange(teslaCoilUpgrades);
        upgradePackets.AddRange(universalUpgrades);
    }

    private void Update()
    {
        //manual unlocks. this will be removed
        if (Input.GetKeyDown(KeyCode.F2))
        {
            StartWeaponUnlock();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            StartUpgrade();
        }
    }
    public void StartUpgrade()
    {
        UpgradeSpawner.DisplayPopups(PickRandomUpgrades(upgradePackets, 3));
    }
    public void StartWeaponUnlock()
    {
        UpgradeSpawner.DisplayPopups(PickRandomUpgrades(autoWeapons, 3));
    }
    //this can be called with either a list or single packet
    //any upgrades added here will go into the pool 
    public void AddUpgrade(UpgradeLine packet)
    {
        upgradePackets.Add(packet);
    }
    public void AddUpgrade(UpgradeLine[] packets)
    {
        foreach(UpgradeLine packet in packets)
        {
            upgradePackets.Add(packet);
        }
    }

    //*Liam I just simplified how to choose the packet type by passing the UpgradeInfoPacket rather than it being a bool.*
    public List<UpgradeLine> PickRandomUpgrades(List<UpgradeLine> upgradePacketList, int countOfUpgrades)
    {
        List<UpgradeLine> chosenPackets = new(); // the packets that will be displayed

        List<UpgradeLine> packetsToChooseFrom = upgradePacketList; // refrence to the list to pick from
       
        //if (isWeapon) // choose what list to pick from
        //    packetsToChooseFrom = autoWeapons;
        //else
        //    packetsToChooseFrom = upgradePackets;

        if (countOfUpgrades > packetsToChooseFrom.Count) // if there are less upgrades then the count to display, limit the amount we display to remaining
            countOfUpgrades = packetsToChooseFrom.Count;
        int loopCap = 0;

        while (chosenPackets.Count < countOfUpgrades)// this finds x random upgrades
        {
            loopCap++;
            if (loopCap >= 100) // this code should not fail, but just in case stop after 100 tries
            {
                print("PICK RANDOM UPGRADE INFINITE LOOP: " + loopCap);
                return null;
            }
            int ran = Random.Range(0, packetsToChooseFrom.Count);
            UpgradeLine packet = packetsToChooseFrom[ran]; // pick a random packet

            if (chosenPackets.Contains(packetsToChooseFrom[ran])) // if we picked it allready, try again
                continue;

            chosenPackets.Add(packet);
           
        }

        return chosenPackets;
    }
    // theese is called by the ui upgrade spawner, and just handle removing them
    public void UnlockWeapon(UpgradeLine packet) 
    {
        packet.packet[0].weaponToEnable.SetActive(true);
        autoWeapons.Remove(packet);
    }
    public void UnlockUpgrade(UpgradeLine upgradeLine)
    {
        UpgradeInfoPacket packet = upgradeLine.packet[upgradeLine.levels];
        allWeaponStats.Upgrade(packet);
        upgradeLine.levels += 1;
  
       
        if(upgradeLine.packet.Length <= upgradeLine.levels)
        { 
            print("was too big packetL: " + upgradeLine.packet.Length + " levels:  " + upgradeLine.levels);
            upgradePackets.Remove(upgradeLine);
        }
       
        //if (packet.Upgrade == null)
        //{
        //    print(packet.upgradeTitle + " does not have a delage set!");
        //    return;
        //}
        //packet.Upgrade();
    }

}

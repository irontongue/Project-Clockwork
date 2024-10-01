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

public class UpgradeManager : MonoBehaviour
{

    //theese hold the current avalabe pool 
    [SerializeField] List<UpgradeInfoPacket> upgradePackets = new();
    [SerializeField] List<UpgradeInfoPacket> autoWeapons = new();

    [SerializeField] UpgradeSpawnerUI UpgradeSpawner; // this is what displays the cards
    AllWeaponStats allWeaponStats;
    private void Start()
    {
        allWeaponStats = FindObjectOfType<AllWeaponStats>();
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
    public void AddUpgrade(UpgradeInfoPacket packet)
    {
        upgradePackets.Add(packet);
    }
    public void AddUpgrade(UpgradeInfoPacket[] packets)
    {
        foreach(UpgradeInfoPacket packet in packets)
        {
            upgradePackets.Add(packet);
        }
    }

    //*Liam I just simplified how to choose the packet type by passing the UpgradeInfoPacket rather than it being a bool.*
    public List<UpgradeInfoPacket> PickRandomUpgrades(List<UpgradeInfoPacket> upgradePacketList, int countOfUpgrades)
    {
        List<UpgradeInfoPacket> chosenPackets = new(); // the packets that will be displayed

        List<UpgradeInfoPacket> packetsToChooseFrom = upgradePacketList; // refrence to the list to pick from

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

            UpgradeInfoPacket packet = packetsToChooseFrom[Random.Range(0, packetsToChooseFrom.Count)]; // pick a random packet

            if (chosenPackets.Contains(packet)) // if we picked it allready, try again
                continue;

            chosenPackets.Add(packet);
        }

        return chosenPackets;
    }
    // theese is called by the ui upgrade spawner, and just handle removing them
    public void UnlockWeapon(UpgradeInfoPacket packet) 
    {
        packet.weaponToEnable.SetActive(true);
        autoWeapons.Remove(packet);
    }
    public void UnlockUpgrade(UpgradeInfoPacket packet)
    {
        allWeaponStats.Upgrade(packet);
        upgradePackets.Remove(packet);
        //if (packet.Upgrade == null)
        //{
        //    print(packet.upgradeTitle + " does not have a delage set!");
        //    return;
        //}
        //packet.Upgrade();
    }

}

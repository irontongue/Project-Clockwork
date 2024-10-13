using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


//this packet is used for both the upgrade and the weapon unlocks. 
[System.Serializable]
public struct UpgradeInfoPacket 
{

    public string upgradeTitle;

    public string upgradeBody;

    public bool isWeapon; // if this is for a weapon

    public UpgradeType upgradeType;

    
    [ShowIf("upgradeType", UpgradeType.Shotgun)]public ShotgunUpgrades shotgunUpgrades;
    [ShowIf("upgradeType", UpgradeType.TeslaCoil)]public TeslaCoilUpgrades teslaCoilUpgrades;
    [ShowIf("upgradeType", UpgradeType.Sniper)]public SniperUpgrades sniperUpgrades;

    //[ShowIf("upgradeType", UpgradeType.FlameThrower)] public FlameThrowerUpgrades flameThrowerUpgrades; // UNSLASH WHEN MADE WEAPON SCRIPTS AND ENUM
    //[ShowIf("upgradeType", UpgradeType.Rifle)] public RifleUpgrades rifleUpgrades;
    //[ShowIf("upgradeType", UpgradeType.Dog)] public DogUpgrades dogUpgrades;
    //[ShowIf("upgradeType", UpgradeType.GrenadeLaucher)] public GrenadeLaucherUpgrades grenadeLaucherUpgrades;
    //[ShowIf("upgradeType", UpgradeType.ThrowingKnives)] public ThrowingKnivesUpgrades throwingKnivesUpgrades;

    [ShowIf("upgradeType", UpgradeType.WeaponStat)]public WeaponType weaponType;
    [ShowIf("upgradeType", UpgradeType.WeaponStat)]public WeaponStatType weaponStatType;
   

    [ShowIf("isWeapon")] public GameObject weaponToEnable;

    public float amountToAdd;

}


[System.Serializable]
public class UpgradeLine // this is a class and not a struct, since you cannot pass structs by refrence grrrrr
{
    [FoldoutGroup("$packetName")] public string packetName;
    [FoldoutGroup("$packetName")] public WeaponType weaponType;
    [FoldoutGroup("$packetName")] public UpgradeInfoPacket[] packet;
    [FoldoutGroup("$packetName")] public int levels;
}

public class UpgradeManager : MonoBehaviour
{

    [SerializeField] UpgradeType weaponToEdit;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.Shotgun)] List<UpgradeLine> shotgunUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.Sniper)] List<UpgradeLine> sniperUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.TeslaCoil)] List<UpgradeLine> teslaCoilUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.FlameThrower)] List<UpgradeLine> flameThrowerUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.Rifle)] List<UpgradeLine> rifleUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.Dog)] List<UpgradeLine> dogUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.GrenadeLaucher)] List<UpgradeLine> grenadeLaucherUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.ThrowingKnives)] List<UpgradeLine> throwingKnivesUpgrades;
    [SerializeField, ShowIf("weaponToEdit", UpgradeType.PlayerRevolver)] List<UpgradeLine> playerRevolverUpgrades;




    //[SerializeField, ShowIf("weaponToEdit", UpgradeType.Universal)] List<UpgradeLine> universalUpgrades;

    Dictionary<WeaponType, List<UpgradeLine>> weaponUpgradeDictionary = new(); //Holds all of the upgrade lines to then be put into the upgradePackets list when adding a weapon
    [Header("Presentation Settings")]

    [Space(20f)]

    [Header("RunTime")]
    [ShowInInspector] List<UpgradeLine> upgradePackets = new(); // use show in inspector to see values, but not seralize them,
    [SerializeField] List<UpgradeLine> autoWeapons = new();//oops i did it here as well, rip 20 mins

    [SerializeField] bool gainWeaponOnStart;
    //[SerializeField] List<UpgradeInfoPacket> autoWeapons = new();

    [SerializeField] UpgradeSpawnerUI UpgradeSpawner; // this is what displays the cards
   
    AllWeaponStats allWeaponStats;
    public bool instantlyGainAllUpgrades;
    private void Start()
    {
        allWeaponStats = FindObjectOfType<AllWeaponStats>();
        upgradePackets.AddRange(playerRevolverUpgrades);
        //upgradePackets.AddRange(shotgunUpgrades);
        //upgradePackets.AddRange(sniperUpgrades);
        //upgradePackets.AddRange(teslaCoilUpgrades);
        //upgradePackets.AddRange(universalUpgrades);
        upgradePackets.AddRange(dogUpgrades);

        InitilizeWeaponUpgradeDictionary();
        if(instantlyGainAllUpgrades)
        {
            while(autoWeapons.Count > 0)
            {
                UnlockWeapon(autoWeapons[0]);
            }
            while(upgradePackets.Count > 0)
            {
                UnlockUpgrade(upgradePackets[0]);
            }
        }
        else
        if (gainWeaponOnStart)
            StartWeaponUnlock();
       
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
    float timeUpgradeStarted;
    public void StartUpgrade()
    {
        UpgradeSpawner.DisplayPopups(PickRandomUpgrades(upgradePackets, 3));
        timeUpgradeStarted = Time.time;
    }
    public void StartWeaponUnlock()
    {
        UpgradeSpawner.DisplayPopups(PickRandomUpgrades(autoWeapons, 3));
        timeUpgradeStarted = Time.time;
    }
    //this can be called with either a list or single packet
    //any upgrades added here will go into the pool 
    public void AddUpgrade(UpgradeLine packet)
    {
     
        upgradePackets.Add(packet);
    }
    public void AddUpgrade(UpgradeLine[] packets)
    {
    

        foreach (UpgradeLine packet in packets)
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
        foreach(UpgradeLine up in weaponUpgradeDictionary[packet.weaponType])
        {
            upgradePackets.Add(up);
        }
        autoWeapons.Remove(packet);
    }
    public void UnlockUpgrade(UpgradeLine upgradeLine)
    {
        UpgradeInfoPacket packet = upgradeLine.packet[upgradeLine.levels];
        allWeaponStats.Upgrade(packet);
        upgradeLine.levels += 1;
        
        
        if(upgradeLine.packet.Length <= upgradeLine.levels)
        {
            upgradePackets.Remove(upgradeLine);
        }
       
        //if (packet.Upgrade == null)
        //{
        //    print(packet.upgradeTitle + " does not have a delage set!");
        //    return;
        //}
        //packet.Upgrade();
    }
    void InitilizeWeaponUpgradeDictionary()
    {
        weaponUpgradeDictionary.Add(WeaponType.Shotgun, shotgunUpgrades);
        weaponUpgradeDictionary.Add(WeaponType.TeslaCoil, teslaCoilUpgrades);
        weaponUpgradeDictionary.Add(WeaponType.Sniper, sniperUpgrades);
        weaponUpgradeDictionary.Add(WeaponType.FlameThrower, flameThrowerUpgrades);
        weaponUpgradeDictionary.Add(WeaponType.Rifle, rifleUpgrades);
        weaponUpgradeDictionary.Add(WeaponType.Dog, dogUpgrades);
        weaponUpgradeDictionary.Add(WeaponType.GrenadeLaucher, grenadeLaucherUpgrades);
        weaponUpgradeDictionary.Add(WeaponType.ThrowingKnives, throwingKnivesUpgrades);

    }
}

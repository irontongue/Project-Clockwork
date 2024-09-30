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

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] List<UpgradeInfoPacket> upgradePackets = new();
    [SerializeField] List<UpgradeInfoPacket> autoWeapons = new();

    [SerializeField] UpgradeSpawnerUI UpgradeSpawner;

    private void Update()
    {
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
        UpgradeSpawner.DisplayPopups(PickRandomUpgrades(false, 3));
    }
    public void StartWeaponUnlock()
    {
        UpgradeSpawner.DisplayPopups(PickRandomUpgrades(true, 3));
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

        while (chosenPackets.Count < countOfUpgrades)
        {
            loopCap++;
            if (loopCap >= 100)
            {
                print(loopCap);
                print("PICK RANDOM UPGRADE INFINITE LOOP ");
                return null;
            }

            UpgradeInfoPacket packet = packetsToChooseFrom[Random.Range(0, packetsToChooseFrom.Count)];

            if (chosenPackets.Contains(packet))
                continue;

            chosenPackets.Add(packet);
        }

        return chosenPackets;
    }

    public void UnlockWeapon(UpgradeInfoPacket packet)
    {
        packet.weaponToEnable.SetActive(true);
        autoWeapons.Remove(packet);
    }
    public void UnlockUpgrade(UpgradeInfoPacket packet)
    {
        upgradePackets.Remove(packet);
    }


}

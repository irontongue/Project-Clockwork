using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UpgradeTree
{
    public BaseUpgrade[] upgrades;
}
public class BaseUpgrade
{
    public string title;
    public string description;
    public float value;
    public UpgradeInfoPacket.Upgrade upgradePointer;
}


[CreateAssetMenu(menuName = "ScriptableObjects/BaseUpgrades")]
public class UpgradeInfo : ScriptableObject
{
    public UpgradeTree[] upgradeTrees;
  
}







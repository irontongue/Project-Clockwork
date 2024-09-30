using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUpManager : MonoBehaviour
{
    [SerializeField] int[] expToLvlup;
    int maxLevel;
    float currentEXP;
    int currentLevel;
    public bool readyToLVLup;
    UpgradeManager upgradeManager;

    private void Start()
    {
        maxLevel = expToLvlup.Length;
        upgradeManager = FindAnyObjectByType<UpgradeManager>();
    }

    public void ReciveEXP(float amount)
    {
        currentEXP += amount;

        if (currentEXP >= expToLvlup[currentLevel])
            readyToLVLup = true;
    }
    public void LevelUp()
    {
        GamePauser.instance.PauseGame(true);
        readyToLVLup = false;
        currentEXP -= expToLvlup[currentLevel];
        currentLevel++;
        upgradeManager.StartUpgrade();
    }
}

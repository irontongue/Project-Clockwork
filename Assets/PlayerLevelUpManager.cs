using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelUpManager : MonoBehaviour
{
    [SerializeField] int[] expToLvlup;
    int maxLevel;
    float currentEXP;
    int currentLevel;

    private void Start()
    {
        maxLevel = expToLvlup.Length;
    }

    public void ReciveEXP(float amount)
    {
        currentEXP = amount;

        if (expToLvlup[currentLevel] < currentEXP)
            LevelUp();
    }
    void LevelUp()
    {
        GamePauser.instance.PauseGame(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    [SerializeField] static float goblinSpawnNoiseInterval = 1f, ogerSpawnNoiseInterval = 0.25f;
    [SerializeField] static float goblinDeathNoiseInterval = 2f, ogerDeathNoiseInterval = 0.25f;

   static float currentGSpawn, currentGDeath;
   static float currentOSpawn, currentODeath;

    private void Update()
    {
        currentGDeath -= Time.deltaTime;
        currentGSpawn -= Time.deltaTime;
        currentODeath -= Time.deltaTime;
        currentOSpawn -= Time.deltaTime;
    }

    public static bool NoiseReady(EnemySpawner.Enemies type, bool DeathNoise)
    {
        switch(type)
        {
            case EnemySpawner.Enemies.Melee:
                if (currentGSpawn <= 0 && !DeathNoise)
                {
                    currentGSpawn = goblinSpawnNoiseInterval;
                    return true;
                }
                else if (currentGDeath <= 0 && DeathNoise)
                {
                    currentGDeath = goblinDeathNoiseInterval;
                    return true;
                }
                break;
            case EnemySpawner.Enemies.Ranged:
                if (currentOSpawn <= 0 && !DeathNoise)
                {
                    currentOSpawn = ogerDeathNoiseInterval;
                    return true;
                }
                else if (currentGDeath <= 0 && DeathNoise)
                {
                    currentODeath = ogerDeathNoiseInterval;
                    return true;
                }
                break;
        }
        return false;
    }

}

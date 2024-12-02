using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    [SerializeField] static float goblinSpawnNoiseInterval = 0.5f, ogerSpawnNoiseInterval = 0.25f;
    [SerializeField] static float goblinDeathNoiseInterval = 1f, ogerDeathNoiseInterval = 0.25f;

   static float currentGSpawn, currentGDeath;
   static float currentOSpawn, currentODeath;


    static float genericNoiseInterval = 0.1f;
    static float currentGenericSpawnInterval = 0f;

    private void Update()
    {
        currentGDeath -= Time.deltaTime;
        currentGSpawn -= Time.deltaTime;
        currentODeath -= Time.deltaTime;
        currentOSpawn -= Time.deltaTime;
        currentGenericSpawnInterval -= Time.deltaTime;
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

            default:

                if(currentGenericSpawnInterval < 0)
                {
                    currentGenericSpawnInterval = genericNoiseInterval;
                    return true;
                }

                break;
        }
        return false;
    }

}

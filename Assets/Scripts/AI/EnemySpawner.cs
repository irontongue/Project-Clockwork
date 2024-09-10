using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    struct WaveInfo
    {
        public GameObject[] enemies;
        public GameObject[] spawnPoints;
        public float maxEnemiesToSpawn;
        public float spawnSpeed;

    
    }

    [SerializeField] WaveInfo[] waves;
    [SerializeField] float pathRadiusFromSpawn;
    [SerializeField] LayerMask walkableMask;
    WaveInfo currentWave;

    private void Start()
    {
        StartWave();
    }
    private void Update()
    {
        if(waveSpawning)
            WaveLoop();
    }

    int currentWaveIndex = -1;
    void StartWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex > waves.Length)
            return;

        waveSpawning = true;
        currentWave = waves[currentWaveIndex];
    }

    bool waveSpawning;
    float lastTimeSinceSpawn;
    int spawnedEnemies;

    void WaveLoop()
    {
        lastTimeSinceSpawn -= Time.deltaTime;

        if (lastTimeSinceSpawn > 0)
            return;

        lastTimeSinceSpawn = currentWave.spawnSpeed;

        GameObject spawnedEnemy = Instantiate(currentWave.enemies[Random.Range(0, currentWave.enemies.Length - 1)]);
        spawnedEnemy.transform.position = currentWave.spawnPoints[Random.Range(0, currentWave.spawnPoints.Length - 1)].transform.position; 
      


     
     
        

        spawnedEnemies += 1;
        print(spawnedEnemies);
        if(spawnedEnemies >= currentWave.maxEnemiesToSpawn)
        {
            waveSpawning = false;
        }
    }
}

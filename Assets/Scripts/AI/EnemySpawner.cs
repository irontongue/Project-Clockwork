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
        public float enemiesToKillToEndWave;

    
    }

    [SerializeField] WaveInfo[] waves;
    [SerializeField] float pathRadiusFromSpawn;
    [SerializeField] LayerMask walkableMask;
    [SerializeField] WaveEvent[] startEvents;
    [SerializeField] WaveEvent[] endEvents;

    WaveInfo currentWave;

    private void Start()
    {
        if (waves.Length == 0)
        {
            Debug.LogWarning("EnemeySpawner: Waves have not been set.");
            return;
        }
      //  
    }
    private void Update()
    {
        if(waveSpawning)
            WaveLoop();
    }

    int currentWaveIndex = -1;

    public void StartSpawner()
    {
        StartWave();

        foreach (WaveEvent wEvent in startEvents)
        {
            wEvent.WaveStart();
        }
    }
    void StartWave()
    {
        currentWaveIndex++;
        enemysKilled = 0;
        spawnedEnemies = 0;
        if (currentWaveIndex >= waves.Length)
        {
            AllWavesCleared();
            print("no more waves!");
            return;
        }
          

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
        try
        {
            NavMesh.SamplePosition(currentWave.spawnPoints[Random.Range(0, currentWave.spawnPoints.Length)].transform.position, out NavMeshHit hit, 3, walkableMask);
            spawnedEnemy.transform.position = hit.position;
            spawnedEnemy.GetComponent<NavMeshAgent>().enabled = true;
        }
        catch
        {
            print("Agent:" + name + "Failed to find position");
            spawnedEnemy.SetActive(false);
        }

       
      

        spawnedEnemies += 1;
 
        if(spawnedEnemies >= currentWave.maxEnemiesToSpawn)
        {
            waveSpawning = false;
        }

        
    }
    int enemysKilled;
    public void EnemyKilled()
    {
        enemysKilled++;

        if (enemysKilled >= currentWave.enemiesToKillToEndWave)
            StartWave();
    }

    void AllWavesCleared()
    {
        foreach (WaveEvent wEvent in startEvents)
        {
            wEvent.WaveEnd();
        }
    }
 
}

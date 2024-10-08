using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class EnemySpawner : MonoBehaviour
{
    #region StaticEnemyInfo

    [System.Serializable]
   
    struct EnemySpawnInfo
    {
        public Enemies enemy;
        public int spawnChances;
    }
    public enum Enemies {Melee, Ranged}
   
  

    #endregion
    [System.Serializable]
    struct WaveInfo
    {
        [FoldoutGroup("Wave")] public EnemySpawnInfo[] enemies;
        [FoldoutGroup("Wave")] public float maxEnemiesToSpawn;
        [FoldoutGroup("Wave")] public float spawnSpeed;
        [FoldoutGroup("Wave")] public float enemiesToKillToEndWave;
        [FoldoutGroup("Wave")] public float EXPShare;
        [FoldoutGroup("Wave")] public WaveEvent EndEvent;

    }
  

    [SerializeField] WaveInfo[] waves;
    [SerializeField] float pathRadiusFromSpawn;
    [SerializeField] LayerMask walkableMask;
    [SerializeField] WaveEvent[] spawnerStartEvents;
    [SerializeField] WaveEvent[] spawnerEndEvents;
    [SerializeField] GameObject[] spawnPoints;

    PlayerLevelUpManager playerLevelUpManager;

    WaveInfo currentWave;

    private void Start()
    {
        if (waves.Length == 0)
        {
            Debug.LogWarning("EnemeySpawner: Waves have not been set.");
            return;
        }
        playerLevelUpManager = FindAnyObjectByType<PlayerLevelUpManager>();
      //  StartWave();
    }
    private void Update()
    {
        if(waveSpawning)
            WaveLoop();
    }

    int currentWaveIndex = -1;

    public void StartSpawner(GameObject refGameobject)
    {
        StartWave();
    }
    void StartWave()
    {
       
        foreach (WaveEvent wEvent in spawnerStartEvents)
        {
            wEvent.WaveStart();
        }

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
        spawnPool.Clear();
       
        foreach (EnemySpawnInfo info in currentWave.enemies)
        {
            for(int i = 0; i < info.spawnChances; i++)
            {
                spawnPool.Add(AIInfo.staticEnemyList[info.enemy]);
            }    
        }
     
    }

    bool waveSpawning;
    float lastTimeSinceSpawn;
    int spawnedEnemies;
    public List<GameObject> spawnPool = new();

    Vector3 foundPos;
    [SerializeField] float minSpawnDistanceFromPlayer;
    Vector3 FindSpawnPos()
    {
        int i = 0;
        while (i < 100)
        {
            i++;
            foundPos = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            if (Vector3.Distance(foundPos, playerLevelUpManager.transform.position) < minSpawnDistanceFromPlayer)
                continue;

            return foundPos;

        }

        print("FAILED TO FIND POSITION FOR ENEMY");
       return Vector3.zero;
    }
    void WaveLoop()
    {
        lastTimeSinceSpawn -= Time.deltaTime;

        if (lastTimeSinceSpawn > 0)
            return;

        lastTimeSinceSpawn = currentWave.spawnSpeed;

        GameObject spawnedEnemy = Instantiate(spawnPool[Random.Range(0, spawnPool.Count)]);
        AIBase ai = spawnedEnemy.GetComponent<AIBase>();
        try
        {
            NavMesh.SamplePosition(FindSpawnPos(), out NavMeshHit hit, 3, walkableMask);
            spawnedEnemy.transform.position = hit.position;
            spawnedEnemy.GetComponent<NavMeshAgent>().enabled = true;
        }
        catch
        {
           
            print("Agent:" + name + "Failed to find position");
            spawnedEnemy.SetActive(false);
            return; // give up and try again, this isnt the final solution. 
        }

        ai.spawner = this;
        ai.EXP = currentWave.EXPShare / currentWave.maxEnemiesToSpawn;

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
        {
            if (currentWave.EndEvent != null)
                currentWave.EndEvent.WaveEnd();
            StartWave();
        }
            
    }

    void AllWavesCleared()
    {
        foreach (WaveEvent wEvent in spawnerEndEvents)
        {
            wEvent.WaveEnd();
        }
     //   if (playerLevelUpManager.readyToLVLup)
      //      playerLevelUpManager.LevelUp();
    }
 
}

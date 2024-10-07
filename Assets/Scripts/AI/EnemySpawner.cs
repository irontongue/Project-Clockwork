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
        public EnemySpawnInfo[] enemies;
        public GameObject[] spawnPoints;
        public float maxEnemiesToSpawn;
        public float spawnSpeed;
        public float enemiesToKillToEndWave;
        public float EXPShare;

    }
  

    [SerializeField] WaveInfo[] waves;
    [SerializeField] float pathRadiusFromSpawn;
    [SerializeField] LayerMask walkableMask;
    [SerializeField] WaveEvent[] startEvents;
    [SerializeField] WaveEvent[] endEvents;

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
       
        foreach (WaveEvent wEvent in startEvents)
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
            NavMesh.SamplePosition(currentWave.spawnPoints[Random.Range(0, currentWave.spawnPoints.Length)].transform.position, out NavMeshHit hit, 3, walkableMask);
            spawnedEnemy.transform.position = hit.position;
            spawnedEnemy.GetComponent<NavMeshAgent>().enabled = true;
        }
        catch
        {
            print("Agent:" + name + "Failed to find position");
            spawnedEnemy.SetActive(false);
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
            StartWave();
    }

    void AllWavesCleared()
    {
        foreach (WaveEvent wEvent in startEvents)
        {
            wEvent.WaveEnd();
        }
     //   if (playerLevelUpManager.readyToLVLup)
      //      playerLevelUpManager.LevelUp();
    }
 
}

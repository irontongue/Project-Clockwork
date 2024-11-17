using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class EnemySpawner : MonoBehaviour
{
    #region StaticEnemyInfo

    [System.Serializable]
   
    public struct EnemySpawnInfo
    {
        public Enemies enemy;
        public bool spawnInGroup;
        public int minSpawnCount, maxSpawnCount;
        public int spawnChances;
    }
    public enum Enemies {Melee, Ranged, Slob, LesserFly, Gremlin, EliteMelee, BossSlob}
   
  

    #endregion
    [System.Serializable]
    struct WaveInfo
    {
        [FoldoutGroup("Wave")] public EnemySpawnInfo[] enemies;
        [FoldoutGroup("Wave")] public float maxEnemiesToSpawn;
        [FoldoutGroup("Wave")] public float spawnSpeed;
        [FoldoutGroup("Wave")] public int enemiesToKillToEndWave;
        [FoldoutGroup("Wave")] public float EXPShare;
        [FoldoutGroup("Wave")] public WaveEvent EndEvent;
        [FoldoutGroup("Wave")] public float timeAfterEndingToStartNewWave;
    }
 
    [SerializeField] WaveInfo[] waves;
    [SerializeField] float pathRadiusFromSpawn;
    [SerializeField] LayerMask walkableMask;
    [SerializeField] WaveEvent[] spawnerStartEvents;
    [SerializeField] WaveEvent[] spawnerEndEvents;
    [SerializeField] GameObject[] spawnPoints;

    PlayerLevelUpManager playerLevelUpManager;
    [SerializeField] float minSpawnDistanceFromPlayer;

    
    WaveInfo currentWave;

    private void Start()
    {
        if (waves.Length == 0)
        {
            Debug.LogWarning("EnemeySpawner: Waves have not been set.");
            return;
        }
        playerLevelUpManager = FindAnyObjectByType<PlayerLevelUpManager>();
    }
    private void Update()
    {
        if (GameState.GamePaused)
            return;

        if (waveSpawning)
            WaveLoop();

        if (!autoStartNewWave)
            return;

      //  timeSinceWaveEnded+= Time.deltaTime;
      //  if (timeSinceWaveEnded >= timeToWaitToAutoStartNextWave)
      //  {
      //      if (waveToStart < currentWaveIndex)
      ///          return;
     //       StartWave();
      //  }
          
        
    }
    [Header("DEBUG ONLY")]
    [SerializeField] int currentWaveIndex = -1;

    public void StartSpawner(GameObject refGameobject)
    {
        StartWave();
        foreach (WaveEvent wEvent in spawnerStartEvents)
        {
            wEvent.WaveStart();
        }
        GameState.inCombat = true;
    }
    void StartWave()
    {
        

        currentWaveIndex++;
        enemysKilled = 0;
        spawnedEnemies = 0 - (currentWave.enemiesToKillToEndWave - spawnedEnemies); // since there could be multiple waves going on, reduce spawned enemys by how many enemies are left in the last wave.
        autoStartNewWave = false;
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
                spawnPool.Add(info);
            }    
        }

        StartCoroutine("WaveLoop");
    }

    bool waveSpawning;
    float lastTimeSinceSpawn;
    int spawnedEnemies;
    public List<EnemySpawnInfo> spawnPool = new();

    Vector3 foundPos;
 
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
    bool finalWaveFinished;
   
    EnemySpawnInfo chosenEnemy;
    int groupSpawnCount;
    bool autoStartNewWave;
    float timeSinceWaveEnded;
    float timeToWaitToAutoStartNextWave;
    int waveToStart;
    IEnumerator WaveLoop()
    {
        int index = currentWaveIndex;
        WaveInfo wave = currentWave;
        while(spawnedEnemies < wave.maxEnemiesToSpawn)
        {
            chosenEnemy = spawnPool[Random.Range(0, spawnPool.Count)];
            lastTimeSinceSpawn = currentWave.spawnSpeed;
            if (chosenEnemy.spawnInGroup)
            {
                groupSpawnCount = Random.Range(chosenEnemy.minSpawnCount, chosenEnemy.maxSpawnCount);
                wave.enemiesToKillToEndWave -= (groupSpawnCount - 1);
            }
                
            else
                groupSpawnCount = 1;
            //  GameObject spawnedEnemy = Instantiate(spawnPool[Random.Range(0, spawnPool.Count)]);

            for(int i = 0; i < groupSpawnCount; i++)
            {
                GameObject poolObj = ObjectPooler.RetreiveObject(chosenEnemy.enemy.ToString());
                poolObj.GetComponent<PoolObject>().ReuseObject();
                AIBase ai = poolObj.GetComponent<AIBase>();
                try
                {
                    NavMesh.SamplePosition(FindSpawnPos(), out NavMeshHit hit, 3, walkableMask);
                    poolObj.transform.position = hit.position;
                }
                catch
                {
                    print("Agent:" + name + "Failed to find position");
                    poolObj.gameObject.SetActive(false);
                    continue; // give up and try again, this isnt the final solution. /// or is it
                }
                if (!ai.agentless)
                    ai.agent.enabled = true;
                ai.spawner = this;
                ai.EXP = currentWave.EXPShare / currentWave.maxEnemiesToSpawn;
                ai.gameObject.SetActive(true);
                ai.PlaySpawnSound();

                ai.RandomisePolarOffset();

                spawnedEnemies += 1;

                if (spawnedEnemies >= currentWave.maxEnemiesToSpawn)
                {
                    waveSpawning = false;
                    if (index < currentWaveIndex)
                        yield return null;
                 //   autoStartNewWave = true;
                 //   waveToStart = currentWaveIndex++;
                 //   timeSinceWaveEnded = 0;
                  //  timeToWaitToAutoStartNextWave = wave.timeAfterEndingToStartNewWave;

                }
            }
 
            yield return new WaitForSeconds(wave.spawnSpeed);
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
        GameState.inCombat = false;
        //   if (playerLevelUpManager.readyToLVLup)
        //      playerLevelUpManager.LevelUp();
    }
 
}

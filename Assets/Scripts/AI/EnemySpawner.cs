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
    public enum Enemies {Melee, Ranged, Slob, LesserFly}
   
  

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
        if(waveSpawning)
            WaveLoop();
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
    }
    void StartWave()
    {
        

        currentWaveIndex++;
        enemysKilled = 0;
        spawnedEnemies = 0 - (currentWave.enemiesToKillToEndWave - spawnedEnemies); // since there could be multiple waves going on, reduce spawned enemys by how many enemies are left in the last wave.
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
                spawnPool.Add(info.enemy);
            }    
        }

        StartCoroutine("WaveLoop");
    }

    bool waveSpawning;
    float lastTimeSinceSpawn;
    int spawnedEnemies;
    public List<Enemies> spawnPool = new();

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
    // ReSharper disable Unity.PerformanceAnalysis
    IEnumerator WaveLoop()
    {
        WaveInfo wave = currentWave;
        while(spawnedEnemies < wave.maxEnemiesToSpawn)
        {
            lastTimeSinceSpawn = currentWave.spawnSpeed;

            //  GameObject spawnedEnemy = Instantiate(spawnPool[Random.Range(0, spawnPool.Count)]);
            GameObject poolObj = ObjectPooler.RetreiveObject(spawnPool[Random.Range(0, spawnPool.Count)].ToString());
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
            if(!ai.agentless)
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
     //   if (playerLevelUpManager.readyToLVLup)
      //      playerLevelUpManager.LevelUp();
    }
 
}

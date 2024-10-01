using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AIInfo : SerializedMonoBehaviour
{
    [SerializeField]  Dictionary<EnemySpawner.Enemies, GameObject> enemyList = new();
    public static Dictionary<EnemySpawner.Enemies, GameObject> staticEnemyList;

    private void Start()
    {
        staticEnemyList = enemyList;
    }
}

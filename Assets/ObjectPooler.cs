using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    static public Dictionary<string, List<PoolObject>> objectPool = new();
    static public Dictionary<string, PoolObject> objectRefrences = new();
    static void Update()
    {
        
    }

    static public void InitilizeObjectPool(string identifyer, PoolObject refrenceObject, float amountToPreSpawn = 0)
    {
        if(!objectPool.ContainsKey(identifyer))
        {
            objectPool.Add(identifyer, new List<PoolObject>());
            objectRefrences.Add(identifyer, refrenceObject);
        } 
        else
            print(identifyer + " Allready exists!");

        
    }
    static PoolObject poolObject;
    static public PoolObject RetreiveObject(string identifyer)
    {
        if(objectPool[identifyer].Count > 1)
        {
            poolObject = objectPool[identifyer][0];
            objectPool[identifyer].Remove(poolObject);
            return poolObject;
        }
        else
        {
            poolObject = Instantiate(objectRefrences[identifyer], Vector3.zero, quaternion.identity);
            poolObject.identifyer = identifyer;
            return poolObject;    
        }
    }

    static public void ReturnObject(PoolObject poolObject)
    {
        objectPool[poolObject.identifyer].Add(poolObject);
    }
}

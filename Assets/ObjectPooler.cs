using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public class ObjectPool// this is a class, just to make the code a bit neater down below. since there is list of lists, that makes the code long. 
    {
        public List<GameObject> objectPool;
        public bool loopingList;
        public int currentIndex;
        public int listSize;
    }
    static public Dictionary<string, ObjectPool> objectPoolCollection = new();
    static public Dictionary<string, GameObject> objectRefrences = new();
    static public Dictionary<string, ObjectPool> loopingPool = new();

    private void Awake()
    {
        objectPoolCollection = new();
        objectRefrences = new();
        loopingPool = new();
    }

    //use this whenever you want to start a new object pool. this pool has a dynamic size, and will create a new object if there are none currently in the pool
    // specify the object that you want to be instaniated to fill the pool. can also prespawn objects if wanted.
    static public void InitilizeObjectPool(string identifyer, GameObject refrenceObject, bool loopingList = false, int amountToPreSpawn = 0)
    {
        

        if (objectPoolCollection.ContainsKey(identifyer)) // dont want to ovveride a created pool
        {
            Debug.LogError(identifyer + " Allready exists!");
            return;
        }

        ObjectPool newPool = new();

        newPool.objectPool = new();
        newPool.loopingList = loopingList;
        newPool.listSize = amountToPreSpawn;
        newPool.currentIndex = 0;
        
    
        for(int i = 0; i < amountToPreSpawn; i++)
        {
            newPool.objectPool.Add(Instantiate(refrenceObject, Vector3.zero, quaternion.identity));
        }

        objectPoolCollection.Add(identifyer, newPool);
        objectRefrences.Add(identifyer, refrenceObject);
    }
    static GameObject returnedObject;
    static ObjectPool currentPool;
    //use this to get an object from the specefied pool, you have to create a pool above first. if the object pool was created to be a looping list, this will handle it.
    static public GameObject RetreiveObject(string identifyer)
    {
        currentPool = objectPoolCollection[identifyer];
        if (objectPoolCollection[identifyer].objectPool.Count > 1)
        {
            if (objectPoolCollection[identifyer].loopingList)
            {

                returnedObject = currentPool.objectPool[currentPool.currentIndex];
                currentPool.currentIndex++;
                print(currentPool.currentIndex);
                if (currentPool.currentIndex >= currentPool.listSize)
                    currentPool.currentIndex = 0;
            }
            else
            {
                returnedObject = objectPoolCollection[identifyer].objectPool[0];
                objectPoolCollection[identifyer].objectPool.Remove(returnedObject);
            }
            return returnedObject;
        }
        else
        {
            if(currentPool.loopingList)
            {
                Debug.LogError(identifyer + " Has not beein intilized");
                return null;
            }
            returnedObject = Instantiate(objectRefrences[identifyer], Vector3.zero, quaternion.identity);
            returnedObject.GetComponent<PoolObject>().identifyer = identifyer;
            return returnedObject;
        }
    }
    //when done with a pool object, call this, and it will auto add it to the correct list. if the gameobject derives from pool object, the identifyer can be automatically found.
    // if not you have to pass through an identifyer.
    static PoolObject poolObject;
    static public void ReturnObject(GameObject gObject, string identifyer = "")
    {
        if (identifyer != "")
        {
            objectPoolCollection[identifyer].objectPool.Add(gObject);
            return;
        }

        poolObject = gObject.GetComponent<PoolObject>();

        if (poolObject == null)
        {
            Debug.LogError(gObject.name + " Was not returned to the pool. Return object was called with no identifyer and no pool object component!");
            return;
        }
        objectPoolCollection[poolObject.identifyer].objectPool.Add(gObject);
    }
}

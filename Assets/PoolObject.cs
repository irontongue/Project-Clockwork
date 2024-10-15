using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

// the point of the poolobject class is for objects that require special functionailty for being enabled/disabled. like the AI needs multiple variables reset.
// so it should be a pool object. but a particle system should not be a pool object, since it does not need to be re initilized.
public class PoolObject : MonoBehaviour
{

    [ReadOnly]public string identifyer;

    public virtual void ReuseObject()
    {

    }
    public virtual void DecomissionObject()
    {
        ObjectPooler.ReturnObject(gameObject);
    }
}

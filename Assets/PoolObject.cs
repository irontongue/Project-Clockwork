using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{

    public string identifyer;
    public virtual void ReuseObject()
    {

    }
    public virtual void DecomissionObject()
    {
        ObjectPooler.ReturnObject(this);
    }
}

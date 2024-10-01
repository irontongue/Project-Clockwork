using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEvent : MonoBehaviour
{
    public virtual void WaveStart()
    {

    }
    public virtual void WaveEnd()
    {
        print("Base Wave End");
    }
}

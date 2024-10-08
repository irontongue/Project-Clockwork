using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEventDoor : WaveEvent
{
    [SerializeField] GameObject door;



    public override void WaveStart()
    {
        door.SetActive(true);
        door.GetComponent<Renderer>().enabled = true;
    }
    public override void WaveEnd()
    {
        door.SetActive(false);
    }
}

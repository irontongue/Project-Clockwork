using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatchSpriteLooker : MonoBehaviour
{
    static public List<Transform> lookers = new();
    private void Awake()
    {
        lookers = new();
    }

    static public void AddLooker(Transform me)
    {
        lookers.Add(me);
     
    }
    static public void RemoveLooker(Transform me)
    {
        lookers.Remove(me);
    }

    Vector3 newPos;

    private void Update()
    {
        if (GameState.GamePaused)
            return;
        
        foreach(Transform looker in lookers)
        {
            newPos.x = looker.eulerAngles.x;
            looker.LookAt(PlayerMovement.playerPosition);
            newPos.y = looker.eulerAngles.y;
            newPos.z = looker.eulerAngles.z;
            looker.eulerAngles = newPos;
        }
    }
}

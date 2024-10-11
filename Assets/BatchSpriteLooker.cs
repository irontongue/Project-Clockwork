using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatchSpriteLooker : MonoBehaviour
{
    static public List<Transform> lookers = new();
 
    static public void AddLooker(Transform me)
    {
        lookers.Add(me);
    }
    static public void RemoveLooker(Transform me)
    {
        lookers.Remove(me);
    } 
    private void Update()
    {
        if (GameState.GamePaused)
            return;
        
        foreach(Transform looker in lookers)
        {
            looker.LookAt(PlayerMovement.playerPosition);
        }
    }
}

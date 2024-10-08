using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStaticReference : MonoBehaviour
{
    public static Transform playerTransform;
    public static GameObject playerGameObject;
    public static PlayerDamageHandler playerDamageHandler;
    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        playerGameObject = playerTransform.gameObject;
        playerDamageHandler = playerGameObject.GetComponent<PlayerDamageHandler>();
    }
   
}

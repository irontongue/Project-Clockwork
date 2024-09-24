using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLooker : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform.gameObject;    
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform);
    }
}

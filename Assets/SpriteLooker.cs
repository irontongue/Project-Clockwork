using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLooker : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        // player = FindAnyObjectByType<PlayerMovement>().transform.gameObject;    
        BatchSpriteLooker.AddLooker(this.transform);
    }

    // Update is called once per frame
 
    private void OnDestroy()
    {
        BatchSpriteLooker.RemoveLooker(this.transform);
    }
}

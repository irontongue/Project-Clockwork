using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidOutTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
            PlayerMovement.playerRef.ResetPlayerToSafePos();
    }
}

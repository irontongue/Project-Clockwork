using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidOutTrigger : MonoBehaviour
{
    [SerializeField] GameObject playerRespawnPoint;
    private void OnTriggerEnter(Collider other)
    {
        Vector3 newPos = Vector3.zero;
        if (other.GetComponent<PlayerMovement>())
        {
            if (playerRespawnPoint != null)
                newPos = playerRespawnPoint.transform.position; 
            PlayerMovement.playerRef.GetComponent<PlayerMovement>().ResetPlayerToSafePos(newPos);

        }
    }
}

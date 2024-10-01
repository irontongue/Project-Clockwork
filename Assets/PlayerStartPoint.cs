
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerStartPoint : MonoBehaviour
{
    [SerializeField,ReadOnly] Vector3 playerSpawnPoint;
    [Button]
    void SetStartPos()
    {
        GameObject player = FindAnyObjectByType<PlayerMovement>().gameObject;
        playerSpawnPoint = player.transform.position;
        transform.position = playerSpawnPoint;
    }
    [Button]
    void ResetPlayer()
    {
        GameObject player = FindAnyObjectByType<PlayerMovement>().gameObject;
        player.transform.position = playerSpawnPoint;
    }

}

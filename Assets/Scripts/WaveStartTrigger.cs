using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveStartTrigger : MonoBehaviour
{
    [SerializeField] EnemySpawner spawner;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            spawner.StartSpawner(gameObject);
           
            Destroy(this.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.25f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}

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
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);// tanks: https://discussions.unity.com/t/gizmo-rotation/376793
        Gizmos.color = new Color(0, 0, 1, 0.25f);
   
        Gizmos.DrawCube(transform.position, transform.localScale);
    }


}

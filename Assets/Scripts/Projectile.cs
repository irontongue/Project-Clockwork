using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed;
    public GameObject origin;

    private void Update()
    {
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("HIT");
        PlayerDamageHandler handler = other.GetComponent<PlayerDamageHandler>();
        if (handler == null)
        {
         
            return;
        }

        handler.Damage(damage, origin.transform);
        Destroy(gameObject);

    }
}

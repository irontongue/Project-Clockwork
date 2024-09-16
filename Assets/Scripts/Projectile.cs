using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float speed;

    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        print("HIT");
        PlayerDamageHandler handler = other.GetComponent<PlayerDamageHandler>();
        if (handler == null)
        {
         
            return;
        }

        handler.Damage(damage);
        Destroy(gameObject);

    }
}

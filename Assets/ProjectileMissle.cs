using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMissle : ProjectileBase
{
    [SerializeField] float radius;
    void Update()
    {
        Move();
        //transform.LookAt(transform.forward);

    }
    
    private void OnCollisionEnter(Collision other) {
        Explode(radius, stats.damage, enemiesLM);
        Instantiate(PFXPrefab, transform.position, Quaternion.identity);
        ObjectPooler.ReturnObject(gameObject, "AWML_Missle");
        gameObject.SetActive(false);
    }
}

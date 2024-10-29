
using UnityEngine;

public class ProjectileMissle : ProjectileBase
{
    void Update()
    {
        transform.LookAt(AutomaticWeaponMissleLauncher.lockOnPoint);
        Move();
        //transform.LookAt(transform.forward);

    }

    private void OnCollisionEnter(Collision other) {
        // print(other.transform.name);
        // Selection.activeGameObject=other.gameObject;
        // EditorApplication.isPaused = true;
        Explode(stats.explosionRadius, stats.damage, enemiesLM);
        Instantiate(PFXPrefab, transform.position, Quaternion.identity);
        ObjectPooler.ReturnObject(gameObject, "AWML_Missle");
        gameObject.SetActive(false);
    }
}

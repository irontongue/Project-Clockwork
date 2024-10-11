using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BatchProjectileManager : MonoBehaviour
{
    static List<Projectile> projectiles = new();

    [SerializeField] GameObject projectileObject;
    void Awake()
    {
        ObjectPooler.InitilizeObjectPool("Projectile", projectileObject.GetComponent<PoolObject>());
    }
    static public void AddProjectile(Projectile projectile)
    {
        projectiles.Add(projectile);
    }
    static public void RemoveProjectile(Projectile projectile)
    {
        projectiles.Remove(projectile);
    }

    private void Update()
    {
        if (GameState.GamePaused)
            return;

        if (projectiles.Count <= 0)
            return;

        foreach (Projectile projectile in projectiles)
        {
            if (projectile.active)
                projectile.transform.position += (projectile.transform.forward * projectile.speed * Time.deltaTime);
        }
    }
}

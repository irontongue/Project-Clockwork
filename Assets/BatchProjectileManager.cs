using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatchProjectileManager : MonoBehaviour
{
    static List<Projectile> projectiles = new();

    [SerializeField] GameObject projectileObject;
    [SerializeField] GameObject rockProjectile;
    void Start()
    {
        ObjectPooler.InitilizeObjectPool("Projectile", projectileObject);
        ObjectPooler.InitilizeObjectPool("Rock", rockProjectile);
        projectiles = new();
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
                projectile.transform.position += projectile.speed * Time.deltaTime * projectile.transform.forward;
        }
    }
}

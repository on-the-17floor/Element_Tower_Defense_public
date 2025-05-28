using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : IObjectFactory<TowerBullet>
{
    private GameObject prefab;
    private TowerTier tier;
    public ProjectileFactory(List<GameObject> prefab, TowerTier tier)
    {
        this.prefab = prefab[(int)tier];
        this.tier = tier;
    }

    public TowerBullet CreateInstance()
    {
        GameObject projectile = GameObject.Instantiate(prefab);
        if (projectile.TryGetComponent(out TowerBullet bullet))
        {
            bullet.InitBullet(tier);
            return bullet;
        }

        return null;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectileManager : MonoBehaviour
{
    private const string projectileLable = "ProjectilePrefab";

    [Header("Projectile Base Colors")]
    [SerializeField] private Color[] colors;

    [Header("Projectile Prefab")]
    [SerializeField] private List<GameObject> projectilePrefabs;  // 투사체 프리팹

    // pool
    public Dictionary<TowerTier, ObjectPool<TowerBullet>> pool { get; private set; }

    // get set
    public Color[] Colors => colors;

    private async void Start()
    {
        projectilePrefabs = await ResourceLoader.AsyncLoadLable<GameObject>(projectileLable);

        pool = new Dictionary<TowerTier, ObjectPool<TowerBullet>>();

        // 티어마다 오브젝트풀
        foreach(TowerTier tier in Enum.GetValues(typeof(TowerTier)))
        {
            Transform parent = new GameObject($"{tier}_Bullet").transform;
            parent.parent = transform;

            pool.Add(tier, new ObjectPool<TowerBullet>
                (new ProjectileFactory(projectilePrefabs, tier), 4, parent));
        }
    }

    public bool GetProjectile(ElementType type, TowerTier tier, out TowerBullet bullet)
    {
        bullet = null;
        if(pool.TryGetValue(tier , out var tmpPool))
        {
            bullet = tmpPool.GetPoolAsT();     // 가져오기
            bullet.OnBullet?.Invoke(type);      // 색변경
            return true;
        }

        return false;
    }
}
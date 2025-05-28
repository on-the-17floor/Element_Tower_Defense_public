using System;
using System.Collections.Generic;
using UnityEngine;
using Input = Modoium.Service.Input;

public class TowerManager : Singleton<TowerManager>
{
    public TileSelector TileSelector { get; private set; }      
    public TowerGenerator TowerGenerator { get; private set; }
    public TowerUpgrade TowerUpgrade { get; private set; }
    public TowerDataBase TowerData { get; private set; }
    public TowerProjectileManager projectileManager { get; private set; }
    public HashSet<BaseTower> towerList { get; private set; }

    public Action towerListUpdated = delegate { };

    protected override void Initialize()
    {
        towerList = new HashSet<BaseTower>();

        TileSelector    = FindObjectOfType<TileSelector>();
        TowerGenerator  = FindObjectOfType<TowerGenerator>();
        TowerUpgrade    = FindObjectOfType<TowerUpgrade>();
        TowerData       = FindObjectOfType<TowerDataBase>();
        projectileManager = FindObjectOfType<TowerProjectileManager>();

        // 타워 버프 수치 초기화 이벤트 할당
        // 기본값 : 1
        // 버프 수치는 곱
        towerListUpdated += () => 
        {
            foreach (var tower in towerList)
                tower.DamageBuffRate = 1f; 
        };
    }

    public void AddTower(BaseTower tower)
    {
        towerList.Add(tower);
        towerListUpdated?.Invoke();
    }

    public void RemoveTower(BaseTower tower)
    {
        towerList.Remove(tower);
        Destroy(tower.gameObject);
    }
}
using System.Collections.Generic;
using UnityEngine;

public struct TicketData
{
    public TowerTier tier;
    public ElementType type;

    public TicketData(TowerTier tier, ElementType type)
    {
        this.tier = tier;
        this.type = type;
    }
}

public class TowerGenerator : MonoBehaviour
{
    private TowerManager towerManager;
    private TowerDataBase towerDataBase;
    private TileSelector tileSelector;

    private Queue<TicketData> ticket;

    private void Start()
    {
        ticket = new();

        towerManager = TowerManager.Instance;
        towerDataBase = towerManager.TowerData;
        tileSelector = towerManager.TileSelector;
    }

    public void CreateNewTower(TowerTier tier, bool isCombine = false)
    {
        // 예외처리
        if (tileSelector == null)
            return;

        // 예외처리
        if (tileSelector.CurrentTile == null)
            return;

        // 이미 타워가 존재
        if (tileSelector.CurrentTile.tower != null)
            return;

        TowerData data;
        if(isCombine)
            data = towerDataBase.GetRandomData(tier);
        else
        {
            if (ticket.TryDequeue(out TicketData useTicket))
            {
                tier = useTicket.tier;
                data = towerDataBase.GetTowerData(tier, useTicket.type);
            }
            else
                data = towerDataBase.GetRandomData(tier); 
        }

        // 타워 생성
        GameObject towerPrefab = towerDataBase.GetTowerPrefab(data.tierType, data.elementType);
        GameObject newTower = Instantiate(towerPrefab, tileSelector.CurrentTile.transform);

        // 티어 이펙트 생성
        GameObject effectPrefab = towerDataBase.GetTowerEffect(tier);
        Transform newEffect = Instantiate(effectPrefab).transform;

        // 타워 초기화
        BaseTower tower = newTower.GetComponent<BaseTower>();
        tower.InitTower(data, tileSelector.CurrentTile, newEffect);

        // 타일에 타워 등록
        tileSelector.CurrentTile.RegisterTower(tower);
        tileSelector.ResetTile();

        towerManager.AddTower(tower);

        // 타워 생성 이펙트
        EffectManager.Instance.PlayNormalEffect(EffectName.NewTower, tower.transform.position, out var effect);
        SoundManager.Instance.SFXPlay(SFXType.TowerCreate, tower.transform.position);

        // 타워 생성 시 애널리틱스 이벤트 발생
        Analytics.PlaceDragonEvent(data.elementType, tower.transform.position);

    }
    public void CombineTower(TowerTile mainTile, TowerTile ConsumedTile)
    {
        // 타음 티어
        TowerTier nextTier = mainTile.tower.Data.tierType + 1;

        // 타워 삭제
        //Destroy(mainTile.tower.gameObject);
        //Destroy(ConsumedTile.tower.gameObject);

        towerManager.RemoveTower(mainTile.tower);
        towerManager.RemoveTower(ConsumedTile.tower);

        // 타일 초기화
        mainTile.tower = null;
        ConsumedTile.tower = null;

        // 타일 선택 및 타워 생성
        tileSelector.SelectTile(mainTile);
        CreateNewTower(nextTier, true);

        // 하이퍼 티어 생성 시 애널리틱스 이벤트 발생
        if(nextTier == TowerTier.Hyper)
        {
            Analytics.MergeHyperDragon(StageManager.Instance.CurrentRound);
        }

    }

    public void UseTowerTicket(TowerTier tier, ElementType type)
    {
        ticket.Enqueue(new TicketData(tier, type));
    }
}

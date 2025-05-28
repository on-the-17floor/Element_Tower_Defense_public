using UnityEngine;
using EPOOutline;

public class TowerTile : MonoBehaviour
{
    [SerializeField] private Outlinable outline;
    [SerializeField] private Transform towerPosition;
    public BaseTower tower { get; set; }

    /// <summary>
    /// 타워 위치 초기화
    /// </summary>
    public void ResetTowerPosition()
    {
        tower.transform.position = towerPosition.position;
    }

    /// <summary>
    /// 타워 등록
    /// </summary>
    /// <param name="tower">캐싱해둘 타워</param>
    public void RegisterTower(BaseTower tower)
    {
        this.tower = tower;
        ResetTowerPosition();
    }

    /// <summary>
    /// 합성 조건 확인
    /// </summary>
    /// <param name="tile">조건을 확인할 타일</param>
    /// <returns>합성 가능 여부</returns>
    public bool CombineCheck(TowerTile tile)
    {
        // null 예외처리
        if (tile == null)
            return false;

        // 다른 티어일때
        if (tower.Data.tierType != tile.tower.Data.tierType)
            return false;

        // 다른 속성일때
        if (tower.Data.elementType != tile.tower.Data.elementType)
            return false;

        // 최고등급 예외처리
        if (tile.tower.Data.tierType == TowerTier.Hyper)
            return false;

        return true;
    }

    public void SetOutLineActive(bool active)
    {
        outline.enabled = active;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using Input = Modoium.Service.Input;

[RequireComponent(typeof(TowerTile))]
public class TileDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TowerTile tile;

    [Header("Drag")]
    [SerializeField] private LayerMask dragLayer;
    private float dragY;

    private List<BaseTower> highlistTowers = new();

    private bool dragCanceled = false;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 예외처리
        if (tile.tower == null)
            return;

        if(TowerManager.Instance.TileSelector.IsDragging)
        {
            EventSystem.current.SetSelectedGameObject(null);
            return;
        }

        // 드래그 상태 ( 캔슬 false, 드래그 중 )
        dragCanceled = false;
        tile.tower.IsDragging = true;
        TowerManager.Instance.TileSelector.IsDragging = true;

        // 드래그 고정할 Y값 할당
        dragY = tile.tower.transform.position.y;

        // 타워 드래그 시작 -> 판매 UI ON
        UIManager.Instance.SellZoneUI.OnSellZone(true, tile.tower.Data.tierType);

        // 타워 하이라이트 ON
        foreach (var tower in TowerManager.Instance.towerList)
        {
            // 동일한 티어, 속성 타워 찾기
            if (tile.tower.Data.tierType != tower.Data.tierType)
                continue;

            if (tile.tower.Data.elementType != tower.Data.elementType)
                continue;

            tower.onTowerHighlight.Invoke(true);

            // OFF을 위한 저장
            highlistTowers.Add(tower);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 예외처리
        if (tile.tower == null)
            return;

        if (dragCanceled)
            return;

        // 마우스 위치로 타워 이동
        Vector3 mousePosition = GetMousePosition(dragY);
        tile.tower.transform.position = new Vector3(mousePosition.x, dragY, mousePosition.z);
    }

    private void Update()
    {
        if (tile.tower == null)
            return;

        if (tile.tower.IsDragging == false)
            return;

        if (Input.touchCount > 1)
        {
            // OnEndDrag 호출 방지
            EventSystem.current.SetSelectedGameObject(null);

            // 드래그 상태 ( 캔슬 true, 드래그 끝 )
            dragCanceled = true;
            tile.tower.IsDragging = false;
            TowerManager.Instance.TileSelector.IsDragging = false;

            // 초기화
            tile.ResetTowerPosition();
            UIManager.Instance.SellZoneUI.OnSellZone(false);

            // 타워 하이라이트 초기화
            foreach (BaseTower tower in highlistTowers)
            {
                tower.onTowerHighlight.Invoke(false);
            }
            highlistTowers.Clear();
            return;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 예외처리
        if (tile.tower == null)
            return;

        // 드래그 상태 ( 드래그 끝 )
        tile.tower.IsDragging = false;
        TowerManager.Instance.TileSelector.IsDragging = false;

        // 드래그 결과
        if (!dragCanceled)
            DragResult(eventData);

        // 타워 드래그 종료 -> 판매 UI OFF
        UIManager.Instance.SellZoneUI.OnSellZone(false);
    }

    private void DragResult(PointerEventData eventData)
    {
        // 드래그 종료 후 오브젝트
        GameObject topObject = TowerRayCast(eventData);

        // 드래그 종료했을때 감지된 오브젝트가 없을때
        if (topObject == null)
            tile.ResetTowerPosition();

        // UI위에서 드래그를 멈췄을때  ( 타워 판매 )
        else if (topObject.GetComponentInParent<SellZone>(true))
            SellTower();

        // 타일/타워 위에서 드래그를 멈췄을때 ( 타워 합성 )
        else if (topObject.TryGetComponent(out TowerTile otherTile))
            TryTowerCombine(otherTile);

        // TowerTile이 아닌곳에서 드래그 멈췄을때
        else
            tile.ResetTowerPosition();

        // 타워 하이라이트 OFF
        foreach (BaseTower tower in highlistTowers)
        {
            tower.onTowerHighlight.Invoke(false);
        }
        highlistTowers.Clear();
    }

    private void SellTower()
    {
        TowerTier tier = tile.tower.Data.tierType;

        // 최고등급 티어의 타워는 판매 X
        if (tier == TowerTier.Hyper)
        {
            tile.ResetTowerPosition();
            return;
        }

        int cost = Define.DEFAULT_SELLCOST * (1 << (int)tier);

        UserManager.Instance.AddSummonToken(cost);      // 재화 지급
        TowerManager.Instance.RemoveTower(tile.tower);  // 타워 삭제 -> UI OFF

        SoundManager.Instance.SFXPlay(SFXType.TowerSale, transform.position);
    }

    /// <summary>
    /// 합성 시도
    /// </summary>
    /// <param name="otherTile">재료가 되는 타일</param>
    private void TryTowerCombine(TowerTile otherTile)
    {
        //Tower 없음 / 드래그 시작과 끝이 같은 타일
        if (otherTile.tower == null || otherTile == tile)
        {
            tile.ResetTowerPosition();
            return;
        }

        // 합성 가능 여부 확인
        if (tile.CombineCheck(otherTile) == false)
        {
            tile.ResetTowerPosition();
            return;
        }

        // 타워 합성
        // otherTile ( 메인 ), tile ( 재료 )
        TowerManager.Instance.TowerGenerator.CombineTower(otherTile, tile);
    }

    private Vector3 GetMousePosition(float fixedY)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, fixedY, 0)); // Y 고정 평면

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return transform.position;
    }

    /// <summary>
    /// 드래그 끝났을때 위치에 있는 오브젝트 검출
    /// </summary>
    /// <param name="eventData"></param>
    /// <returns></returns>
    private GameObject TowerRayCast(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // 가장 상단 object 반환 ( UI 포함 )
        if (results.Count > 0)
            return results[0].gameObject;

        return null;
    }
}

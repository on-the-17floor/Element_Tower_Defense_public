using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTower : BaseSpecialAttack
{
    /* 타워 주위 {range} 범위 내 
     * 타워 공격력 {value} 만큼 증가
     */
    [Header("Special Attack Specification")]
    [SerializeField] private LayerMask towerLayer;
    [SerializeField] private float buffRange;
    [SerializeField] private float buffRate;

    // OverlapSphere용 배열  ( 기본 크기 15 )
    private Collider[] overlapResult = new Collider[15];
    public override void Start()
    {
        base.Start();
        SetData(out buffRange, out buffRate);

        // 타워를 생설될 때 이벤트 할당
        // towerList의 내용이 업데이트 될 때 ( 타워 생성, 합성, 판매 )
        // 타워의 버프를 초기화하고, 버프를 다시 할당
        TowerManager.Instance.towerListUpdated += Buff;

        Buff();
    }

    private void Buff()
    {
        int towerCount = Physics.OverlapSphereNonAlloc(
            transform.position, buffRange, overlapResult, towerLayer);

        for(int i = 0; i < towerCount; i++)
        {
            if (overlapResult[i].TryGetComponent(out BaseTower tower) == false)
                continue;

            tower.DamageBuffRate *= buffRate;
        }
    }

    private void OnDestroy()
    {
        //타워가 삭제될 때 이벤트 할당 해제
        TowerManager.Instance.towerListUpdated -= Buff;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashAttack : BaseSpecialAttack, ISpecialAttack
{
    /* 피격 지점 {range} 만큼 범위 공격 
     * 타워 데미지의 50%의 데미지
    */

    [Header("Special Attack Specification")]
    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private float splashRange;
    [SerializeField] private float splashDamageRate;

    // OverlapSphere용 배열  ( 기본 크기 10 )
    private Collider[] overlapResult = new Collider[10];

    public override void Start()
    {
        base.Start();

        SetData(out splashRange, out splashDamageRate);
    }

    public void SpecialAttack(Collider target)
    {
        if (target == null)
            return;

        int enemyCount = Physics.OverlapSphereNonAlloc(target.transform.position, splashRange, overlapResult, unitLayer);

        for(int i = 0; i < enemyCount; i++)
        {
            if (overlapResult[i].TryGetComponent(out BaseUnit unit) == false)
                continue;

            if(unit.IsDeadState) 
                continue;

            // 임시 이펙트
            Vector3 effectPos = unit.transform.position + new Vector3(0, 1f, 0);
            EffectManager.Instance.PlayElementEffect(type, tier, EffectType.Hit, unit.transform.position);

            float damage = owner.CalculateDamage(unit) * splashDamageRate;
            unit.GetDamage(damage);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class BaseTower : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        if (data == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);
    }

    // Manager
    private TowerProjectileManager projectileManager;

    [SerializeField] private Transform attackPosition;
    [SerializeField] private Transform tierPosition;

    [Header("enemy Layer")]
    [SerializeField] private LayerMask targetLayer;

    [Header("Component")]
    [SerializeField] private Animator animator;

    [Header("Highlight")]
    [SerializeField] private GameObject highlight;
    public Action<bool> onTowerHighlight = delegate { };

    [Header("TowerData View")]
    [SerializeField] private TowerData data;
    [SerializeField] private TowerTile tile;
    private float attackDelay = 0;

    private ISpecialAttack specialAttackHandler;

    // GET / SET
    public TowerData Data => data;
    public TowerTile Tile => tile;
    public bool IsDragging { get; set; }
    public float DamageBuffRate { get; set; }

    public void InitTower(TowerData data, TowerTile tile, Transform tierEffect)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, -180, 0));

        // 투사체 매니저
        projectileManager = TowerManager.Instance.projectileManager;

        // 티어 이펙트 위치 초기화
        tierEffect.parent = tierPosition;
        tierEffect.localPosition = Vector3.zero;

        // 데이터 초기화
        this.data = data;
        this.tile = tile;
        IsDragging = false;

        // 특수 공격 핸들러 초기화
        TryGetComponent(out specialAttackHandler);

        // 타워 하이라이트 이벤트
        onTowerHighlight += (active) => 
        {
            Vector3 t = transform.localScale;
            float defaultSize = 3.5f;
            highlight.transform.localScale = new Vector3(defaultSize / t.x, defaultSize / t.y, defaultSize / t.z);
            highlight.SetActive(active); 
        };
    }

    private void Update()
    {
        // 타워 드래그중 동작 X
        if (IsDragging)
            return;

        Attack();
    }

    /// <summary>
    /// 공격 범위 내 몬스터 감지
    /// </summary>
    /// <returns></returns>
    private bool DetectEnemy(out Collider unit)
    {
        unit = null;
        Collider[] targets = Physics.OverlapSphere(transform.position, data.attackRange, targetLayer);

        if (targets.Length == 0)
            return false;

        float minDistance = Mathf.Infinity;
        
        foreach (Collider target in targets)
        {
            float distance = Vector3.SqrMagnitude(target.transform.position - transform.position);
            if(distance < minDistance)
            {
                minDistance = distance;
                unit = target;
            }
        }

        if (unit == null)
            return false;

        return true;
    }

    /// <summary>
    /// 타겟 몬스터 공격
    /// </summary>
    /// 

    private void Attack()
    {

        // 쿨타임
        attackDelay += Time.deltaTime;
        if (attackDelay < data.attackSpeed)
            return;

        // 쿨타임중 몬스터 감지 X
        // 몬스터 감지
        if (DetectEnemy(out Collider target) == false)
            return;

        // 몬스터 바라보기
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);

        // 타워의 총알 가져오기
        if (projectileManager.GetProjectile(data.elementType, data.tierType, out TowerBullet bullet) == false)
            return;

        // 애니메이션
        animator.SetTrigger("Attack");

        // 발사
        bullet.Shoot(attackPosition, target.transform, () =>
        {
            HitDamage(target);
            specialAttackHandler?.SpecialAttack(target);
        });


        // 쿨타임 초기화
        attackDelay = 0;
    }

    private void HitDamage(Collider target)
    {
        if(target == null) 
            return;

        if (target.TryGetComponent(out BaseUnit unit) == false)
            return;

        if (unit.IsDeadState == true)
            return;

        float damage = CalculateDamage(unit);
        // (( 기본데미지 * ( 1.05^강화수치 ) ) * 상성 데이터) * (방어율 * 방어 무시 )

        unit.GetDamage(damage);
    }

    public float CalculateDamage(BaseUnit target)
    {
        // 기본 데미지 * 버프 
        float damage = data.damage * DamageBuffRate;

        // 업그레이드 수치 적용
        damage *= TowerManager.Instance.TowerUpgrade.GetUpgradeRate(data.elementType);

        // 상성 적용
        damage *= Define.ELEMENTAL_DAMAGE_MATRIX[(int)data.elementType, (int)target.UnitData.elementType];

        // 방어율 무시 적용
        float ignoreDefence = target.UnitData.defence * data.ignoreDefenseRate;

        // 방어율 적용
        float defenceRate = 100f / (100f + ignoreDefence);
        damage *= defenceRate;

        return damage;
    }
}

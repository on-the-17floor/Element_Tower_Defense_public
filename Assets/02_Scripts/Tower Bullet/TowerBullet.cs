using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    [Header("Set Bullet Speed")]
    [SerializeField] private float bulletSpeed;

    protected TowerTier poolKey;
    protected ElementType currentType;

    protected TowerProjectileManager projectileManager;
    protected EffectManager effectManager;
    protected ObjectPool<TowerBullet> poolReference;

    protected bool hitEffectPlayed;

    protected Action OnComplete;
    public Action<ElementType> OnBullet = delegate { };
    public Action OnHit;
    public Action OnSFX;

    public virtual void InitBullet(TowerTier tier)
    {
        poolKey = tier;
        projectileManager = TowerManager.Instance.projectileManager;
        effectManager = EffectManager.Instance;
    }

    public virtual void Shoot(Transform start, Transform target, Action onHit)
    {
        // 예외처리
        if (start == null || target == null)
        {
            hitEffectPlayed = true;
            ReturnAsPool();
            return;
        }

        // 이벤트 할당
        this.OnHit = onHit;

        hitEffectPlayed = false;

        // 발사 이펙트 / 효과음
        effectManager.PlayElementEffect(currentType, poolKey, EffectType.Muzzle, start.position);
        OnSFX?.Invoke();

        // 시작위치
        transform.position = start.position;

        Vector3 targetPosition = target.position + Define.BULLET_OFFSET;

        // 목표 바라보기
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
       
        // 발사
        float distnace = Vector3.Distance(start.position, targetPosition);
        float duration = distnace / bulletSpeed;
        transform.DOMove(targetPosition, duration).SetEase(Ease.Linear).OnComplete(ReturnAsPool);
    }

    protected void ReturnAsPool()
    {
        PlayHitEffect();

        if (poolReference == null)
            projectileManager.pool.TryGetValue(poolKey, out poolReference);

        poolReference.SetObject(this);
    }
    protected void PlayHitEffect()
    {
        if(hitEffectPlayed == false)
        {
            OnHit?.Invoke();
            OnHit = null;

            hitEffectPlayed = true;
            effectManager.PlayElementEffect(currentType, poolKey, EffectType.Hit, transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayHitEffect();
    }

}
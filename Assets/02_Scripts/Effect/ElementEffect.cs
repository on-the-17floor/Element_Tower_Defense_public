using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementEffect : MonoBehaviour
{
    [Header("Main Particle System")]
    [SerializeField] private ParticleSystem parentParticle;

    [Header("Change Color")]
    [SerializeField] private ParticleSystem[] particles;

    protected TowerTier poolKey;
    protected EffectType poolType;
    protected EffectManager effectManager;
    protected ObjectPool<ElementEffect> poolReference;

    public Action<ElementType> OnEffect = delegate { };

    public void InitEffect(TowerTier tier, EffectType type)
    {
        poolKey = tier;
        poolType = type;
        effectManager = EffectManager.Instance;

        OnEffect += PlayEffect;
    }

    private void PlayEffect(ElementType type)
    {
        SetElementEffect(type);     // 이펙트 색 변경

        // 파티클 실행 예상시간
        float duration = parentParticle.main.duration
            + parentParticle.main.startLifetime.constantMax;

        parentParticle.Play(true);  // 하위에 있는 파티클시스템도 전부 실행

        // 예상시간 후 ReturnAsPool 실행
        Invoke("ReturnAsPool", duration);
    }

    public void ReturnAsPool()
    {
        if (poolReference == null)
        {
            int key = effectManager.GetEffectKey(poolKey, poolType);
            effectManager.elementPool.TryGetValue(key, out poolReference);
        }

        poolReference.SetObject(this);
    }

    protected virtual void SetElementEffect(ElementType type)
    {
        // 파티클 색 다 변경해주기
        Color baseColor = effectManager.BaseColor[(int)type];

        foreach (var particle in particles)
        {
            var main = particle.main;
            float alpha = main.startColor.color.a;
            main.startColor = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
    }
}

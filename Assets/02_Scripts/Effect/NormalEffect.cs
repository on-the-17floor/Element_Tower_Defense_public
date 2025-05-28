using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem parentParticle;

    private EffectName poolKey;
    private ObjectPool<NormalEffect> poolReference;

    public float duration { get; private set; }

    public void InitEffect(EffectName poolKey)
    {
        this.poolKey = poolKey;
        duration = parentParticle.main.duration + parentParticle.main.startLifetime.constantMax;
    }

    public void PlayEffect()
    {
        // 실행
        parentParticle.Play(true);

        // 예상시간 후 ReturnAsPool 실행
        Invoke("ReturnAsPool", duration);
    }

    public void ReturnAsPool()
    {
        if (poolReference == null)
            EffectManager.Instance.normalPool.TryGetValue(poolKey, out poolReference);

        poolReference.SetObject(this);
    }
}

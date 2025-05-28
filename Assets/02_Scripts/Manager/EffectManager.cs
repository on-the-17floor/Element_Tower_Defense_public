using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
{
    private const string normalEffectLabel = "NormalEffect";
    private const string hitEffectLabel = "HitEffect";
    private const string muzzleEffectLabel = "MuzzleEffect";

    [Header("Element Effect BaseColor")]
    [SerializeField] private Color[] baseColors;

    [Header("Normal Effect Prefab")]
    [SerializeField] private List<GameObject> normalEffects;

    [Header("Tier Hit Effect Prefab")]
    [SerializeField] private List<GameObject> baseHitEffects;

    [Header("Tier Muzzle Effect Prefab")]
    [SerializeField] private List<GameObject> baseMuzzleEffects;

    // pool
    public Dictionary<EffectName, ObjectPool<NormalEffect>> normalPool { get; private set; } 
    public Dictionary<int, ObjectPool<ElementEffect>> elementPool { get; private set; }
    public Color[] BaseColor => baseColors;

    protected override void Initialize()
    {
        // 이펙트 프리팹 리소스 비동기 로드
        InitResource();
    }

    private async void InitResource()
    {
        // 이펙트 프리팹 리소스 로드
        normalEffects = await ResourceLoader.AsyncLoadLable<GameObject>(normalEffectLabel);
        baseHitEffects = await ResourceLoader.AsyncLoadLable<GameObject>(hitEffectLabel);
        baseMuzzleEffects = await ResourceLoader.AsyncLoadLable<GameObject>(muzzleEffectLabel);

        // 리소스 로드가 끝나면 풀 생성
        CreatePool();
    }

    private void CreatePool()
    {
        // 오브젝트 풀 초기화
        normalPool = new Dictionary<EffectName, ObjectPool<NormalEffect>>();
        elementPool = new Dictionary<int, ObjectPool<ElementEffect>>();

        // 일반 이펙트 ( Particle system )
        foreach (EffectName name in Enum.GetValues(typeof(EffectName)))
        {
            Transform effectParent = new GameObject($"{name}_normalEffect").transform;
            effectParent.parent = transform;

            normalPool.Add(name, new ObjectPool<NormalEffect>(
                new EffectFactory(normalEffects[(int)name], name), 2, effectParent));
        }

        // 원소 속성 이펙트 ( Element Effect )
        foreach (TowerTier tier in Enum.GetValues(typeof(TowerTier)))
        {
            Transform hitParent = new GameObject($"{tier}_HitEffect").transform;
            Transform muzzleParent = new GameObject($"{tier}_MuzzleEffect").transform;
            hitParent.parent = transform;
            muzzleParent.parent = transform;

            int hitKey = GetEffectKey(tier, EffectType.Hit);
            int muzzleKey = GetEffectKey(tier, EffectType.Muzzle);
            elementPool.Add(hitKey, new ObjectPool<ElementEffect>
                (new ElementEffectFactory(baseHitEffects, tier, EffectType.Hit), 4, hitParent));
            elementPool.Add(muzzleKey, new ObjectPool<ElementEffect>
                (new ElementEffectFactory(baseMuzzleEffects, tier, EffectType.Muzzle), 4, muzzleParent));
        }
    }

    public int GetEffectKey(TowerTier tier, EffectType type)
    {
        int key = ((int)type * Define.EFFECT_KEY_OFFESET) + (int)tier;
        return key;
    }

    public bool PlayElementEffect(ElementType element, TowerTier tier, EffectType type, Vector3 playPos)
    {
        int key = GetEffectKey(tier, type);
        if(elementPool.TryGetValue(key, out var tmpPool))
        {
            ElementEffect effect = tmpPool.GetPoolAsT();    // 가져오기
            effect.transform.position = playPos;            // 이펙트 실행위치
            effect.OnEffect?.Invoke(element);               // 색변경 및 실행
            return true;
        }
        return false;
    }
    public bool PlayNormalEffect(EffectName name, Vector3 playPos, out NormalEffect effect)
    {
        effect = null;
        if (normalPool.TryGetValue(name, out var tmpPool))
        {
            effect = tmpPool.GetPoolAsT();          // 가져오기
            effect.transform.position = playPos;    // 이펙트 실행위치
            effect.PlayEffect();                    // 실행
            return true;
        }
        return false;
    }
}

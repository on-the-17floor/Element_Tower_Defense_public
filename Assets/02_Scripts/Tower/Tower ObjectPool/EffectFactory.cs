using System.Collections.Generic;
using UnityEngine;

public class ElementEffectFactory : IObjectFactory<ElementEffect>
{
    private GameObject prefab;
    private TowerTier tier;
    private EffectType effectType;

    public ElementEffectFactory(List<GameObject> prefabs, TowerTier tier, EffectType effectType)
    {
        prefab = prefabs[(int)tier];
        this.tier = tier;
        this.effectType = effectType;
    }

    public ElementEffect CreateInstance()
    {
        GameObject effect = GameObject.Instantiate(prefab);
        if (effect.TryGetComponent(out ElementEffect baseEffect))
        {
            baseEffect.InitEffect(tier, effectType);
            return baseEffect;
        }

        return null;
    }
}

public class EffectFactory : IObjectFactory<NormalEffect>
{
    private GameObject prefab;
    private EffectName poolKey;

    public EffectFactory(GameObject prefabs, EffectName poolKey)
    {
        prefab = prefabs;
        this.poolKey = poolKey;
    }

    public NormalEffect CreateInstance()
    {
        GameObject effect = GameObject.Instantiate(prefab);
        if (effect.TryGetComponent(out NormalEffect baseEffect))
        {
            baseEffect.InitEffect(poolKey);
            return baseEffect;
        }

        return null;
    }
}
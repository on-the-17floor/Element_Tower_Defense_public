using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecialAttack
{
    public void SpecialAttack(Collider target);
}

public class BaseSpecialAttack : MonoBehaviour
{
    protected BaseTower owner;
    protected ElementType type;
    protected TowerTier tier;

    public virtual void Start()
    {
        owner = GetComponent<BaseTower>();
        type = owner.Data.elementType;
        tier = owner.Data.tierType;
    }

    protected void SetData(out float range, out float rate)
    {
        TowerSpecialData data = TowerManager.Instance.TowerData.
            GetTowerSpecialData(tier, type);

        range = data.range;
        rate = data.rate;
    }
}

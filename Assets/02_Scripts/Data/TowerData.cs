using Firebase.Database;

[System.Serializable]
public class TowerData : ILoadableFromSnapshot
{
    public TowerTier tierType;
    public ElementType elementType;
    public float damage;
    public float attackSpeed;
    public float attackRange;
    public float ignoreDefenseRate;

    public TowerData(TowerTier tierType, ElementType elementType, float damage, float attackSpeed, float attackRange, float ignoreDefenseRate)
    {
        this.tierType = tierType;
        this.elementType = elementType;
        this.damage = damage;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;
        this.ignoreDefenseRate = ignoreDefenseRate;
    }

    public TowerData()
    {
        tierType = (TowerTier)0;
        elementType = (ElementType)0;
        damage = 0f;
        attackSpeed = 0f;
        attackRange = 0f;
        ignoreDefenseRate = 0f;
    }

    public void LoadFromSnapshot(DataSnapshot snap)
    {
        tierType = Extension.StringToEnum<TowerTier>(snap.Child("tierType").Value.ToString());
        elementType = Extension.StringToEnum<ElementType>(snap.Child("elementType").Value.ToString());

        damage = float.Parse(snap.Child("damage").Value.ToString());
        attackSpeed = float.Parse(snap.Child("attackSpeed").Value.ToString());
        attackRange = float.Parse(snap.Child("attackRange").Value.ToString());
        ignoreDefenseRate = float.Parse(snap.Child("ignoreDefenseRate").Value.ToString());
    }

}
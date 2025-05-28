using Firebase.Database;

[System.Serializable]
public class UnitData : ILoadableFromSnapshot
{
    public ElementType elementType;

    public int num;
    public string name;

    public float maxHp;
    public float speed;
    public float defence;

    public float recoverTime;
    public float recoverRate;

    public int attack;

    public UnitData(ElementType type, int num, string name, float maxHp,
        float speed, float defence, float recoveryTime, float recoveryRate, int attack)
    {
        this.elementType = type;
        this.num = num;
        this.name = name;
        this.maxHp = maxHp;
        this.speed = speed;
        this.defence = defence;
        this.recoverTime = recoveryTime;
        this.recoverRate = recoveryRate;
        this.attack = attack;
    }

    // 복사생성자
    public UnitData(UnitData copy)
    {
        this.elementType = copy.elementType;
        this.num = copy.num;
        this.name = copy.name;
        this.maxHp = copy.maxHp;
        this.speed = copy.speed;
        this.defence = copy.defence;
        this.recoverTime = copy.recoverTime;
        this.recoverRate = copy.recoverRate;
        this.attack = copy.attack;
    }

    public UnitData()
    {
        elementType = (ElementType)0;
        num = 0;
        name = "Name";
        maxHp = 0f;
        speed = 0f;
        defence = 0f;
        recoverTime = 0f;
        recoverRate = 0f;
        attack = 0;
    }

    public void LoadFromSnapshot(DataSnapshot snap)
    {
        elementType = Extension.StringToEnum<ElementType>(snap.Child("elementType").Value.ToString());

        num = int.Parse(snap.Key);
        name = snap.Child("name").Value.ToString();

        maxHp = float.Parse(snap.Child("hp").Value.ToString());
        speed = float.Parse(snap.Child("speed").Value.ToString());
        defence = float.Parse(snap.Child("defence").Value.ToString());

        recoverTime = float.Parse(snap.Child("recoverTime").Value.ToString());
        recoverRate = float.Parse(snap.Child("recoverRate").Value.ToString());

        attack = int.Parse(snap.Child("attack").Value.ToString());
    }
}
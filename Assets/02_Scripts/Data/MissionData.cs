using Firebase.Database;

[System.Serializable]
public class MissionData : ILoadableFromSnapshot
{
    public int level;
    public int monsterIndex;
    public int summonToken;
    public int spawnCount;
    public int coolTime;

    public MissionData(int level, int monsterIndex, int summonToken, int spawnCount, int coolTime)
    {
        this.level = level;
        this.monsterIndex = monsterIndex;
        this.summonToken = summonToken;
        this.spawnCount = spawnCount;
        this.coolTime = coolTime;
    }
    
    public MissionData()
    {
        level = 0;
        monsterIndex = 0;
        summonToken = 0;
        spawnCount = 0;
        coolTime = 0;
    }

    public void LoadFromSnapshot(DataSnapshot snap)
    {
        level = int.Parse(snap.Key);
        monsterIndex = int.Parse(snap.Child("monsterIndex").Value.ToString());
        summonToken = int.Parse(snap.Child("summonToken").Value.ToString());
        spawnCount = int.Parse(snap.Child("spawnCount").Value.ToString());
        coolTime = int.Parse(snap.Child("coolTime").Value.ToString());
    }

}
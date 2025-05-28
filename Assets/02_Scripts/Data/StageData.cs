using Firebase.Database;

[System.Serializable]
public class StageData : ILoadableFromSnapshot
{
    public int round;
    public int monsterIndex;
    public int spawnCount;
    public int summonToken;
    public int upgradToken;

    public StageData(int round, int monsterIndex, int spawnCount, int summonToken, int upgradToken)
    {
        this.round = round;
        this.monsterIndex = monsterIndex;
        this.spawnCount = spawnCount;
        this.summonToken = summonToken;
        this.upgradToken = upgradToken;
    }

    public StageData()
    {
        round = 0;
        monsterIndex = 0;
        spawnCount = 0;
        summonToken = 0;
        upgradToken = 0;
    }

    public void LoadFromSnapshot(DataSnapshot snap)
    {
        round = int.Parse(snap.Key);
        monsterIndex = int.Parse(snap.Child("monsterIndex").Value.ToString());
        spawnCount = int.Parse(snap.Child("spawnCount").Value.ToString());
        summonToken = int.Parse(snap.Child("summonToken").Value.ToString());
        upgradToken = int.Parse(snap.Child("upgradToken").Value.ToString());
    }
}
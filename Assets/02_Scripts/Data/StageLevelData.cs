using Firebase.Database;

[System.Serializable]
public class StageLevelData : ILoadableFromSnapshot
{
    public DifficultyType difficulty;
    public int maxLife;
    public int maxRound;
    public float breakTime;
    public float timeScale;

    public StageLevelData(DifficultyType difficulty, int maxLife, int maxRound, float breakTime, float timeScale)
    {
        this.difficulty = difficulty;
        this.maxLife = maxLife;
        this.maxRound = maxRound;
        this.breakTime = breakTime;
        this.timeScale = timeScale;
    }

    public StageLevelData()
    {
        difficulty = (DifficultyType)0;
        maxLife = 0;
        maxRound = 0;
        breakTime = 0f;
        timeScale = 0f;
    }

    public void LoadFromSnapshot(DataSnapshot snap)
    {
        difficulty = Extension.StringToEnum<DifficultyType>(snap.Child("difficulty").Value.ToString());
        maxLife = int.Parse(snap.Child("maxLife").Value.ToString());
        maxRound = int.Parse(snap.Child("maxRound").Value.ToString());
        breakTime = float.Parse(snap.Child("breakTime").Value.ToString());
        timeScale = float.Parse(snap.Child("timeScale").Value.ToString());
    }

}
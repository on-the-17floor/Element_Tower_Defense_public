using System.Collections.Generic;

// 데이터 보관소
// 서버에서 받아온 데이터를 보관하는 클래스

public static class DataStore
{
    public static UserData userData = new();

    public static List<StageData> stageDataList = new();
    public static List<TowerData> towerDataList = new();
    public static List<MissionData> missionDataList = new();
    public static List<StageLevelData> stageLevelDataList = new();
    public static List<TowerSpecialData> towerSpecialDataList = new();

    // 하드모드 추가하면서 몬스터 리스트 -> 딕셔너리로 변경
    public static Dictionary<DifficultyType, List<UnitData>> monsterDataDict = new();

    // 현재 난이도 
    public static DifficultyType difficultyType;
}
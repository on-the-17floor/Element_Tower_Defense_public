using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

using Random = UnityEngine.Random;

public class DataPatcher
{
    /// <summary>
    /// 서버에서 데이터를 받아와 최신 데이터로 갱신
    /// </summary>
    public async Task UpdateGameData()
    {
        DataSnapshot snap;

        // 차례대로 업데이트
        UpdateLoadingProgress(0f, "Start synchronizing data...");
        await Task.Delay(RandomWaitTime());

        // 노말 난이도 몬스터 데이터
        snap = await FirebaseCRUD.SearchData("jsonData/MonsterData");
        var normalList = FirebaseCRUD.GetDataList<UnitData>(snap);
        DataStore.monsterDataDict.Add(DifficultyType.Normal, normalList);

        // 하드 난이도 몬스터 데이터
        snap = await FirebaseCRUD.SearchData("jsonData/MonsterData_Hard");
        var hardList = FirebaseCRUD.GetDataList<UnitData>(snap);
        DataStore.monsterDataDict.Add(DifficultyType.Hard, hardList);

        UpdateLoadingProgress(0.2f, "Monster data synchronization completed");
        await Task.Delay(RandomWaitTime());

        snap = await FirebaseCRUD.SearchData("jsonData/TowerData");
        DataStore.towerDataList = FirebaseCRUD.GetDataList<TowerData>(snap);
        snap = await FirebaseCRUD.SearchData("jsonData/TowerSpecialData");
        DataStore.towerSpecialDataList = FirebaseCRUD.GetDataList<TowerSpecialData>(snap);
        UpdateLoadingProgress(0.4f, "Tower data synchronization completed");
        await Task.Delay(RandomWaitTime());

        snap = await FirebaseCRUD.SearchData("jsonData/StageData");
        DataStore.stageDataList = FirebaseCRUD.GetDataList<StageData>(snap);
        UpdateLoadingProgress(0.6f, "Stage data synchronization completed");
        await Task.Delay(RandomWaitTime());

        snap = await FirebaseCRUD.SearchData("jsonData/MissionData");
        DataStore.missionDataList = FirebaseCRUD.GetDataList<MissionData>(snap);
        UpdateLoadingProgress(0.8f, "Mission data synchronization completed");
        await Task.Delay(RandomWaitTime());

        snap = await FirebaseCRUD.SearchData("jsonData/StageLevelData");
        DataStore.stageLevelDataList = FirebaseCRUD.GetDataList<StageLevelData>(snap);

        UpdateLoadingProgress(1f, "All data synchronization completed");
        await Task.Delay(RandomWaitTime());
    }

    /// <summary>
    /// 갱신한 데이터를 로컬에 json 형태로 저장
    /// </summary>
    public void SaveGameData()
    {
        SaveLoader.SaveList<StageData>(DataStore.stageDataList, nameof(StageData));
        SaveLoader.SaveList<TowerData>(DataStore.towerDataList, nameof(TowerData));
        SaveLoader.SaveList<MissionData>(DataStore.missionDataList, nameof(MissionData));
        SaveLoader.SaveList<StageLevelData>(DataStore.stageLevelDataList, nameof(StageLevelData));
        SaveLoader.SaveList<TowerSpecialData>(DataStore.towerSpecialDataList, nameof(TowerSpecialData));

        for (int i = 0; i < Extension.EnumCount<DifficultyType>(); i++)
        {
            var list = DataStore.monsterDataDict[(DifficultyType)i];
            SaveLoader.SaveList<UnitData>(list, nameof(UnitData) + $"_{i}");
        }

        //SaveLoader.SaveDictionary<DifficultyType, List<UnitData>>(DataStore.monsterDataDict, nameof(UnitData));
    }

    /// <summary>
    /// json 형태로 저장된 로컬 데이터를 불러와서 사용
    /// </summary>
    public async Task LoadGameData()
    {
        // 차례대로 업데이트
        UpdateLoadingProgress(0f, "Bringing up stored data...");
        await Task.Delay(RandomWaitTime());

        DataStore.stageDataList = SaveLoader.LoadList<StageData>(nameof(StageData));
        UpdateLoadingProgress(0.2f, "Stage Data Load Completed");
        await Task.Delay(RandomWaitTime());

        for(int i=0; i< Extension.EnumCount<DifficultyType>(); i++)
        {
            var list = SaveLoader.LoadList<UnitData>(nameof(UnitData) + $"_{i}");
            DataStore.monsterDataDict.Add((DifficultyType)i, list);
        }
        UpdateLoadingProgress(0.4f, "Monster Data Load Completed");

        DataStore.towerDataList = SaveLoader.LoadList<TowerData>(nameof(TowerData));
        DataStore.towerSpecialDataList = SaveLoader.LoadList<TowerSpecialData>(nameof(TowerSpecialData));
        UpdateLoadingProgress(0.6f, "Tower Data Load Completed");
        await Task.Delay(RandomWaitTime());

        DataStore.missionDataList = SaveLoader.LoadList<MissionData>(nameof(MissionData));
        UpdateLoadingProgress(0.8f, "Mission Data Load Completed");
        await Task.Delay(RandomWaitTime());

        DataStore.stageLevelDataList = SaveLoader.LoadList<StageLevelData>(nameof(StageLevelData));
        UpdateLoadingProgress(1f, "All Data Load Completed");
        await Task.Delay(RandomWaitTime());
    }

    private void UpdateLoadingProgress(float progress, string message)
    {
        DataSyncManager.Instance.OnLoadingProgressChange?.Invoke(progress, message);
        Debug.Log($"로딩 진행률 : {progress:P2} - {message}");
    }

    /// <summary>
    /// 랜덤 초 반환 
    /// </summary>
    private int RandomWaitTime()
    {
        // 0.1초 ~ 0.5초 사이에서 return
        return Random.Range(100, 500);
    }

}
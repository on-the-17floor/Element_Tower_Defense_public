using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManger : Singleton<EnemyManger>
{
    [SerializeField]
    public Transform[] point;

    [Header("===Enemy Prefabs===")]
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private List<GameObject> missionPrefabs;

    [Header("===Container===")]
    private Dictionary<int, UnitData> numToData;
    private UnitFactory unitFactory;

    [Header("===Data==")]
    [SerializeField] private List<UnitData> unitDataList;

    public Dictionary<int, UnitData> NumToData { get => numToData;  }

    protected override void Initialize()
    {
        // 현재 난이도에 따라 데이터 가져오기
        DifficultyType type = DataStore.difficultyType;
        switch (type) 
        {
            case DifficultyType.Normal:
                unitDataList = DataStore.monsterDataDict[DifficultyType.Normal];
                Debug.Log("================현재 Normal 모드 =================");
                break;
            case DifficultyType.Hard:
                unitDataList = DataStore.monsterDataDict[DifficultyType.Hard];
                Debug.Log("================현재 Hard 모드 =================");
                break;
        }

        // 펙토리 생성
        unitFactory = new UnitFactory();

        // 딕셔너리 초기화 
        InitDataDictionary();

        // 이동 포인트 
        point = StageManager.Instance.wayPoint.GetWayPoint();
    }

    private async void Start()
    {
        // 프리팹 가져오기 : 스테이지별 순서대로
        enemyPrefabs = await ResourceLoader.AsyncLoadLable<GameObject>("Monster");
        // 프리팹 가져오기 : 미션 순서대로
        missionPrefabs = await ResourceLoader.AsyncLoadLable<GameObject>("Monster_Mission");

        // InitPoolTransform();
        // InitFactory();
        // InitPool();
    }

    private void InitDataDictionary()
    {
        if (numToData == null)
            numToData = new Dictionary<int, UnitData>();

        // 리스트를 딕셔너리로
        for (int i = 0; i < unitDataList.Count; i++)
        {
            numToData.Add(unitDataList[i].num, unitDataList[i]);
        }
    }

    private GameObject ReturnEnemyPrefab(int monsterNum , int currStageNum = 0) 
    {
        // 미션몬스터는 인덱스가 91부터
        if (monsterNum >= 90) 
        {
            int index = monsterNum % 90 - 1;
            return missionPrefabs[index];
        }

        return enemyPrefabs[currStageNum];
    }

    private UnitData MonsterData(int monsterNum)
    {
        if (numToData.TryGetValue(monsterNum, out UnitData data))
            return data;

        return null;
    }

    public GameObject GetEnemy(int monsterNum)
    {
        // 0 부터 시작 
        int stageNum = StageManager.Instance.CurrentRound;

        // 데이터 , 프리팹 
        UnitData data = MonsterData(monsterNum);
        GameObject monsterIns = ReturnEnemyPrefab(monsterNum , stageNum);
        
        if (data == null)
        {
            Debug.LogError($"No monster data corresponding to the number {monsterNum}");
            return null;
        }

        // 펙토리 조립
        unitFactory.AssemblyEnemyComponent(data, monsterIns);
        // 펙토리에서 생성 
        GameObject temp = unitFactory.CreateInstance();

        return temp;

        /*
        // 인덱스 검사
        if (monsterNum < 0)
            return null;

        if (numToPool.TryGetValue(monsterNum, out ObjectPool<BaseUnit> data)) 
        {
            return numToPool[monsterNum].GetPool();
        }

        Debug.LogWarning($"Failed to get enemy. No pool exists for monster Number: {monsterNum}");
        return null;
        */
    }



    // 이전코드
    /*

    private void InitPoolTransform() 
    {

    // Pooling Trs 제작
    if (parent == null)
    {
        parent = new GameObject("EnemyPoolingParent").transform;

        foreach (var element in numToData) 
        {
            int num = element.Key;
            UnitData data = element.Value;

            GameObject child = new GameObject(data.num.ToString());
            child.transform.SetParent(parent);
        }
    }
    // parent 하위 넣기 
    numToTransform = new Dictionary<int, Transform>();

    int idx = 0;
    foreach(var element in numToData)
    {
        int num = element.Key;
        UnitData data = element.Value;

        Transform trs = parent.GetChild(idx);

        numToTransform.Add(num, trs);

        idx++;
    }
    }
    private void InitFactory() 
    {
    if (numToEnemyFactory == null)
        numToEnemyFactory = new Dictionary<int, IEnemyFactory<BaseUnit>>();

    // 딕셔너리에 있는 갯수만큼
    foreach(var element in numToData)
    { 
        int num = element.Key; 
        UnitData data = element.Value;

        // 펙토리 생성
        IEnemyFactory<BaseUnit> factory = new UnitFactory();
        factory.AssemblyEnemyComponent(data, baseUnitPrefab);

        // 딕셔너리에 추가
        numToEnemyFactory.Add(num, factory);
    }

    }

    private void InitPool() 
    {
    if (numToPool == null)
    {
        numToPool = new Dictionary<int, ObjectPool<BaseUnit>>();
    }

    foreach(var element in numToEnemyFactory) 
    {
        int num = element.Key;
        IEnemyFactory<BaseUnit> data = element.Value;

        // Trs범위를 넘으면 안됨
        if (numToTransform.TryGetValue(num, out Transform tranform))
        {
            // 딕셔너리에 추가 
            numToPool.Add(num, new ObjectPool<BaseUnit>(data, 10, tranform));
        }
        else 
        {
            Debug.LogError($"Pool transform index out of range for unit Number: {num}");
        }
    }

    }

    public void ReturnToPool(BaseUnit unit) 
    {
    int idx = unit.UnitData.num;

    if (numToPool.TryGetValue(idx, out ObjectPool<BaseUnit> temp))
    {
        numToPool[idx].SetObject(unit.gameObject);
    }
    else 
    {
        Debug.LogWarning($"Failed to return unit to pool. No pool exists for unit type: {idx}");
    }

    }
    */

}

using System.Collections.Generic;
using UnityEngine;

public class TowerDataBase : MonoBehaviour
{
    private const string towerPrefabLabel = "TowerPrefab";
    private const string towerEffectLabel = "TowerEffect";

    [Header("Prefab")]
    [SerializeField] private List<GameObject> towerElementPrefab;       // 타워 속성별 프리팹
    [SerializeField] private List<GameObject> towerTierEffect;          // 타워 티어별 이펙트

    [Header("Data")]
    [SerializeField] private List<TowerData> towerData;                 // 타워데이터 ( 데이터테이블
    [SerializeField] private List<TowerSpecialData> towerSpecialData;   // 특수능력 데이터
    private Dictionary<(TowerTier, ElementType), TowerSpecialData> specialDictionary;

    private async void Start()
    {
        towerData = DataStore.towerDataList;
        towerSpecialData = DataStore.towerSpecialDataList;
        SetSpecialDictionary();

        towerElementPrefab = await ResourceLoader.AsyncLoadLable<GameObject>(towerPrefabLabel);
        towerTierEffect = await ResourceLoader.AsyncLoadLable<GameObject>(towerEffectLabel);
    }

    private void SetSpecialDictionary()
    {
        specialDictionary = new();

        foreach (var data in towerSpecialData)
        {
            TowerTier tier = data.tierType;
            ElementType type = data.elementType;                                                                                                

            specialDictionary.Add((tier, type), data);
        }
    }

    public TowerData GetRandomData(TowerTier tier)
    {
        // tier의 랜덤 속성 타입
        // 타워로 만들수있는 속성 ( 불 물 풀 땅 빛 ) 중 랜덤
        int randomType = Random.Range(0, Define.ELEMENT_COUNT);
        int dataIndex = randomType + (Define.ELEMENT_COUNT * (int)tier);

        return towerData[dataIndex];
    }

    public TowerData GetTowerData(TowerTier tier, ElementType type)
    {
        // 타워의 티어/속성 데이터
        int dataIndex = GetElementTowerIndex(tier, type);
        return towerData[dataIndex];
    }

    public GameObject GetTowerPrefab(TowerTier tier, ElementType type)
    {
        // 타워의 티어/속성 프리팹
        int dataIndex = GetElementTowerIndex(tier, type);
        return towerElementPrefab[dataIndex];
    }

    public GameObject GetTowerEffect(TowerTier tier)
    {
        // 타워의 티어 프리팹
        return towerTierEffect[(int)tier];
    }

    public int GetElementTowerIndex(TowerTier tier, ElementType type)
    {
        return (int)type + (Define.ELEMENT_COUNT * (int)tier);
    }

    public TowerSpecialData GetTowerSpecialData(TowerTier tier, ElementType type)
    {
        if(specialDictionary.TryGetValue((tier, type), out var data))
            return data;

        return null;
    }

    public bool CheckAssestLoaded()
    {
        if (towerElementPrefab == null || towerTierEffect == null)
            return false;

        if (towerElementPrefab.Count == 0 || towerTierEffect.Count == 0)
            return false;

        return true;
    }
}

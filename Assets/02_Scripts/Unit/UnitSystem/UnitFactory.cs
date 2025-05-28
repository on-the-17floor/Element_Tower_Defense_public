using System;
using UnityEngine;

public class UnitFactory
{
    private GameObject prefab;
    private UnitData unitData;

    public void AssemblyEnemyComponent(UnitData data, GameObject obj)
    {
        // 깊은복사 
        this.unitData = new UnitData(data);
        // 저장만 
        this.prefab = obj;
    }

    public GameObject CreateInstance()
    {
        //인스턴스화
        GameObject monsterIns = UnityEngine.GameObject.Instantiate(prefab);

        BaseUnit unit = monsterIns.GetComponent<BaseUnit>();

        if (unit == null)
            unit = monsterIns.AddComponent<BaseUnit>();
        
        // 동작 인터페이스 생성
        IRun run = new UnitRun(unit, monsterIns.GetComponent<MonoBehaviour>());
        IRecovery reco = new UnitRecovery(unit, monsterIns.GetComponent<MonoBehaviour>());

        // 동작 조립
        unit.InitRunning(run);
        unit.InitRecovery(reco);
        unit.InitUnitdata(unitData);


        return unit.gameObject;
    }
}

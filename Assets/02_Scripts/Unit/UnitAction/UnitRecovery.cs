using System;
using System.Collections;
using UnityEngine;

public class UnitRecovery : IRecovery
{
    private BaseUnit owner;
    private MonoBehaviour mono;
    private Action? onRecovery;

    public UnitRecovery(BaseUnit unit, MonoBehaviour mono)
    {
        this.owner = unit;
        this.mono = mono;
    }

    public void IRecovery(Action action)
    {
        this.onRecovery = action;

        mono.StartCoroutine(Recovery());
    }

    IEnumerator Recovery() 
    { 
        while (true) 
        {
            // owner의 hp를 owner.unitData의 회복시간마다, 이미 max이면 x
            if (owner.Hp < owner.UnitData.maxHp) 
            {
                float amount = owner.UnitData.maxHp * ( owner.UnitData.recoverRate / 100);

                // 최대 hp안넘도록
                double temp = Mathf.Min(owner.Hp + amount, owner.UnitData.maxHp);

                // 2자리수 반올림
                owner.Hp = (float)Math.Round(temp , 2 );

                // 회복 액션 실행
                onRecovery?.Invoke();
            }

            yield return new WaitForSeconds(owner.UnitData.recoverTime);

        }
    }
}

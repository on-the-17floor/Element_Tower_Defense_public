using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowAttack : BaseSpecialAttack, ISpecialAttack
{
    [Header("Special Attack Specification")]
    [SerializeField] private float slowDuration;
    [SerializeField] private float slowRate;

    // 중복 방지를 위한 Dictionary
    private Dictionary<BaseUnit, Coroutine> slowCoroutines = new();

    public override void Start()
    {
        base.Start();

        SetData(out slowDuration, out slowRate);
    }

    public void SpecialAttack(Collider target)
    {
        if (target == null)
            return;

        if (target.TryGetComponent(out BaseUnit unit) == false)
            return;

        if (this == null)
            return;
        // 지속시간 초기화
        if(slowCoroutines.TryGetValue(unit, out Coroutine coroutine))
            StopCoroutine(coroutine);

        unit.SlowEnemy(slowRate);

        slowCoroutines[unit] = StartCoroutine(SlowDelay(unit, slowDuration));
    }

    private IEnumerator SlowDelay(BaseUnit unit, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 예외처리
        if (unit != null)
        {
            unit.DoneSlowEnemy();
            slowCoroutines.Remove(unit);
        }
    }
}

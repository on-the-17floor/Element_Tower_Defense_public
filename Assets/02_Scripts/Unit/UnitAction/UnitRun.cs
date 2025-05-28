using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRun : IRun
{
    private BaseUnit owner;
    private Transform ownerTrs;
    private MonoBehaviour mono;
    private Action? reachToEndAction;

    public UnitRun(BaseUnit unit , MonoBehaviour mono )
    {
        this.owner = unit;
        this.ownerTrs = owner.gameObject.transform;
        this.mono = mono;
    }

    public void IRun(Transform[] point , Action action)
    {
        this.reachToEndAction = action;

        // StageManger에서 Turning Point 받아오기 
        mono.StartCoroutine(Run(point));
    }

    IEnumerator Run(Transform[] position)
    {
        Vector3 startPosition;
        Vector3 nextPosition;

        for (int i = 0; i < position.Length - 1; i++)
        {
            startPosition = position[i].position;
            nextPosition = position[i + 1].position;

            // 이동 + 회전 
            yield return mono.StartCoroutine(MoveAndRotateToward(startPosition, nextPosition));
        }

        // 끝까지 도달 시
        reachToEndAction?.Invoke();

        // 코루틴 종료
        yield break;
    }

    IEnumerator MoveAndRotateToward(Vector3 currPosition, Vector3 nextPosition)
    {
        // 이동
        float journeyLength = Vector3.Distance(currPosition, nextPosition);
        float moveProgress = 0f;

        float duration = 0.2f;
        float elapsed = 0f;

        // 방향벡터정규화
        Vector3 direction = (nextPosition - currPosition).normalized;

        // 플레이어 회전 : 시작회전 / 도착 회전 
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion startRotation = ownerTrs.rotation;

        // 패널 회전 : 시작회전 / 도착 회전 
        Quaternion startPanelRotation = owner.UnitHpBar.panelRotation;
        Quaternion targetPanelRotation = TargetRotation(direction);

        while (true)
        {
            if (moveProgress >= 1f)
                break;

            if (elapsed < duration)
            {
                // 시간 기반 진행도 계산
                float timeProgress = elapsed / duration;

                // 오브젝트 : Slerp로 부드럽게 회전
                ownerTrs.rotation = Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    timeProgress
                );

                // 패널 : Slerp로 부드럽게 회전
                owner.UnitHpBar.panelRotation = Quaternion.Slerp(
                    startPanelRotation,
                    targetPanelRotation,
                    timeProgress
                );

                elapsed += Time.deltaTime;
            }
            else 
            {
                // 회전 끝 : 정확한 수치 
                ownerTrs.rotation = targetRotation;
                owner.UnitHpBar.panelRotation = targetPanelRotation;
            }

            // 이동 (BaseUnit의 UnitSpeed)
            float step = owner.UnitSpeed * Time.deltaTime / journeyLength;
            moveProgress += step;

            moveProgress = Mathf.Clamp01(moveProgress);

            // 다음 포인트로 이동 
            ownerTrs.transform.position = Vector3.Lerp(currPosition, nextPosition, moveProgress);

            yield return null;
        }

        ownerTrs.transform.position = nextPosition;
        ownerTrs.rotation = targetRotation;

        yield break;
    }

    private Quaternion TargetRotation(Vector3 direction) 
    {
        // 4방향 확인 
        if (direction.x >= 0.9f) // 오른쪽
        {
            return Quaternion.Euler(45, -90, 0);
        }
        else if (direction.x <= -0.9f) // 왼쪽
        {
            return Quaternion.Euler(45, 90, 0);
        }
        else if (direction.z >= 0.9f) // 위
        {
            return Quaternion.Euler(45, 0, 0);
        }
        else // 아래 (direction.z < -0.9f)
        {
            return Quaternion.Euler(45, 180, 0);
        }
    }
}

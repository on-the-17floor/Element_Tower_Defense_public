
using UnityEngine;

[CreateAssetMenu(fileName = "newMission", menuName = "hiddenMission/Kill-Mission")]
public class KillMissionData : HiddenMissionData, IHiddenMission
{
    [Header("Mission Data")]
    public int targetCount;
    public int currentCount;

    public void initialize()
    {
        isRewardGiven = false;
        currentCount = 0;

        StageManager.Instance.OnUnitDeath += TryCompleteMission;
    }

    public void TryCompleteMission()
    {
        if (isRewardGiven)
            return;

        currentCount++;

        // 클리어 확인
        if (targetCount <= currentCount)
        {
            GiveReward();   // 보상 지급
            StageManager.Instance.OnUnitDeath -= TryCompleteMission;    // 이벤트 구독 취소
        }
    }
}
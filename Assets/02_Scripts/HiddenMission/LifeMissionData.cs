
using UnityEngine;

[CreateAssetMenu(fileName = "newMission", menuName = "hiddenMission/life-Mission")]
public class LifeMissionData : HiddenMissionData, IHiddenMission
{
    [Header("Mission Data")]
    public int targetCouunt;
    public int currentCount;

    public void initialize()
    {
        Debug.Log("초기화됨");

        isRewardGiven = false;
        currentCount = 0;

        UserManager.Instance.onLifeChange += TryCompleteMission;
    }

    public void TryCompleteMission()
    {
        if (isRewardGiven)
            return;

        currentCount++;

        // 클리어 확인
        if (targetCouunt <= currentCount)
        {
            GiveReward();   // 보상 지급
            UserManager.Instance.onLifeChange -= TryCompleteMission;    // 이벤트 구독 취소
        }
    }
}
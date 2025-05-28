
using UnityEngine;

[CreateAssetMenu(fileName = "newMission", menuName = "hiddenMission/Reward-Mission")]
public class RewardMissionData : HiddenMissionData, IHiddenMission
{
    [Header("Mission Data")]
    public RewardType targetRewardType;    // 미션 재화
    public int targetRewardValue;   // 미션 재화 수치

    // TODO:
    // 리팩토링
    public void initialize()
    {
        isRewardGiven = false;

        switch (targetRewardType)
        {
            case RewardType.SummonToken:
                UserManager.Instance.onSummonTokenChange += TryCompleteMission;
                break;
            case RewardType.UpgradeToken:
                UserManager.Instance.onUpgradTokenChange += TryCompleteMission;
                break;
            case RewardType.RewardTicket:
                UserManager.Instance.onRewardTicketChange += TryCompleteMission;
                break;
        }
    }

    // TODO:
    // 리팩토링
    public void TryCompleteMission()
    {
        if (isRewardGiven)
            return;

        switch (targetRewardType)
        {
            case RewardType.SummonToken:
                if (targetRewardValue <= UserManager.Instance.SummonToken)
                {
                    GiveReward();
                    UserManager.Instance.onSummonTokenChange -= TryCompleteMission;
                }
                break;
            case RewardType.UpgradeToken:
                if (targetRewardValue <= UserManager.Instance.UpgradToken)
                {
                    GiveReward();
                    UserManager.Instance.onUpgradTokenChange -= TryCompleteMission;
                }
                break;
            case RewardType.RewardTicket:
                if (targetRewardValue <= UserManager.Instance.RewardTicket)
                {
                    GiveReward();
                    UserManager.Instance.onRewardTicketChange -= TryCompleteMission;
                }
                break;
        }
    }
}
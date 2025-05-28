
using UnityEngine;

[CreateAssetMenu(fileName = "defaultData", menuName = "hiddenMission/default-Mission")]
public class HiddenMissionData : ScriptableObject
{
    [Header("Clear Data")]
    public bool isRewardGiven;

    [Header("Reward Data")]
    public RewardType[] rewardType;
    public int[] rewardValue;

    /// <summary>
    /// 리워드 지급 메서드
    /// </summary>
    public void GiveReward()
    {
        isRewardGiven = true;

        for(int i = 0; i < rewardType.Length; i++)
        {
            RewardType type = rewardType[i];
            int value = rewardValue[i];

            switch (rewardType[i])
            {
                case RewardType.SummonToken:
                    UserManager.Instance.AddSummonToken(value);
                    break;
                case RewardType.UpgradeToken:
                    UserManager.Instance.AddUpgradToken(value);
                    break;
                case RewardType.RewardTicket:
                    UserManager.Instance.RewardTicket+= value;
                    break;
            }
        }

        UIManager.Instance.GetMessageUI<RewardMessage>(MessageUIType.RewardMessage)
            .OnMessage(Define.MESSAGE_KEY_HIDDENMISSION, rewardType, rewardValue);
    }
}
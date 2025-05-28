
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newMission", menuName = "hiddenMission/Tower-Mission")]
public class TowerMissionData : HiddenMissionData, IHiddenMission
{
    [Header("Mission Data")]
    public TowerTier[] targetTier;
    public ElementType[] targetElement;

    private Dictionary<(TowerTier, ElementType), int> targetCounts;

    public void initialize()
    {
        isRewardGiven = false;

        targetCounts = new Dictionary<(TowerTier, ElementType), int>();
        for (int i = 0; i < targetTier.Length; i++)
        {
            var key = (targetTier[i], targetElement[i]);
            if(targetCounts.ContainsKey(key))
                targetCounts[key]++;
            else
                targetCounts[key] = 1;
        }

        TowerManager.Instance.towerListUpdated += TryCompleteMission;
    }

    public void TryCompleteMission()
    {
        if (isRewardGiven)
            return;

        // 복사본
        var tmpDict = new Dictionary<(TowerTier, ElementType), int>(targetCounts);

        foreach (var tower in TowerManager.Instance.towerList)
        {
            var key = (tower.Data.tierType, tower.Data.elementType);

            if(tmpDict.TryGetValue(key, out int value))
            {
                value--;

                if (value > 0)
                    tmpDict[key] = value;
                else
                    tmpDict.Remove(key);
            }
                
            if (tmpDict.Count == 0)
                break;
        }

        if (tmpDict.Count > 0)
            return;

        GiveReward();
        TowerManager.Instance.towerListUpdated -= TryCompleteMission;
    }
}
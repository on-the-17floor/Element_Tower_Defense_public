using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    [SerializeField] private int[] upgradeData;

    public int this[ElementType type]
    {
        get 
        {
            // 어둠 속성 예외처리
            if (type == ElementType.Dark)
                return 0;

            return upgradeData[(int)type]; 
        }
    }
    public int[] UpgradeData => upgradeData;

    private void Start()
    {
        // 빛, 어둠 속성 제외
        int upgradeLength = Define.ELEMENT_COUNT;
        upgradeData = new int[upgradeLength];
    }

    public void ElementUpgrade(ElementType type)
    {
        upgradeData[(int)type]++;
    }

    /// <summary>
    /// 업그레이드 수치에 맞는 데미지 상승 배율 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetUpgradeRate(ElementType type)
    {
        int upgradeLevel = upgradeData[(int)type];
        return Mathf.Pow(Define.UPGRADE_RATE, upgradeLevel);
    }
}

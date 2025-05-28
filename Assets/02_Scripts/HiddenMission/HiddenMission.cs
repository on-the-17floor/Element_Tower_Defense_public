using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenMission : MonoBehaviour
{
    /*
        1 킬 수 보상 250 킬  ( 완료
        +200골드

        2 킬 수 보상 500킬   ( 완료
        +300골드

        3 킬 수 보상 750킬   ( 완료
        +400골드

        4 킬 수 보상 1000킬  ( 완료 
        +600골드

        5 라이프가 3개 남았을 때 ( 완료 
        +800강화석

        6 1개의 타워로 클리어 
        +100골드

        7 1500골드 모으기  ( 완료 
        +500골드

        8 [퍼플] 빛속성 5개
        +500골드/+500강화석

        9 전리품 3개 모으기  ( 완료 
        전리품 1개

        10 종류별로 하이퍼티어 모으기
        +1000강화석
     */

    [SerializeField] private HiddenMissionData[] hiddenMissions;

    private void Start()
    {
        foreach(IHiddenMission hiddenMission in hiddenMissions)
        {
            // 히든 미션 초기화
            hiddenMission.initialize();
        }
    }
}
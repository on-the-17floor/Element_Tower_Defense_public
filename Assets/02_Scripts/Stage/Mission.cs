using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Mission : MonoBehaviour
{

    private int _level;
    public Action OnCoolTimeEnded;
    public Action OnCoolDown;

    public SpawnPoint spawnPoint;
    public MissionData data;

    private int coolTime;
    public int CoolTime
    {
        get { return coolTime; }
        set
        {
            coolTime = value;
            OnCoolDown?.Invoke();
            if (coolTime <= 0)
            {
                //쿨타임 종료
                Debug.Log("쿨타임 종료");
                IsChallenging = false;
            }
        }
    }


    private bool isChallenging;
    public bool IsChallenging
    {
        get { return isChallenging; }
        private set
        {
            isChallenging = value;
            if (!isChallenging)
            {
                //미션 도전 가능
                Debug.Log("미션 도전 가능");
                OnCoolTimeEnded?.Invoke();
                coolTime = data.coolTime;
            }
        }
    }

    private void Awake()
    {
        //임시...
        _level = int.Parse(this.gameObject.name);

        data = StageManager.Instance.missionData[_level];
        spawnPoint = StageManager.Instance.spawnPoint;

        coolTime = data.coolTime;
        isChallenging = false;
    }


    public void MissionStart()
    {
        // 만약 아직 쿨타임이 덜 돌았다면 미션 시작 불가
        if (IsChallenging) return;

        IsChallenging = true;
        spawnPoint.SpawnUnit(data.monsterIndex, data.spawnCount);
        StartCoroutine(StartTimer(data.coolTime));

    }

    public void Fail()
    {
        //Debug.Log($"미션 실패: Life {data.penalty} 감소");
        //UserManager.Instance.TakeDamage(data.penalty);

        Debug.Log("미션 실패: Life 3 감소");
    }

    public void Clear()
    {
        // 애널리틱스 이벤트 실행
        Analytics.KillMissionMonsterEvent(_level, StageManager.Instance.CurrentRound);

        Debug.Log($"미션 성공: 소환석 {data.summonToken} 지급");
        UserManager.Instance.AddSummonToken(data.summonToken);

        UIManager.Instance.GetMessageUI<RewardMessage>(MessageUIType.RewardMessage)
            .OnMessage(Define.MESSAGE_KEY_MISSION, RewardType.SummonToken, data.summonToken);
    }
    
    private IEnumerator StartTimer(int limit)
    {
        for (int i = 0; i < limit; i++)
        {
            //Debug.Log("쿨타임 끝나기까지: " + CoolTime);
            yield return new WaitForSeconds(1f);
            CoolTime--;
        }
    }

    public bool CheckIndex(int index, bool isDead)
    {
        if (data.monsterIndex == index)
        {
            if (isDead) Clear();
            else Fail();

            return true;
        }
        else
        {
            return false;
        }            
    }

    public void SetData(int level)
    {
        data = StageManager.Instance.missionData[level];
        spawnPoint = StageManager.Instance.spawnPoint;

        coolTime = data.coolTime;
        isChallenging = false;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Input = Modoium.Service.Input;

public class StageManager : Singleton<StageManager>
{
    [Header("===Component===")]
    public WayPoint wayPoint;
    public SpawnPoint spawnPoint;

    private Coroutine timerCoroutine;

    // 액션
    public Action OnRoundChange;        // 라운드 변경 시 호출되는 델리게이트
    public Action OnUnitDeath;          // 적 사망 시 호출되는 델리게이트
    public Action OnTimerPause;
    public Action OnTimerResume;

    [Header("===Conitaner===")]
    private List<StageLevelData> stageLevelData = new(); //스테이지 레벨 데이터
    private List<StageData> stageData = new(); //스테이지 데이터
    public List<MissionData> missionData = new(); //미션 데이터

    public Dictionary<int, Mission> missionDictionary = new();
    public List<Mission> missionList = new();

    [Header("===Stage===")]
    public DifficultyType difficulty;   //난이도
    public int maxRound;                //최대 라운드
    private int _currentRound;            //현재 라운드
    public int maxCount;                //최대 적의 수
    [SerializeField] private int _currentCount; //현재 적의 수
    private float _startTime;                //게임 시작 시간
    private float _endTime;                  //게임 종료 시간
    private float _restTimer;                //휴식 시간
    [SerializeField] private int _missionCount;

    private float _pauseTime = 0f;
    private float pauseStartTime = 0f;
    private float pauseEndTime = 0f;

    #region 프로퍼티

    public int CurrentRound
    {
        get { return _currentRound; }
        private set
        {

            if (value >= maxRound)
            {
                //게임 클리어
                if (UserManager.Instance.Life > 0)
                {
                    GameClear();
                    return;
                }
                    
            }
            else
            {
                _currentRound = value;

                // 변화 감지
                OnRoundChange?.Invoke();  

                // 다음 라운드 세팅
                SetRound();

                // 휴식 시간 타이머 세팅
                SetTimer();
            }
        }
    }

    public int CurrentCount
    {
        get { return _currentCount; }
        set
        {
            _currentCount = value;
            //Debug.Log("현재 적의 수: " + _currentCount);

            if (_currentCount <= 0 && CurrentRound <= maxRound)
            {
                if (UserManager.Instance.Life > 0)
                {
                    //라운드 클리어
                    //다음 라운드로 넘어간다.
                    RoundClear();
                }
            }
        }
    }

    public float RestTimer
    {
        get { return _restTimer; }
        private set
        {
            _restTimer = value;
            //Debug.Log("RestTimer: " + _restTimer);
            if (_restTimer == 5)
            {
                int nextRound = CurrentRound + 1;
                if (nextRound % 10 == 0)
                {
                    // 보스 라운드
                    UIManager.Instance.GetMessageUI<BossWaveUI>(MessageUIType.BossMessage)
                        .OnMessage();
                }
                else
                {
                    // 일반 라운드
                    UIManager.Instance.GetMessageUI<WaveUI>(MessageUIType.WaveMessage)
                        .OnMessage();
                }
            }
            if (_restTimer <= 0)
            {
                //라운드 시작
                RoundStart();
            }
        }
    }
    #endregion

    private void OnValidate()
    {
        /// <summary> OnValidate 관련 </summary>
        ///
        /// OnValidate는 에디터 함수이기 때문에
        /// Find를 사용해도 빌드한 최종 결과물에는 영향을 주지 않음
        /// 다만 너무 남용할 경우 오류가 생길 수 있으니 GetComponent정도만 사용할 것

        wayPoint = GameObject.Find("WayPoint")?.GetComponent<WayPoint>();
        //spawnPoint = GameObject.Find("SpawnPoint")?.GetComponent<SpawnPoint>();
    }


    protected override void Initialize()
    {
        // 데이터 싱크 매니저에서 데이터를 받아온다
        stageData = DataStore.stageDataList;
        missionData = DataStore.missionDataList;
        stageLevelData = DataStore.stageLevelDataList;

        // 받아온 데이터로 미션 세팅
        for (int i = 0; i < missionData.Count; i++)
        {
            GameObject missionGO = new GameObject($"{i}");
            missionGO.transform.SetParent(this.transform);
            Mission mission = missionGO.AddComponent<Mission>();
            mission.SetData(i);
            missionList.Add(mission);

            //missionList.Add(new GameObject($"{i}").AddComponent<Mission>());
            //missionList[i].SetData(i);
        }

        // 난이도에 따른 게임 데이터 설정
        maxRound = stageLevelData[(int)DifficultyType.Normal].maxRound;
        _restTimer = stageLevelData[(int)DifficultyType.Normal].breakTime;
    }

    private void Start()
    {
        OnTimerPause += InitPauseTimer;
        OnTimerPause += StartPause;

        OnTimerResume += EndPause;
        OnTimerResume += CalculationPauseTime;
        OnTimerResume += InitPauseTimer;

        Debug.Log("StageManager Start");

        if (wayPoint == null)
        {
            Debug.Log("StageManager 초기화 실패");
            return;
        }

        _pauseTime = 0f;
        GameStart(0);
        UserManager.Instance.RestartGameData();
    }

    public void GameStart(int round)
    {
        CurrentRound = round;
        _startTime = Time.unscaledTime;
        SetTimer();
        Analytics.InitAnalyticsEvent();
    }


    void SetTimer()
    {
        // 타이머 초기화
        RestTimer = stageLevelData[(int)DifficultyType.Normal].breakTime;

        // 이미 돌고있는 타이머가 있는지 확인
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // 휴식 시간 타이머 시작
        timerCoroutine = StartCoroutine(StartRestTimer(_restTimer));

    }

    private IEnumerator StartRestTimer(float limit)
    {
        for (int i = 0; i < limit; i++)
        {
            //Debug.Log($"남은 시간: {_restTimer}");
            yield return new WaitForSeconds(1f);
            RestTimer--;
        }

        timerCoroutine = null;
    }

    // 휴식시간 끝나면 시작
    void RoundStart()
    {
        // 몬스터 스폰    
        spawnPoint.SpawnUnit(stageData[CurrentRound].monsterIndex,
                             stageData[CurrentRound].spawnCount);
    }

    // 라운드 값이 바뀔 때마다 호출
    void SetRound()
    {
        CurrentCount = stageData[CurrentRound].spawnCount;
    }

    // 몬스터 count가 0이 되면 호출
    void RoundClear()
    {
        // 보상 지급
        GiveReward();

        // 다음 라운드로 넘어간다
        CurrentRound++;  
    }

    // 라운드 클리어 시 보상 지급
    void GiveReward()
    {
        Debug.Log("====== GiveReward ======");

        // stageData에 따른 보상 지급
        UserManager.Instance.AddSummonToken(stageData[CurrentRound].summonToken);
        UserManager.Instance.AddSummonToken(stageData[CurrentRound].upgradToken);

        List<RewardType> rewardTypes = new() { RewardType.SummonToken };
        List<int> rewardValues = new() { stageData[CurrentRound].summonToken };

        if(stageData[CurrentRound].upgradToken > 0)
        {
            rewardTypes.Add(RewardType.UpgradeToken);
            rewardValues.Add(stageData[CurrentRound].upgradToken);
        }

        //클리어한 라운드가 보스 라운드였다면 전리품 선택권++
        if ((CurrentRound+1) % 10 == 0 && CurrentRound != 0)
        {
            UserManager.Instance.RewardTicket++;
            UIManager.Instance.GetMessageUI<RewardMessage>(MessageUIType.RewardMessage)
                .OnMessage(Define.MESSAGE_KEY_STAGE, RewardType.RewardTicket, 1);
        }

        UIManager.Instance.GetMessageUI<RewardMessage>(MessageUIType.RewardMessage)
            .OnMessage(Define.MESSAGE_KEY_STAGE, rewardTypes.ToArray(), rewardValues.ToArray());
    }

    // 게임 오버
    public void GameOver()
    {
        // 애널리틱스 이벤트 실행
        Analytics.EndAnalyticsEvent(false);

        _endTime = Time.unscaledTime;

        // UI 매니저에서 게임 오버 UI 띄우기
        GameOver gameoverUI = (GameOver)UIManager.Instance.PopupUIs[(int)PopupUIType.GameOverUI];
        gameoverUI.Show();
        gameoverUI.GameResult(GameResult.ResultDefeat);

        // 임시 코드
        Debug.Log("====== GameOver ======");
        Time.timeScale = 0; // 게임 정지
    }

    // 게임 클리어
    public void GameClear()
    {
        // 애널리틱스 이벤트 실행
        Analytics.EndAnalyticsEvent(true);

        _endTime = Time.unscaledTime;

        // UI 매니저에서 게임 클리어 UI 띄우기
        // UI 매니저에서 게임 오버 UI 띄우기
        GameOver gameoverUI = (GameOver)UIManager.Instance.PopupUIs[(int)PopupUIType.GameOverUI];
        gameoverUI.Show();
        gameoverUI.GameResult(GameResult.ResultVictory);

        // 임시 코드
        Debug.Log("====== GameClear ======");

        Time.timeScale = 0; // 게임 정지
    }
    
    // 게임 재시작 (미구현)
    public void Restart()
    {
        UserManager.Instance.RestartGameData();
        GameStart(0);
    }

    // 사라지는 몬스터로부터 식별번호를 받아오는 메서드
    public void SendIndex(int num, bool isDead)
    {
        // 몬스터가 사망했다면 이벤트 실행
        if (isDead)
        {
            OnUnitDeath?.Invoke();
        }

        foreach (var mission in missionList)
        {
            if (mission.CheckIndex(num, isDead))
            {
                return;
            }
        }

        CurrentCount--;
    }

    // 실행할 미션을 선택하는 메서드 (UI에서 사용?)
    public void SelectMission(int level)
    {
        if (level < 0 || level >= missionData.Count)
        {
            Debug.LogError("존재하지 않는 미션 레벨이 입력됨");
            return;
        }

        missionList[level].MissionStart();
    }


    /// <summary>속성 강화 메서드</summary>
        /// 특정 속성의 공격력을 강화
        /// 강화를 할 때마다 소모되는 강화석이 +1씩 증가
        /// 즉 구매가격 = 강화수치 +1
    public bool UpgradeElement(ElementType type, out int level)
    {
        // 소모 강화석 계산
        level = TowerManager.Instance.TowerUpgrade[type];
        int price = level + 1;

        // 충분한 강화석이 있는지 확인 (없을시 false 반환)
        if (!UserManager.Instance.HasEnoughUpgradToken(price))
        {
            return false;
        }

        // 강화석 소모
        UserManager.Instance.UseUpgradToken(price);

        // 해당 속성 강화
        TowerManager.Instance.TowerUpgrade.ElementUpgrade(type);

        level++;
        return true;
    }

  
    #region 플레이 타임 계산 메서드
    private int GetPlayTime(float start, float end)
    {
        int playtime = (int)(end - start);
        Debug.Log($"playtime: {playtime}");
        return playtime;
    }

    public void GetPlayTime(out int minutes, out int seconds)
    {
        int playtime = GetPlayTime(_startTime, _endTime);

        minutes = playtime / 60;
        seconds = playtime % 60;
    }

    public string GetPlayTime()
    {
        int playtime = GetPlayTime(_startTime, _endTime);
        int pauseTime = (int)_pauseTime;

        Debug.Log($"Total playtime: {playtime}");
        Debug.Log($"Total pauseTime: {pauseTime}");

        playtime = playtime - pauseTime;

        int minutes = playtime / 60;
        int seconds = playtime % 60;

        Debug.Log($"GetPlayTime playtime: {playtime}");
        Debug.Log($"{minutes}:{seconds}");
        return $"{minutes}:{seconds}";
    }

    // 정지된 시간 계산 관련 메서드들
    public void StartPause()
    {
        pauseStartTime = Time.unscaledTime;
        Debug.Log($"pauseStartTime: {pauseStartTime}");
    }
    public void EndPause()
    {
        if (pauseStartTime == 0f)
            return;

        pauseEndTime = Time.unscaledTime;
        Debug.Log($"pauseEndTime: {pauseEndTime}");
    }
    public void CalculationPauseTime()
    {
        if (pauseStartTime == 0f)
            return;

        int _pauseTime = GetPlayTime(pauseStartTime, pauseEndTime);

        if(_pauseTime < 0)
            return;

        this._pauseTime += _pauseTime;

        // 확인용 테스트 코드
        Debug.Log($"PauseTime: {_pauseTime}");
        Debug.Log($"Total PauseTime: {this._pauseTime}");
    }


    public void InitPauseTimer()
    {
        pauseStartTime = 0f;
        pauseEndTime = 0f;
    }

    //private void OnDistroy()
    //{
    //    // 이벤트 해제
    //    OnTimerPause -= InitPauseTimer;
    //    OnTimerPause -= StartPause;
    //    OnTimerResume -= EndPause;
    //    OnTimerResume -= CalculationPauseTime;
    //}

    #endregion

}

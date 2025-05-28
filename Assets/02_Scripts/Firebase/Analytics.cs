using UnityEngine;
using Firebase.Analytics;


static public class Analytics
{
    #region 애널리틱스 키

    public static bool hyperMergeTogle = false;

    //이벤트 이름

    // 재접속 횟수
    //public static readonly string RECONNECT_EVENT = "reconnect_event";

    // 게임 클리어
    public static readonly string EVENT_GAME_CLEAR = "game_clear";

    // 게임 오버
    public static readonly string EVENT_GAME_OVER = "game_over";

    // 미션 몬스터 처치
    public static readonly string EVENT_KILL_MISSION_MONSTER = "kill_mission_monster";

    // 평균 강화 수치
    public static readonly string EVENT_AVG_UPGRADE_LEVEL = "avg_upgrade_level";

    // 용 배치
    public static readonly string EVENT_PLACE_DRAGON = "place_dragon";

    //웨이브 시작
    public static readonly string EVENT_START_WAVE = "start_wave";

    //전리품 사용
    public static readonly string EVENT_USE_TICKET = "use_ticket";

    //2배속 사용
    public static readonly string EVENT_USE_SPEED_UP = "use_speed_up";

    //용 합성
    public static readonly string EVENT_MERGE_HYPER_DRAGON = "merge_hyper_dragon";
    #endregion

    #region 애널리틱스 파라미터
    //파라미터

    // 유저가 플레이 한 난이도 (하드/노말)
    public static readonly string PARAM_DIFFICULTY_TYPE = "difficulty_type";

    //유저의 누적 재접속 횟수
    //public static readonly string reconnect_count = "reconnect_count";

    //마지막 라운드 보스 처치 시
    //public static readonly string ParamR2ewardAmoun2t = "clear_round";

    //유저가 몇 라운드에 죽었는지
    public static readonly string PARAM_GAMEOVER_ROUND = "gameover_round";

    //1단계, 2단계, 3단계 미션 몬스터 구분
    public static readonly string PARAM_MONSTER_ID = "monster_id";

    //처치 시점의 라운드 정보
    public static readonly string PARAM_ROUND_NOMBER = "round_number";

    //유저가 평균적으로 몇 강까지 하는지
    public static readonly string PARAM_AVG_LEVEL = "avg_level";

    //어떤 용이 배치됐는지 (예: 0=불, 1=물, 2=땅, 3=풀, 4=빛)
    public static readonly string PARAM_DRAGON_TYPE = "dragon_type";

    //어느 위치에 배치했는지 (어디에 많이 배치하는지 분석 가능)
    public static readonly string PARAM_BOARD_POSITION_X = "board_position_x";
    public static readonly string PARAM_BOARD_POSITION_Z = "board_position_z";

    //현재 시작한 웨이브 번호 기록( 얼마나 플레이했는지 추척 가능)
    public static readonly string PARAM_WAVE_NUMBER = "wave_number";

    //전리품 종류
    public static readonly string PARAM_LOOT_TYPE = "loot_type";

    //전리품을 언제(몇 라운드) 사용했는지 기록
    public static readonly string PARAM_USE_ROUND_NUMBER = "use_round_number";

    //2배속을 켰는지 껐는지 상태 기록
    public static readonly string PARAM_IS_SPEED_UP = "is_speed_up";

    //하이퍼티어 드래곤이 언제(몇 라운드) 나오는지 기록
    public static readonly string PARAM_HYPER_ROUND_NUMBER = "hyper_round_number";

    #endregion

    // 애널리틱스 초기화 메서드
    static public void InitAnalyticsEvent()
    {
        hyperMergeTogle = false;
    }

    // 게임 종료시 실행되는 애널리틱스 이벤트 모음
    static public void EndAnalyticsEvent(bool isClear)
    {
        AvgUpgradeLevelEvent();

        if (isClear)
        {
            GameClearEvent();
        }
        else
        {
            GameOverEvent(StageManager.Instance.CurrentRound+1);
        }
    }


    /// <summary>
    /// 게임 클리어 시 실행할 애널리틱스 이벤트 
    /// </summary>
    static public void GameClearEvent()
    {
        FirebaseAnalytics.LogEvent(EVENT_GAME_CLEAR,
            PARAM_DIFFICULTY_TYPE, DataStore.difficultyType.ToString());
    }

    /// <summary>
    /// 게임 오버 시 실행할 애널리틱스 이벤트
    /// </summary>
    /// <param name="round">게임 오버된 라운드</param>
    static public void GameOverEvent(int round)
    {
        if (UserManager.Instance.Life > 0) return;

        Parameter[] parameters = new Parameter[]
        {
            new Parameter(PARAM_GAMEOVER_ROUND, round+1),
            new Parameter(PARAM_DIFFICULTY_TYPE, DataStore.difficultyType.ToString())
        };

        FirebaseAnalytics.LogEvent(EVENT_GAME_OVER, parameters);
    }

    /// <summary>
    /// 미션 몬스터 처치 시 실행할 애널리틱스 이벤트
    /// </summary>
    /// <param name="level">잡은 미션 몬스터의 단계</param>
    /// <param name="round">처치 시점의 라운드 정보</param>
    static public void KillMissionMonsterEvent(int level, int round)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter(PARAM_MONSTER_ID, $"mission{level+1}"),
            new Parameter(PARAM_ROUND_NOMBER, round + 1),
            new Parameter(PARAM_DIFFICULTY_TYPE, DataStore.difficultyType.ToString())
        };

        FirebaseAnalytics.LogEvent(EVENT_KILL_MISSION_MONSTER, parameters);
    }

    /// <summary>
    /// 유저가 게임을 끝냈을 때 평균 강화 레벨을 전송하는 애널리틱스 이벤트
    /// </summary>
    static public void AvgUpgradeLevelEvent()
    {
        int avg = GetAvgUpgradeLevel();

        Parameter[] parameters = new Parameter[]
        {
            new Parameter(PARAM_AVG_LEVEL, avg),
            new Parameter(PARAM_DIFFICULTY_TYPE, DataStore.difficultyType.ToString())
        };

        FirebaseAnalytics.LogEvent(EVENT_AVG_UPGRADE_LEVEL, parameters);
    }

    /// <summary>
    /// 평균 강화 레벨 계산 메서드 (private)
    /// </summary>
    private static int GetAvgUpgradeLevel()
    {
        int avg = 0;
        avg += TowerManager.Instance.TowerUpgrade[ElementType.Fire];

        for(int i=0; i< Define.ELEMENT_COUNT; i++)
        {
            avg += TowerManager.Instance.TowerUpgrade[(ElementType)i];
        }

        return avg / Define.ELEMENT_COUNT;
    }

    /// <summary>
    /// 플레이어가 드래곤(타워)을 배치했을 때 실행할 애널리틱스 이벤트
    /// </summary>
    /// <param name="type">어떤 속성의 용이 배치되었는지</param>
    /// <param name="vec">용이 배치된 위치</param>
    static public void PlaceDragonEvent(ElementType type, Vector3 vec)
    {
        // 벡터를 전송할 수 있는지를 모르겠어서 float 형으로 변환 후 전송
        // 전송 가능하다면 수정할 예정

        Parameter[] parameters = new Parameter[]
        {
            new Parameter(PARAM_DRAGON_TYPE, type.ToString()),
            new Parameter(PARAM_BOARD_POSITION_X, vec.x),
            new Parameter(PARAM_BOARD_POSITION_Z, vec.z),
            new Parameter(PARAM_DIFFICULTY_TYPE, DataStore.difficultyType.ToString())
        };

        FirebaseAnalytics.LogEvent(EVENT_PLACE_DRAGON, parameters);
    }

    /// <summary>
    /// 유저가 전리품을 사용할 때 실행할 애널리틱스 이벤트
    /// </summary>
    /// <param name="type">선택한 전리품 타입</param>
    /// <param name="round">전리품을 몇 라운드에 사용했는지 기록</param>
    static public void UseTicketEvent(TicketType type, int round)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter(PARAM_LOOT_TYPE, type.ToString()),
            new Parameter(PARAM_USE_ROUND_NUMBER, round+1),
            new Parameter(PARAM_DIFFICULTY_TYPE, DataStore.difficultyType.ToString())
        };

        FirebaseAnalytics.LogEvent(EVENT_USE_TICKET, parameters);
    }

    /// <summary>
    /// 하이퍼티어 드래곤이 합성으로 최초 등장했을 때 실행할 애널리틱스 이벤트
    /// </summary>
    /// <param name="round">최초 합성한 라운드</param>
    static public void MergeHyperDragon(int round)
    {
        // 하이퍼 드래곤 합성 이벤트가 이미 발생했으면 리턴
        if (hyperMergeTogle) return;

        Parameter[] parameters = new Parameter[]
        {
            new Parameter(PARAM_HYPER_ROUND_NUMBER, round+1),
            new Parameter(PARAM_DIFFICULTY_TYPE, DataStore.difficultyType.ToString())
        };

        // 하이퍼 드래곤 합성 이벤트 전송
        FirebaseAnalytics.LogEvent(EVENT_MERGE_HYPER_DRAGON, parameters);

        // 하이퍼 드래곤 합성 이벤트가 발생했으므로 토글을 true로 변경
        hyperMergeTogle = true;
    }
}


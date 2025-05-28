using System.Collections.Generic;
using UnityEngine;

public enum BaseUIType
{
    PlayerInfo,
    RoundUI,
    ToggleSpeed
}

public enum PopupUIType
{
    TowerInfo,      // 타워 정보 ( 타워 클릭
    SettingsUI,     // 설정 UI
    MissionUI,      // 미션 몬스터 UI
    LevelupUI,      // 강화 UI
    RewardUI,       // 보스보상 사용 UI
    GameOverUI,     // 게임 오버 UI
    RewardSelectTower,  // 타워 선택 UI ( 보스 보상 -> 타워 선택
    BuildUI,            // 타워 생성 UI ( 버튼 )
}

public enum MessageUIType
{
    WaveMessage,    // 웨이브 시작 알림
    BossMessage,    // 보스 웨이브 시작 알림
    ErrorMessage,   // 에러 메세지
    RewardMessage,  // 보상획득 알림
}

public enum EtcUIType
{
    Sellzone
}

public class UIManager : Singleton<UIManager>
{
    [Header("UI")]
    [SerializeField] private List<BaseUI> baseUIs;
    [SerializeField] private List<PopupUI> popupUIs;
    [SerializeField] private List<MessageUI> messageUIs;
    [SerializeField] private SellZone sellzoneUI;

    [Header("Button")]
    [SerializeField] private ButtonManager buttonManager;

    [Header("popup Background")]
    [SerializeField] private PopupBackground popupBackground;

    public List<BaseUI> BaseUIs => baseUIs;
    public List<PopupUI> PopupUIs => popupUIs;
    public List<MessageUI> MessageUIs => messageUIs;
    public SellZone SellZoneUI => sellzoneUI;
    public ButtonManager ButtonManager => buttonManager;
    public PopupBackground PopupBackground => popupBackground;

    protected override void Initialize()
    {
        // BaseUI 초기화
        foreach(var ui in baseUIs)
            ui.Initialize();

        // PopupUI 초기화
        foreach(var ui in PopupUIs)
            ui.Initialize();

        // MessageUI 초기화
        foreach(var ui in MessageUIs)
            ui.Initialize();        

        // button Manager 초기화
        buttonManager.Initialize();
    }

    private void Start()
    {
        // popupUI Start에서 초기화하는 경우
        foreach (var ui in PopupUIs)
            ui.StartInit();

        popupBackground.OnHideBackground();
    }

    public T GetMessageUI<T>(MessageUIType type) where T : MessageUI
    {        
        if(MessageUIs[(int)type] is T)
        {
            return (T)MessageUIs[(int)type];
        }

        return default(T);
    }
}

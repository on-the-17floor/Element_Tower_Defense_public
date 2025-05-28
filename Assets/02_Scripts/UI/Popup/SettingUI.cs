using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : PopupUI
{
    private RectTransform rectTransform;

    [Header("buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button lobbyButton;

    public override void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();

        // show 이벤트
        OnShowEvent += StopTimer;
        OnShowEvent += ShowAnimation;

        // hide 이벤트
        OnHideEvent += PlayTimer;

        // 버튼 이벤트
        resumeButton.onClick.AddListener(() => 
        { 
            UIManager.Instance.PopupBackground.OnHideBackground(); 
            Hide(); 
        });

        lobbyButton.onClick.AddListener(() =>
        {
            Hide();
            StageManager.Instance.GameOver();
        });
    }

    private void ShowAnimation()
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    private void StopTimer()
    {
        // 타이머 정지 및 상태 감지
        Time.timeScale = 0.0f;
        StageManager.Instance.OnTimerPause?.Invoke();
    }

    private void PlayTimer()
    {
        // 타이머 재개 및 상태 감지
        Time.timeScale = TimeScaleManager.Instance.GetTimeScale();
        StageManager.Instance.OnTimerResume?.Invoke();
    }
}

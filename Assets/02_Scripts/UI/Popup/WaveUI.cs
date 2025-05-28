using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class WaveUI : MessageUI
{
    [SerializeField] private TextMeshProUGUI waveText;
    private RectTransform rect;
    private float rectY;

    public override void Initialize()
    {
        rect = GetComponent<RectTransform>();
        rectY = rect.anchoredPosition.y;
    }

    public void OnMessage()
    {
        gameObject.SetActive(true); // 활성화

        StartCoroutine(WaveCountdown());

        rect.anchoredPosition = new Vector2(-Screen.width, rectY);
        rect.DOAnchorPos(new Vector2(0, rectY), 0.5f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject);
    }

    public void SetWaveLocalized(float sec)
    {
        string localized = LocalizationSettings.StringDatabase.GetLocalizedString("MyTable", "Wave");
        waveText.text = string.Format(localized, sec);
    }


    public IEnumerator WaveCountdown()
    {
        SetWaveLocalized(5);
        float timer = StageManager.Instance.RestTimer;

        while (StageManager.Instance.RestTimer > 0)
        {

            int currentTime = Mathf.CeilToInt(StageManager.Instance.RestTimer);
            // 값이 변했을 때만 UI 갱신
            if (currentTime != timer)
            {
                SetWaveLocalized(currentTime);
                timer = currentTime;
            }

            yield return null; // 매 프레임 검사
        }

        HideAnimation();
    }
    private void HideAnimation()
    {
        rect.DOAnchorPos(new Vector2(Screen.width, rectY), 0.5f)
            .SetEase(Ease.InCubic)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

}

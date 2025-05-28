using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public enum ErrorType
{
    SummonTokenError,
    UpgradTokenError,
    RewardTicketError
}

public class ErrorPopup : MessageUI
{
    [SerializeField] private TextMeshProUGUI errortext;
    private RectTransform rect;
    private float rectY;
    private bool isShowing;

    public override void Initialize()
    {
        rect = GetComponent<RectTransform>();
        rectY = rect.anchoredPosition.y;
        isShowing = false;
    }

    public void OnMessage(ErrorType type) 
    {
        if (isShowing)
            return;

        isShowing = true;

        gameObject.SetActive(true);

        // 로컬라이징
        string key = type.ToString();
        StartCoroutine(LoadLocalizedText(key));

        // Ui 애니메이션
        ShowAnimation();
    }


    private IEnumerator LoadLocalizedText(string key)
    {
        var handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("MyTable", key);

        yield return handle; // 로컬라이즈드 문자열 로드 대기

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            errortext.text = handle.Result;
        }
        else
            Debug.LogWarning($"Localization key '{key}' load failed.");
    }

    private void ShowAnimation()
    {
        SoundManager.Instance.SFXPlay(SFXType.Waring, Camera.main.transform.position);

        rect.anchoredPosition = new Vector2(-Screen.width, rectY);
        rect.DOAnchorPos(new Vector2(0, rectY), 1f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject)
            .OnComplete(() => StartCoroutine(HideDelay()));
    }

    private void HideAnimation()
    {
        rect.DOAnchorPos(new Vector2(Screen.width, rectY), 0.5f)
            .SetEase(Ease.InCubic)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                isShowing = false;
            });
    }

    private IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(1f);
        HideAnimation();
    }
}

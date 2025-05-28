using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public enum GameResult
{
    ResultVictory,
    ResultDefeat
}

public class GameOver : PopupUI
{
    private RectTransform rectTransform;

    [SerializeField] private TextMeshProUGUI showresult;
    [SerializeField] private TextMeshProUGUI round;
    [SerializeField] private TextMeshProUGUI time;

    [SerializeField] private Button exitbtn;

    public bool gameover;

    public override void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
        gameover = false;

        OnShowEvent += UIManager.Instance.PopupBackground.RemoveAllEvent;
        OnShowEvent += ShowAnimation;
        exitbtn.onClick.AddListener(OnExitClickAsync);
    }

    public void GameResult(GameResult result)
    {
        // 종료되었을때
        // GameResult 메서드 호출
        // Show 메서드 호출

        string key = result.ToString();
        StartCoroutine(LoadLocalizedText(key));

        round.text = (StageManager.Instance.CurrentRound + 1).ToString();
        time.text = StageManager.Instance.GetPlayTime();
    }

    private void ShowAnimation()
    {
        gameover = true;

        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject);
    }

    private async void OnExitClickAsync()
    {
        Time.timeScale = 1f;

        // 사운드 중지
        SoundManager.Instance.StopPlayingBGM();

        // 로비씬 비동기 이동
        await SceneLoader.Instance.ChangeSceneAsync(SceneState.Lobby);
    }

    private IEnumerator LoadLocalizedText(string key)
    {
        var handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("MyTable", key);

        yield return handle; // 로컬라이즈드 문자열 로드 대기

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            showresult.text = handle.Result;
        }
        else
        {
            Debug.LogWarning($"Localization key '{key}' load failed.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] private Button singleEasyMode;
    [SerializeField] private Button singleHardMode;

    private void Awake()
    {
        singleEasyMode.onClick.AddListener(() => OnSinglePlayButtonClick(DifficultyType.Normal));
        singleHardMode.onClick.AddListener(() => OnSinglePlayButtonClick(DifficultyType.Hard));
    }

    private async void OnSinglePlayButtonClick(DifficultyType diffType)
    {
        // SFX 실행 
        SoundManager.Instance.SFXPlay(SFXType.UiClick);

        // 사운드 중지
        SoundManager.Instance.StopPlayingBGM();

        // 씬 변경
        await SceneLoader.Instance.ChangeSceneAsync(SceneState.Main);

        // 현재 난이도 저장 
        DataStore.difficultyType = diffType;
    }
}

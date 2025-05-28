using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    [Header("===Ui 요소===")]
    [SerializeField] private Image loadingBarImage;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI lodingText;

    [Header("===로딩설정===")]
    [SerializeField] private float minLoadingTime = 3f;      // 최소 로딩 시간
    [SerializeField] private float extraLodingTime = 1f;    // 완료 후 추가 로딩 시간 
    [SerializeField] private float speed = 3f;               // lerp 속도

    private float targetFill = 0;       // 로딩바가 어디까지 fill 될지 목표 
    private bool isDataLoaded = false;  // 데이터 로드가 끝났는지 
    private bool isMinTimePassed = false;       // 데이터 로드 최소시간이 끝났는지 
    private bool isLoadingNextScene = false;    // 다음씬으로 넘어갔는지 

    private void Awake()
    {
        // 초기화 
        uiPanel.SetActive(true);
        loadingBarImage.fillAmount = 0;
        lodingText.text = "Preparing for data synchronization....";

        // DataSyncManger에 이벤트 등록
        DataSyncManager.Instance.OnLoadingProgressChange += UpdateLoadingProgress;
        DataSyncManager.Instance.OnDataLoadComplete += CompeleteDataLoad;

        StartCoroutine(CheckMinTime());
    }

    private void Update()
    {
        // 로딩바 업데이트
        loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, targetFill, speed * Time.deltaTime);

        // 데이터 로딩 완, 최소시간 지남, 로딩바가 0.95이상 찼을 때, 아직 씬 전환 안했을떄
        if (isDataLoaded && isMinTimePassed && loadingBarImage.fillAmount >= 0.95f && !isLoadingNextScene) 
        {
            isLoadingNextScene = true;

            // 씬 전환
            StartCoroutine(LoadingNextScene());
        }
    }

    // DataSyncManger의 OnLoadingProgressChange에 등록
    private void UpdateLoadingProgress(float time, string text) 
    {
        targetFill = time;
        lodingText.text = text;
    }

    // DataSyncMagner의 OnDataLoadComplete에 등록
    private void CompeleteDataLoad() 
    {
        // 이 함수가 호출된 시점에는 0.8만큼 진행되었음 (fill이 0.8)

        // 데이터 불러오기 완 
        isDataLoaded = true;
        lodingText.text = "Ready! Let's start the game...";

        // 0.2 나머지 채우기
        StartCoroutine(FillRamainProgress());
    }

    // 최소 시간 체크
    IEnumerator CheckMinTime()
    {
        yield return new WaitForSeconds(minLoadingTime);

        isMinTimePassed = true;

        yield break;
    }

    // 남는 0.2정도를 extraLodingTime 내에 꽉 채우기 
    IEnumerator FillRamainProgress() 
    {
        float startTime = Time.time; // 현재 진행 시간
        float progress = targetFill;        // 호출되는 시점엔 0.8

        while (true) 
        {
            if (progress >= 1f)
                break;

            float temp = (Time.time - startTime) / extraLodingTime;
            progress = 0.8f + (0.2f * temp);
            targetFill = Mathf.Clamp01((float)progress);

            yield return null;
        }
    }

    // 씬 전환
    IEnumerator LoadingNextScene() 
    {
        yield return new WaitForSeconds(0.5f);
        // 씬 전환 코드 
        SceneLoader.Instance.ChangeSceneAsync(SceneState.Lobby);

        yield break;
    }
}

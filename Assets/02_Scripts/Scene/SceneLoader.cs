using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// enum은 숫자로 시작 못함
public enum SceneState
{  
    Load,
    Lobby,
    Main,
}

public class SceneLoader : Singleton<SceneLoader>
{
    [Header("===Minimum loading time===")]
    const float waitTime = 1f;

    [Header("===Container===")]
    private Dictionary<SceneState, Action> loadSceneContainer;
    private Dictionary<SceneState, Action> unLoadSceneContainer;
    [SerializeField] private SceneState currSceneState;

    [SerializeField] private SceneLoadingPanel sceneLoadingPanel;

    protected override void Initialize()
    {
        // 실행은 1회 Data씬에서 하니까 따로 검사해서 Destory() 안해줘도 될듯? 
        DontDestroyOnLoad(gameObject);

        // 딕셔너리 초기화
        InitializedContainer();
    }

    private void InitializedContainer() 
    {
        loadSceneContainer = new Dictionary<SceneState, Action>()
        {
            { SceneState.Main , null },
            { SceneState.Lobby , null }
        };

        unLoadSceneContainer = new Dictionary<SceneState, Action>()
        {
            { SceneState.Main , null },
            { SceneState.Lobby , null }
        };
    }

    private void OnEnable()
    {
        // sceneLoaded는 awake -> sceneLoaded -> start 순서로 실행됨 
        SceneManager.sceneLoaded += OnSceneLoaded;

        // sceneUnloaded는 순서 ?? 
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    #region 씬 로드
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene 로드 / 현재 씬 이름 :" + scene.name + "\n / 현재 씬 state" + currSceneState);

        // 다음 씬 이벤트 시작
        if (loadSceneContainer.ContainsKey(currSceneState))
        {
            Debug.Log($"씬 이벤트 실행 : {currSceneState}");
            loadSceneContainer[currSceneState]?.Invoke();
        }
    }

    public void RegisterLoadAction(SceneState state, Action action)
    {
        if (loadSceneContainer.ContainsKey(state))
        {
            loadSceneContainer[state] += action;
        }
    }
    #endregion

    #region 씬 언로드

    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log("Scene 언 로드 / 현재 씬 이름 :" + scene.name + "\n / 현재 씬 state" + currSceneState);

        // 현재 씬 이벤트 실행 
        if (unLoadSceneContainer.ContainsKey(currSceneState))
        {
            Debug.Log($"씬 이벤트 실행 : {currSceneState}");
            unLoadSceneContainer[currSceneState]?.Invoke();
        }
    }
    public void RegisterUnLoadAction(SceneState state, Action action)
    {
        if (unLoadSceneContainer.ContainsKey(state))
        {
            unLoadSceneContainer[state] += action;
        }
    }
    #endregion

    // 씬 전환
    public async Task ChangeSceneAsync(SceneState nextState)
    {
        if (!loadSceneContainer.ContainsKey(nextState))
        {
            Debug.LogError($"No action exists in the dictionary for scene state {nextState}.");
            return;
        }

        // 씬 로드
        StartCoroutine(LoadSceneAsync(nextState));
    }

    IEnumerator LoadSceneAsync(SceneState nextState)
    {
        yield return new WaitForSeconds(0.02f);

        // 현재 씬 업데이트 
        currSceneState = nextState;

        // 씬이름
        string sceneName = GetName(currSceneState);
        if (sceneName == string.Empty)
        {
            Debug.LogError($"No scene corresponding to {sceneName}");
            yield break;
        }

        // sceneLoader 동작 시작
        sceneLoadingPanel.OnLoadingPanel();

        // 비동기 로드 
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 화면 즉시 전환 막기 
        asyncLoad.allowSceneActivation = false;

        // 일정 시간 보장 위한 
        float currTime = 0f;

        // 진행 0.9까지 대기 + 최소 로딩 시간 보장 
        while (true)
        {
            if (asyncLoad.progress >= 0.9f && currTime >= waitTime)
                break;

            currTime += Time.deltaTime;
            yield return null;
        }

        // 씬 전환 가능 
        asyncLoad.allowSceneActivation = true;

        // 완료될때까지 대기
        while (true) 
        {
            // allowSceneActivation를 true로 변경시켜 isDone이 true가 될 수 있음 
            if (asyncLoad.isDone)
                break;

            yield return null;
        }

        // 일정 시간 대기 (다음씬에서 비동기로 리소스 불러오는 시간 여유)
        yield return new WaitForSeconds(0.5f);

        // sceneLoader 동작 끝
        sceneLoadingPanel.EndLadingPanel();
    }

    private string GetName(SceneState state)
    {
        if (Define.SceneNames.TryGetValue(state, out string name)) 
        {
            return name;
        }
        return string.Empty;
    }


}

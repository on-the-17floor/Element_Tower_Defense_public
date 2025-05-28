using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

public enum BGMType 
{
    LobbyBGM,
    DungeonBGM,
    BossBGM
}

public class BGMManager : SoundBase<BGMType>
{
    /// <summary>
    /// 
    /// BGM Manager기능 
    /// 1. 배경음악 전환시 페이드인/아웃
    /// 2. 현재 BGM 중지
    /// 3. BGM 일시중지 + 재개 (구현 X)
    /// 
    /// </summary>

    private const float DEFAULT_FADE_TIME = 0.5f;
    private const bool DEFAULT_LOOP = true;

    [Space]
    [Header("===BGM Setting===")]
    [SerializeField] private AudioSource nowAudioSource;    // 현재 오디오소스
    [SerializeField] BGMType currBGMType;                   // 현재 bgm 타입

    // 페이드인 , 아웃 코루틴
    private Coroutine fadeInCorutine;
    private Coroutine fadeOutCorutine;

    void Awake()
    {
        resourceLabel = "BGM";
        initPoolSize = 3;
    }

    private void Start()
    {
        // SoundManager.OnInitialized 이벤트를 확인하여 초기화
        if (SoundManager.Instance.IsInitialized)
        {
            InitializeAsync();
        }
        else
        {
            SoundManager.Instance.OnInitialized += () => {
                InitializeAsync();
            };
        }

    }

    public override void InitializeAsync()
    {
        base.InitializeAsync();
    }

    // BGM 실행 
    public void PlayBGM(BGMType bgmType, float fadeTime = DEFAULT_FADE_TIME, bool loop = DEFAULT_LOOP) 
    {
        // 초기화전이면 return
        if (!isInitialized)
        {
            Debug.LogWarning($"Cannot play {GetType()}: {GetType()} not initialized");
            return;
        }

        // 0. 현재 실행하고 있는 사운드가 있으면 페이드아웃
        if (nowAudioSource != null && nowAudioSource.clip != null)
        {
            if (fadeOutCorutine != null)
                StopCoroutine(fadeOutCorutine);

            fadeOutCorutine = StartCoroutine(FadeOut(nowAudioSource, fadeTime));
        }

        // 1. 타입에 맞는 오디오 소스 가져오기
        nowAudioSource = base.GetAudioSourceByType(bgmType);
        if (nowAudioSource == null)
        {
            Debug.LogError($"Failed to Get Audio Source by type {bgmType}");
            return;
        }

        // 1-1. 세팅
        nowAudioSource.loop = loop;
        nowAudioSource.spatialBlend = 0f; // 2D 사운드

        // 2. 바꿀 bgm 페이드인
        if (fadeInCorutine != null)
        { 
            StopCoroutine(fadeInCorutine);
        }
        fadeInCorutine = StartCoroutine(FadeIn(nowAudioSource, fadeTime));

        // type 갱신
        currBGMType = bgmType;
    }

    IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        // 일정시간 대기 (전 브금이 완전히 꺼질때까지 대기)
        if (fadeTime > 0)
            yield return new WaitForSeconds(fadeTime * 0.5f);

        // 페이드인 
        // 소리 : 0부터 1까지
        audioSource.volume = 0;

        // 실행 후 fadeIn
        audioSource.Play();

        float startVolume = 0;
        float targetVolume = 1f;
        float currTime = 0;

        while (true) 
        {
            if (currTime >= fadeTime)
                break;

            // 진행시간 +
            currTime += Time.deltaTime;

            // 소리조정 
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currTime / fadeTime);

            // 한프레임 대기 
            yield return null;
        }

        // 정확한 볼륨으로 설정
        audioSource.volume = targetVolume;
        // 코루틴이 끝나면 null
        fadeInCorutine = null;
    }

    IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        // 실행하고 있는 오디오가 없으면
        if (!audioSource.isPlaying)
            yield break;

        // 페이드아웃
        // 소리 : 1부터 0까지 
        float startVolume = audioSource.volume;
        float targetVolume = 0f;
        float currTime = 0;

        while (true)
        {
            if (currTime >= fadeTime)
                break;

            // 진행시간 +
            currTime += Time.deltaTime;

            // 소리조정 
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currTime / fadeTime);

            // 한프레임 대기 
            yield return null;
        }

        // 페이드 아웃이 완료되면 오디오 재생을 중지
        audioSource.Stop();

        // 코루틴이 끝나면 null
        fadeOutCorutine = null;
    }

    // 재생중인 BGM 종료
    public void StopBGM(float fadeTime = DEFAULT_FADE_TIME) 
    {
        // 현재 실행하고 있는 사운드가 있으면 페이드아웃
        if (nowAudioSource != null && nowAudioSource.clip != null)
        {
            Debug.Log($"Stop the BGM currently running : {currBGMType}");

            if (fadeOutCorutine != null)
                StopCoroutine(fadeOutCorutine);

            fadeOutCorutine = StartCoroutine(FadeOut(nowAudioSource, fadeTime));

            nowAudioSource = null;
        }
    }
}

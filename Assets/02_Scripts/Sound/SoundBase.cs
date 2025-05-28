using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// SFXManager와 BGMManager의 공통 기능을 제공하는 기본 클래스
/// </summary>
/// <typeparam name="T">사운드 타입 enum </typeparam>
public abstract class SoundBase<T> : MonoBehaviour
    where T : Enum
{
    /// <summary>
    /// 
    /// SFXManager와 BGMManager의 공통된 기능 
    /// 1. 오디오 소스 할당할 Trs 생성
    /// 2. 믹서그룹 찾기
    /// 3. 오디오 클립 동적로드
    /// 4. 
    /// 
    /// </summary>

    [Header("===Config===")]
    [SerializeField] protected int initPoolSize;
    [SerializeField] protected string resourceLabel;    // 초기화 label 

    [Header("===Component===")]
    protected SoundManager soundManager;
    [SerializeField] protected Transform sourceTransform;      // SFX , BGM을 실행시킬 Tranform
    [SerializeField] protected AudioMixerGroup mixerGroup;     // 믹서의 어느 그룹인지 

    [Header("===Conianer===")]
    private Dictionary<T, AudioSource> typeBySource;

    // 초기화 완료 여부 
    protected bool isInitialized = false;
    public bool IsInitialized => isInitialized;

    // 초기화
    public virtual async void InitializeAsync() 
    {
        Debug.Log($"Initalizing {GetType().Name}");

        // SoundManager가 초기화될 때까지 대기
        if (!SoundManager.Instance.IsInitialized)
        {
            await WaitForSoundManagerInitialization();
        }

        // 사운드manager
        soundManager = GetComponent<SoundManager>();
        if (soundManager == null)
        {
            Debug.Log($"Failed to find SoundManger for {GetType().Name}");
        }

        // 1. AudioSource 담아놓을 오브젝트 생성
        if (sourceTransform == null)
        {
            Transform trs = new GameObject(GetType().Name).GetComponent<Transform>();

            // 사운드 Manger의 PoolPanrent를 부모로 
            trs.InitTransform(soundManager.PoolParent);
            sourceTransform = trs.transform;
        }

        // 2. 믹서 그룹 찾기
        AudioMixerGroup mixerGroup = await InitalizeMixerGroup(resourceLabel);
        if (mixerGroup == null)
        {
            Debug.LogError($"Could not find mixer group : {GetType().Name}");
            return;
        }

        // 3. 오디오 클립 로드 
        List<AudioClip> dynamicLoadAudioClip = await LoadAudioClips(resourceLabel);
        if (dynamicLoadAudioClip == null && dynamicLoadAudioClip.Count < 0)
        {
            Debug.Log($"{GetType().Name} Failed to Dynamic Load Audio Clip List");
            return;
        }

        // 4. 타입별 AudioSource 딕셔너리 초기화
        await InitTypeByClipDict(dynamicLoadAudioClip , mixerGroup);

        // 초기화 끝 
        isInitialized = true;
    }

    // SoundManager 초기화 대기를 위한 유틸리티 메서드
    private async Task WaitForSoundManagerInitialization()
    {
        while (!SoundManager.Instance.IsInitialized)
        {
            await Task.Yield();
        }
    }

    protected async Task<List<AudioClip>> LoadAudioClips(string label) 
    {
        // 비동기 : label에 해당하는 오디오클립 가져오기 
        List<AudioClip> tempClipList = await ResourceLoader.AsyncLoadLable<AudioClip>(label);

        if (tempClipList == null || tempClipList.Count < 0) 
        {
            Debug.Log($"Failed to load {label} audio clips with label: {label}");
            return null;
        }

        return tempClipList;
    }

    // 타입별 오디오 소스 딕셔너리 초기화
    protected async Task InitTypeByClipDict(List<AudioClip> tempClipList, AudioMixerGroup mixerGroup)
    {
        // 타입별 오디오소스
        typeBySource = new Dictionary<T, AudioSource>();

        for (int i = 0; i < tempClipList.Count; i++)
        {
            // 파일 이름 -> Enum 값 변환 
            string clipName = tempClipList[i].name;
            T type = Extension.StringToEnum<T>(clipName);

            // 오디오 소스 생성 (클립, 믹서그룹 추가)
            var insAudioSource = CreateAudioSource(tempClipList[i], mixerGroup);

            // 딕셔너리에 추가
            typeBySource.Add(type , insAudioSource);
        }

    }

    // 오디오 믹서 할당 
    protected async Task<AudioMixerGroup> InitalizeMixerGroup(string label)
    {
        if (soundManager.Mixer == null)
        {
            Debug.LogError("AudioMiser is null in SoundManager");
            return null;
        }

        // mixer에서 group 찾기
        AudioMixerGroup[] groups = soundManager.Mixer.FindMatchingGroups(label);
        if (groups.Length > 0)
        {
            mixerGroup = groups[0];
            return mixerGroup;
        }

        return null;
    }

    // 새 오디오 소스 생성후 리턴
    protected AudioSource CreateAudioSource(AudioClip audioClip, AudioMixerGroup mixerGroup)
    {
        // 1. 컴포넌트 생성
        var audioSourceInstnace = sourceTransform.AddComponent<AudioSource>();
        // 2. 믹서 그룹 설정 
        audioSourceInstnace.outputAudioMixerGroup = mixerGroup;
        // 3. 오디오 클립 할당
        audioSourceInstnace.clip = audioClip;
        // 4. play on awake 끄기
        audioSourceInstnace.playOnAwake = false;

        return audioSourceInstnace;
    }

    // 타입별 오디오 소스 리턴
    public AudioSource GetAudioSourceByType(T type) 
    {
        if (typeBySource.TryGetValue(type, out AudioSource source))
            return source;

        return null;
    }
}

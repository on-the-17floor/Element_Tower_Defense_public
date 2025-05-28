
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Input = Modoium.Service.Input;

public enum SoundType 
{
    BGM,
    SFX
}

public class SoundManager : MonoBehaviour
{
    /// <summary>
    /// SFXManager와 BGMManger에 접근하기 위한
    /// </summary>

    [Header("===Sound Volume===")]
    [SerializeField] private float bgmVolume;
    [SerializeField] private float sfxVolume; 

    [Header("===Transform===")]
    [SerializeField] private Transform soundParent;  // pool Transform 부모 

    [Header("===Component===")]
    [SerializeField] private SFXManager sfxManager;
    [SerializeField] private BGMManager bgmManager;

    [Header("===Mixer===")]
    [SerializeField] private AudioMixer mixer;
    public event System.Action OnInitialized;
    private bool isInitialized = false;

    public bool IsInitialized => isInitialized;

    public bool IsMixerLoaded => mixer != null;

    public Transform PoolParent { get => soundParent;}
    public SFXManager SfxManager { get => sfxManager; }
    public BGMManager BgmManager { get => bgmManager; }
    public AudioMixer Mixer { get => mixer; }
    public float BgmVolume { get => bgmVolume;  }
    public float SFXVolume { get => sfxVolume; }

    #region 싱글톤
    
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();

                if (instance == null)
                {
                    GameObject newManager = new GameObject(typeof(SoundManager).Name, typeof(SoundManager));
                    instance = newManager.GetComponent<SoundManager>();
                }
            }

            return instance;
        }
    }
    #endregion

    public async void Awake()
    {
        #region DontDestory
        // 싱글톤 DontDestory()
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        #endregion

        // 믹서 가져오기
        List<AudioMixer> tempmixer = await ResourceLoader.AsyncLoadLable<AudioMixer>("AudioMixer");
        
        if(tempmixer != null && tempmixer.Count >= 0) 
        {
            mixer = tempmixer[0];
        }  

        // 볼륨 초기화
        bgmVolume = PlayerPrefs.GetFloat(SoundType.BGM.ToString(), 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(SoundType.SFX.ToString(), 0.5f);

        await InitializeAsync();

        // 초기화 완료 표시 및 이벤트 발생
        isInitialized = true;
        OnInitialized?.Invoke();
        Debug.Log("SoundManager 초기화 완료");

    }

    private void Start()
    {
        // BGM 실행
        AsyncPlayBGM();
    }

    #region 초기화

    private async Task InitializeAsync() 
    {
        Debug.Log($"Initalizing {GetType().Name}");

        // 사운드 저장할 trs 생성 
        if (soundParent == null)
        {
            soundParent = this.transform;
        }

        // 컴포넌트 초기화
        InitComponent();

        // 0.1f 대기
        await Task.Delay(100);

        // 씬 이벤트 등록 
        RegisterSceneSoundPlayer();   
    }

    async Task AsyncPlayBGM()
    {
        // WaitUntil과 유사한 기능 구현
        while ( !isInitialized || !bgmManager.IsInitialized)
        {
            await Task.Yield(); // 또는 await Task.Delay(10);
        }

        // 조건이 만족된 후 실행할 코드
        Debug.Log("BGM Manager is now initialized");
        BGMPlay(BGMType.LobbyBGM);
    }

    private void InitComponent() 
    {
        if (sfxManager == null)
        {
            // SFXManager 가져오기
            if (TryGetComponent(out SFXManager sfx))
                sfxManager = sfx;
            else
                sfxManager = this.AddComponent<SFXManager>();
        }

        if (bgmManager == null) 
        {
            // BGMManager 가져오기 
            if (TryGetComponent(out BGMManager bgm))
                bgmManager = bgm;
            else
                bgmManager = this.AddComponent<BGMManager>();
        }
    }

    private void RegisterSceneSoundPlayer() 
    {
        Debug.Log($"Register Scene to SoundPlayer : {GetType().Name}");

        // 로비 씬 로드 시 로비 BGM 재생
        SceneLoader.Instance.RegisterLoadAction(SceneState.Lobby, () => BGMPlay(BGMType.LobbyBGM));

        // 메인 씬 로드 시 던전 BGM 재생
        SceneLoader.Instance.RegisterLoadAction(SceneState.Main, () => BGMPlay(BGMType.LobbyBGM));

        // 로비 씬 언로드 시 BGM 중지
        // SceneLoader.Instance.RegisterUnLoadAction(SceneState.Lobby, () => bgmManager.StopBGM());

        // 메인 씬 언로드 시 BGM 중지
        // SceneLoader.Instance.RegisterUnLoadAction(SceneState.Main, () => bgmManager.StopBGM());
    }

    #endregion

    public void SaveAudioVolume(SoundType type , float value) 
    {
        switch (type) 
        {
            case SoundType.BGM:
                bgmVolume = value; break;
            case SoundType.SFX: 
                sfxVolume = value; break;
        }

        PlayerPrefs.SetFloat(type.ToString(), value);
        PlayerPrefs.Save();
    }

    #region 사운드 실행 

    // 다른 스크립트에서 SFX 실행
    public void SFXPlay(SFXType sfxType, Vector3 ownerPosition) 
    {
        sfxManager.PlaySFX(sfxType ,  ownerPosition);
    }

    public void SFXPlay(SFXType sfxType) 
    {
        sfxManager.PlaySFX(sfxType);
    }

    // 다른 스크립트에서 BGM 실행
    public void BGMPlay(BGMType bgmType , float fadeTime = 0.5f) 
    {
        Debug.Log($"Play the background music : {bgmType}");

        bgmManager.PlayBGM(bgmType, fadeTime);
    }

    // 다른스크립트에서 BGM 중지
    public void StopPlayingBGM() 
    {
        bgmManager.StopBGM(0);
    }
    #endregion
}

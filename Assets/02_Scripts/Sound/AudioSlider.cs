using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// 오디오 볼륨 조절하는 슬라이더 컨트롤러
/// </summary>
public class AudioSlider : MonoBehaviour
{
    [Tooltip("오디오 믹서 참조")]
    public AudioMixer audioMixer;

    [Tooltip("BGM 볼륨 슬라이더")]
    public Slider BGMSlier;
    [Tooltip("SFX 볼륨 슬라이더")]
    public Slider SFXSlider;

    [Header("===Transform===")]
    public GameObject canvas;
    public Transform soundParentTrs;
    public Transform bgmVolumeTrs;
    public Transform sfxVolumeTrs;

    [Space]
    public Transform[] allChildren;

    // 오디오 볼륨 
    private const float MIN_VOLUME = -80f;
    private const float MAX_VOLUME = -10f;
    private const float MIN_AUDIBLE_VALUE = 0.00001f;

    // 오디오믹서 파라미터 이름
    private const string BGM_VOLUME_PARAMETER = "BGM";
    private const string SFX_VOLUME_PARAMETER = "SFX";

    // 오디오 오브젝트 이름
    private const string SOUND_OBJECT_PAREMETER = "Sound";
    private const string BGM_OBJECT_PAREMETER = "BGM Volume";
    private const string SFX_OBJECT_PAREMETER = "SFX Volume";

    void Start()
    {
        // 컴포넌트 찾기 
        FindCanvasAndComponent();

        // SoundManager.OnInitialized 이벤트를 확인하여 초기화
        if (SoundManager.Instance.IsInitialized)
        {
            InitAudioMixer();
            SetUpSlider();
            ApplyInitVolume();
        }
        else
        {
            SoundManager.Instance.OnInitialized += () => {
                InitAudioMixer();
                SetUpSlider();
                ApplyInitVolume();
            };
        }

        // 씬에 할당 
        RegisterSceneSoundPlayer();
    }

    private void RegisterSceneSoundPlayer() 
    {
        // 1. 컴포넌트 초기화
        // 2. 슬라이더 초기화 

        // 로비 씬 로드 시
        SceneLoader.Instance.RegisterLoadAction(SceneState.Lobby, async () => 
        {
            InitAudioMixer();
            ApplyInitVolume();
            await FindCanvasAndComponent();
            SetUpSlider();
        } );

        // 메인 씬 로드 시 
        SceneLoader.Instance.RegisterLoadAction(SceneState.Main, async () =>
        {
            InitAudioMixer();
            ApplyInitVolume();
            await FindCanvasAndComponent();
            SetUpSlider();
        });
    }

    #region 컴포넌트 찾기
    async Task FindCanvasAndComponent()
    {
        // 사운드 부모 Trs 찾기
        if (soundParentTrs == null)
        {
            while (true)
            {
                // 켜져있는 캔버스 찾기 
                canvas = GameObject.Find("Canvas");

                if (canvas != null)
                    break;

                await Task.Yield();
            }
        }

        await Task.Yield();

        if (canvas != null)
        {
            // 캔버스 하위에서 비활성화된 오브젝트도 찾을 수 있음
            soundParentTrs = FindChildByName(canvas.transform, SOUND_OBJECT_PAREMETER);
        }

        if (soundParentTrs == null) 
        {
            Debug.LogError("Canvas Under soundParentTrs is NUll");
            return;
        }

        FindSoundComponent();
    }


    
    private void FindSoundComponent()
    {
        // soundParentTrs가 null이면 더 이상 진행하지 않음
        if (soundParentTrs == null)
        {
            Debug.LogWarning("AudioSlider: Sound parent transform not found. Retrying...");
            return;
        }

        // bgm Trs 찾기
        if (bgmVolumeTrs == null)
        {
            Transform temp = FindChildByName(soundParentTrs, BGM_OBJECT_PAREMETER).transform;
            if (temp != null)
                bgmVolumeTrs = temp;
        }
        // sfxTrs 찾기
        if (sfxVolumeTrs == null)
        {
            Transform temp = FindChildByName(soundParentTrs, SFX_OBJECT_PAREMETER).transform;
            if (temp != null)
                sfxVolumeTrs = temp;
        }

        // bgm 슬라이더 찾기
        if (BGMSlier == null && bgmVolumeTrs != null)
        {
            BGMSlier = bgmVolumeTrs.GetComponentInChildren<Slider>();
        }
        // sfx 슬라이더 찾기
        if (SFXSlider == null && sfxVolumeTrs != null)
        {
            SFXSlider = sfxVolumeTrs.GetComponentInChildren<Slider>();
        }

    }

    private Transform FindChildByName(Transform parent, string name)
    {
        if (parent == null)
            return null;

        // parent GameObject가 비활성화되어 있으면 GetComponentsInChildren이 작동하지 않음
        if (!parent.gameObject.activeInHierarchy && !parent.gameObject.activeSelf)
        {
            Debug.LogWarning($"Parent '{parent.name}' is inactive. Cannot search inactive children.");
            return null;
        }

        // true 파라미터: 비활성화된 자식 오브젝트도 모두 포함하여 검색
        // 모든 하위 계층(자식, 손자, 증손자...)의 Transform을 가져옴
        allChildren = parent.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            if (child.name == name)
            {
                return child;
            }
        }

        Debug.LogWarning($"'{name}' not found under '{parent.name}'");
        return null;
    }
    #endregion

    #region 1회 초기화
    private void InitAudioMixer() 
    {
        if (audioMixer == null) 
        {
            audioMixer = SoundManager.Instance.Mixer;

            if (audioMixer == null)
            { 
                Debug.LogError("AudioSlider.cs : AudioMixer is missing");
                return;
            }
        }
    }
    #endregion

    #region 씬 넘어갈 때 마다 초기화

    private void SetUpSlider() 
    {
        if (BGMSlier != null)
        {
            BGMSlier.onValueChanged.AddListener(SetBGMVolume);
            BGMSlier.value = SoundManager.Instance.BgmVolume;
        }
        else 
        {
            Debug.Log("BGMSlider is NULL");
        }

        if (SFXSlider != null) 
        {
            SFXSlider.onValueChanged.AddListener(SetSFXVolume);
            SFXSlider.value = SoundManager.Instance.SFXVolume;
        }
        else
        {
            Debug.Log("SFXSlider is NULL");
        }
    }

    private void ApplyInitVolume() 
    {
        SetBGMVolume(SoundManager.Instance.BgmVolume);
        SetSFXVolume(SoundManager.Instance.SFXVolume);
    }

    #endregion

    #region 사운드 설정 

    public void SetBGMVolume(float volume)
    {
        // 오디오믹서 소리 설정 
        audioMixer.SetFloat(BGM_VOLUME_PARAMETER, ConvertToDecibel(volume));

        // 볼륨 값 저장 
        SoundManager.Instance.SaveAudioVolume(SoundType.BGM, volume);
    }

    public void SetSFXVolume(float volume)
    {
        // 오디오믹서 소리 설정 
        audioMixer.SetFloat(SFX_VOLUME_PARAMETER, ConvertToDecibel(volume));

        // 볼륨 값 저장 
        SoundManager.Instance.SaveAudioVolume(SoundType.SFX, volume);
    }

    #endregion

    /// <summary>
    /// 0 - 1 사이 볼륨 값을 데시벨로 변환
    /// </summary>
    /// <param name="volume"> 0 - 1 사이의 정규화 된 볼륨 값</param>
    /// <returns>데시벨 값</returns>
    private float ConvertToDecibel(float volume) 
    {
        // 볼륨이 0이면 음소거 (-80dB)
        if (volume <= 0f)
        {
            return MIN_VOLUME;  //  음소거
        }

        // 최소값 설정 
        float safeVolume = Mathf.Max(MIN_AUDIBLE_VALUE, volume);

        // 로그로 변환 
        return Mathf.Log10(safeVolume) * 20;
    }
}

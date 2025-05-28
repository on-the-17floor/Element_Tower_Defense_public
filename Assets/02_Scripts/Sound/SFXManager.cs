using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public enum SFXType 
{
    // 재화
    AddEnhanceToken,    // 강화석 지급
    AddSummonToken,     // 소환석 지급

    // 타워
    TowerCreate,        // 타워 생성
    TowerHyperAttack,   // 타워 하이퍼 공격
    TowerRedAttack,     // 타워 red 공격
    TowerWhiteAttack,   // 타워 화이트 공격

    // UI
    TowerSale,      // 타워 판매
    UiClick,        // UI 클릭
    Waring,          // 경고

    // 몬스터 
    MonsterDie
}

public class SFXManager : SoundBase<SFXType>
{
    /// <summary>
    /// 
    /// SFX Manager기능 
    /// 1. 재생중인 목록에 사운드 추가 
    /// 2. 재생중인 목록에 사운드 제거
    /// 
    /// </summary>

    private const float SFX_RETURN_DELAY = 0.15f;

    [Space]
    [Header("===Camera===")]
    [SerializeField] private Camera mainCamera;

    void Awake()
    {
        resourceLabel = "SFX";
        initPoolSize = 10;
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

        // 씬 레지스터에 추가
        SceneLoader.Instance.RegisterLoadAction(SceneState.Main, FindMainCamera);
        SceneLoader.Instance.RegisterLoadAction(SceneState.Lobby, FindMainCamera);
    }

    public override void InitializeAsync()
    {
        base.InitializeAsync();

        // 카메라 찾기
        FindMainCamera();
    }

    private void FindMainCamera() 
    {
        // 카메라 찾기
        if (mainCamera == null)
        {
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            if (mainCamera == null)
            {
                Debug.LogError("Main camera not found");
            }
        }
    }

    // SFX플레이 : UI 전용 
    public void PlaySFX(SFXType sfxType) 
    {
        // 초기화전이면 return
        if (!isInitialized)
        {
            Debug.LogWarning($"Cannot play {GetType()}: {GetType()} not initialized");
            return;
        }

        // 1. 타입에 해당하는 오디오소스 받기
        AudioSource source = base.GetAudioSourceByType(sfxType);
        if (source == null)
        {
            Debug.LogError($"Failed to Get Audio Source by type {sfxType}");
            return;
        }

        // 3. 실행 
        source.Play();
    }

    // SFX플레이 : 오브젝트 전용 
    public void PlaySFX(SFXType sfxType, Vector3 ownerPosition )
    {
        // 초기화전이면 return
        if (!isInitialized)
        {
            Debug.LogWarning($"Cannot play {GetType()}: {GetType()} not initialized");
            return;
        }

        // 0. 카메라 범위 밖에 있으면 실행 x 
        if (!IsInCameraRange(ownerPosition))
        {
            Debug.LogWarning($"{GetType()}: {sfxType} Clip Position out of camera range");
            return;
        }

        // 1. 타입에 해당하는 오디오소스 받기
        AudioSource source = base.GetAudioSourceByType(sfxType);
        if (source == null) 
        {
            Debug.LogError($"Failed to Get Audio Source by type {sfxType}");
            return;
        }

        // 2. 실행 검사 
        if (source.isPlaying)
        {
            // 실행된지 일정시간 이하이면 return
            if (source.time <= SFX_RETURN_DELAY)
                return;

            // 일정시간 넘었으면 stop
            source.Stop();
        }

        // 3. 실행 
        source.Play();
    }

    // 카메라 범위 안에 있는지 
    private bool IsInCameraRange(Vector3 position)
    {
        if (mainCamera == null)
            return false;

        // 좌측하단 0,0 우측상단 1,1
        Vector3 viewPos = mainCamera.WorldToViewportPoint(position);

        if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)
            return false;

        return true;
    }

}

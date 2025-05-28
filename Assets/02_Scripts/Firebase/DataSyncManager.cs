using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;
using Firebase.Analytics;


/// <summary>
/// 서버에서 받아오는 데이터들은 클래스에 해당 인터페이스를 구현해야 함
/// </summary>
public interface ILoadableFromSnapshot
{
    void LoadFromSnapshot(DataSnapshot snap);
}


public class DataSyncManager : Singleton<DataSyncManager>
{
    private bool isValidated;        // 버전 유효성 검사 결과

    private AuthService authService = new();              // 로그인 관리
    private VersionTracker versionChecker = new();        // 버전 체크
    private DataPatcher dataPatcher = new();              // 데이터 패치

    public event Action OnDataLoadComplete;               // 데이터가 끝났을 때 호출
    public Action<float, string> OnLoadingProgressChange; // 진행률, 작업메시지

    protected override void Initialize()
    {
        Debug.Log("DataSyncManager 초기화");

        InitFirebase();
    }

    private async void InitFirebase()
    {
        var task = FirebaseApp.CheckAndFixDependenciesAsync();

        await task.ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // 로컬 캐시 비활성화
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);

                // Analytics 데이터 수집 시작
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                Debug.Log("Firebase 초기화 완료");
            }
            else
            {
                Debug.LogError("Firebase 초기화 실패: " + task.Result);
            }
        });

        // 버전 체크
        isValidated = await versionChecker.CheckVersion();

        //결과에 따라 로컬 데이터를 불러오거나 서버 데이터를 받아옴
        if (isValidated)
        {
            Debug.Log("로컬 버전과 서버 버전이 일치합니다.");
            await dataPatcher.LoadGameData();   // 로컬 데이터 불러오기
        }
        else
        {
            Debug.Log("로컬 버전과 서버 버전이 일치하지 않습니다. 데이터 동기화 필요.");

            await dataPatcher.UpdateGameData(); // 서버 데이터로 업데이트
            dataPatcher.SaveGameData();         // 로컬에 저장

            versionChecker.UpdateLocalVersion(); // 로컬 버전 갱신
        }

        // 로그인
        authService.Login();

        // 씬 이동
        EnterGame();
    }

    // 씬 이동 메서드
    private void EnterGame()
    {
        // 데이터 완료 액션 실행
        OnDataLoadComplete?.Invoke();
    }

}


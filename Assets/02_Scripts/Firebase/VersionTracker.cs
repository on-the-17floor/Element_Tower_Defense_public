using Firebase.Database;
using System.Threading.Tasks;

using UnityEngine;

public class VersionTracker
{
    private string localVersion;     // 로컬 JSON 버전
    private string serverVersion;    // 서버 JSON 버전

    public async Task<bool> CheckVersion()
    {
        GetLocalVersion();
        await GetServerVersion();

        return serverVersion == localVersion;
    }

    // 기기의 버전을 가져오는 메서드
    private void GetLocalVersion()
    {
        localVersion = SaveLoader.Load<string>("version")?.ToString();
        Debug.Log($"로컬 버전: {localVersion}");
    }

    // 서버의 버전을 가져오는 메서드
    private async Task GetServerVersion()
    {
        DataSnapshot snap = await FirebaseCRUD.SearchData("version");
        serverVersion = FirebaseCRUD.GetData(snap);
    }

    // 기기의 버전을 갱신하는 메서드
    public void UpdateLocalVersion()
    {
        SaveLoader.Save(serverVersion, "version");
        Debug.Log($"로컬 버전 갱신 완료: {serverVersion}");
    }

}
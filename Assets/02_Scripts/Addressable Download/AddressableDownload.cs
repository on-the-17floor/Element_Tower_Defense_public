using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public enum AddressableLabels
{
    Prefab,
    TowerPrefab,
    TowerEffect,
    ProjectilePrefab,
    HitEffect,
    MuzzleEffect,
    NormalEffect,
    BGM,
    SFX,
    Monster,
    Monster_Mission
}

public class AddressableDownload : MonoBehaviour
{
    /* 어드레서블 에셋번들 업데이트
     * 1. Update a Previous Build를 통해 에셋번들 업데이트 하기
     * 2. 업데이트된 json, hash 파일과 bundle파일을 서버에 다시 올리기
     */
    [Header("UI")]
    [SerializeField] private DownloadScene downloadSceneUI;

    [Header("Download UI")]
    [SerializeField] private Button downloadButton;
    [SerializeField] private Button cancelButton;

    [Header("Addressable Data 확인용")]
    [SerializeField] private List<string> addressableKeys;
    [SerializeField] private List<string> catalogToUpdate;

    async void Start()
    {
        downloadButton.onClick.AddListener(async () => await DownloadAsset());
        cancelButton.onClick.AddListener(() => Application.Quit());
        //Caching.ClearCache();

        // 에셋 다운을 위한 키값(Label) 초기화
        addressableKeys = new List<string>();
        foreach (var label in Enum.GetValues(typeof(AddressableLabels)))
        {
            addressableKeys.Add(label.ToString());
            //Addressables.ClearDependencyCacheAsync(label.ToString());
        }
        // 에셋 다운
        await DownloadAddressable();
    }

    private async Task DownloadAddressable()
    {
        try
        {
            //Addressable 초기화
            var initHandle = Addressables.InitializeAsync();
            await initHandle.Task;

            // 카탈로그 업데이트 확인
            await UpdateCatalog();

            // 다운로드 용량 확인
            long downSize = await GetDownloadSize();
            Debug.Log($"다운로드 필요: {downSize / (1024f * 1024f):F2} MB");

            if (downSize > 0)
            {
                downloadSceneUI.UpdateProgressText("need to download");
                downloadSceneUI.SetDownloadSize(downSize);
                downloadSceneUI.OnDownloadUI(true);
            }
            else
            {
                downloadSceneUI.UpdateProgressText("download complete");
                downloadSceneUI.OnStartButton();
                Debug.Log("받을 에셋 없음. ( 최신버전 )");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Addressable 초기화중 예외 발생 {ex}\n 예외 발생");
        }
    }

    private async Task UpdateCatalog()
    {
        try
        {
            // 카탈로그 업데이트
            catalogToUpdate = new List<string>();
            var checkForUpdateHandle = Addressables.CheckForCatalogUpdates();
            checkForUpdateHandle.Completed += result => { catalogToUpdate.AddRange(result.Result); };
            await checkForUpdateHandle.Task;

            if (catalogToUpdate.Count > 0)
            {
                Debug.Log("업데이트 필요함.");
                var updateHandle = Addressables.UpdateCatalogs(catalogToUpdate);
                await updateHandle.Task;
            }
            else
            {
                Debug.Log("업데이트 필요없음");
            }

            Addressables.Release(checkForUpdateHandle);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Addressable Catalog Update Exception {ex}\n카탈로그 업데이트중 예외 발생");
        }
    }

    private async Task<long> GetDownloadSize()
    {
        try
        {
            long downSize = 0;
            foreach (var key in addressableKeys)
            {
                var sizeHandle = Addressables.GetDownloadSizeAsync(key);
                await sizeHandle.Task;
                long size = sizeHandle.Result;
                downSize += size;

                Addressables.Release(sizeHandle);
            }
            return downSize;
        }

        catch (Exception ex)
        {
            Debug.LogError($"Addressable DownloadSize Exception {ex}\n다운로드 사이즈 반환중 예외 발생");
            return 0;
        }
    }

    private async Task DownloadAsset()
    {
        // addressableKeys키값의 모든 의존 에셋 번들을 비동기로 다운로드
        // 이미 로컬에 있는 에셋은 재다운로드 하지 않고 무시함.
        // MergeMode.Union 중복되는 의존성 에셋은 한 번만 다운로드
        try
        {
            downloadSceneUI.OnDownloadUI(false);
            var downHandle = Addressables.DownloadDependenciesAsync(addressableKeys, Addressables.MergeMode.Union);

            downHandle.Completed += (AsyncOperationHandle handle) =>
            {
                downloadSceneUI.UpdateProgressText("download complete");
                downloadSceneUI.OnStartButton();
                Debug.Log("에셋 다운로드 완료");

                Addressables.Release(downHandle);
            };

            while (!downHandle.IsDone)
            {
                downloadSceneUI.UpdateProgress(downHandle.PercentComplete);
                await Task.Yield();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Addressable Download Exception {ex}\n에셋 다운중 예외 발생");
        }
    }
}

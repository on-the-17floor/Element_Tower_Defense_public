using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public static class ResourceLoader
{
    private static readonly Dictionary<string, AsyncOperationHandle> cache = new();

    /// <summary>
    /// Resource Load + Instantiate
    /// Instantiate와 같은 역할을 합니다.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static async Task<GameObject> Instantiate(string fileName)
    {
        var handle = Addressables.InstantiateAsync(fileName);
        await handle.Task;

        return handle.Result;
    }

    /// <summary>
    /// Instantiate한 오브젝트 메모리 해제 + 삭제
    /// </summary>
    /// <param name="instnace"></param>
    public static void ReleaseInstance(GameObject instnace)
    {
        Addressables.ReleaseInstance(instnace);
    }

    /// <summary>
    /// 동기 로드
    /// 초기화 또는 테스트 용도로만 사용하기
    /// </summary>
    /// <typeparam name="T">리소스 타입</typeparam>
    /// <param name="fileName">리소스 파일명</param>
    /// <param name="isCache">캐싱 여부</param>
    /// <returns></returns>
    public static T Load<T>(string fileName)
    {
        // 이미 캐싱된거 있으면 바로 반환
        if (cache.TryGetValue(fileName, out var cacheHandle))
        {
            if(cacheHandle.IsDone)
                return (T)cacheHandle.Result;
        }

        try
        {
            // 에셋 로드
            var handle = Addressables.LoadAssetAsync<T>(fileName);

            // 캐싱여부 확인 후 캐싱
            cache[fileName] = handle;

            // 에셋 로드 완료될때까지 대기
            T result = handle.WaitForCompletion();

            // 반환
            return result;
        }
        catch 
        {
            Debug.LogError($"[Resource Loader] {fileName} 파일 없음 또는 동기 로드 실패");
            return default;
        }
    }

    /// <summary>
    /// 비동기 로드
    /// </summary>
    /// <typeparam name="T">리소스 타입</typeparam>
    /// <param name="fileName">리소스 파일 명</param>
    /// <param name="isCache">캐싱 여부 </param>
    /// <returns></returns>
    public static async Task<T> AsyncLoad<T>(string fileName)
    {
        // 이미 캐싱된거 있으면 바로 반환
        if (cache.TryGetValue(fileName, out var cacheHandle))
        {
            if(cacheHandle.IsDone)
                return (T)cacheHandle.Result;
        }

        try
        {
            // 에셋 로드 및 대기
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(fileName);
            await handle.Task;

            // 에셋 로드 완료
            if(handle.Status == AsyncOperationStatus.Succeeded)
            {
                // 캐싱여부 확인 후 캐싱
                cache[fileName] = handle;

                // 반환
                return handle.Result;
            }

            // 에셋 로드 실패
            Debug.LogError($"[Resource Loader] {fileName} 비동기 로드 실패");
        }
        catch
        {
            Debug.LogError($"[Resource Loader] {fileName} 파일 없음");
        }

        return default;
    }

    public static async Task<List<T>> AsyncLoadLable<T>(string lableName)
    {
        // 이미 캐싱된거 있으면 바로 반환
        if (cache.TryGetValue(lableName, out var cacheHandle))
        {
            if(cacheHandle.IsDone)
            {
                IList<T> resourceList = (IList <T>)cacheHandle.Result;
                List<T> result = resourceList.OrderBy(resource =>
                {
                    if (resource is Object resourceObject)
                        return resourceObject.name;
                    return "";
                }).ToList();

                return result;
            }

        }

        // 에셋 로드
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(lableName, null);
        await handle.Task;

        // 에셋 로드 완료되면 
        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 캐싱 여부 확인 후 캐싱
            cache[lableName] = handle;

            // 정렬
            List<T> result = handle.Result.OrderBy(resource =>
            {
                if (resource is Object resourceObject)
                    return resourceObject.name;
                return "";
            }).ToList();

            // 반환
            return result;
        }

        Debug.Log($"[Resource Load] {lableName} lable 리소스 로드 실패");
        return null;
    }

    /// <summary>
    /// "fileName" 리소스 해제
    /// </summary>
    public static void Release(string fileName)
    {
        if (cache.TryGetValue(fileName, out var handle))
        {
            Addressables.Release(handle);
            cache.Remove(fileName);
        }
    }

    /// <summary>
    /// 전체 리소스 해제
    /// </summary>
    public static void ReleaseAll()
    {
        foreach (var obj in cache.Values)
        {
            Addressables.Release(obj);
        }
        cache.Clear();
    }
}

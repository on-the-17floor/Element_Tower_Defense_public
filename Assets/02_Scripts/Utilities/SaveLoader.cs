using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

public static class SaveLoader
{
    public static void Save<T>(T data, string fileName)
    {
        Debug.Log("세이브 시작");

        // 데이터 직렬화 객체 생성
        var container = new Version<T>(data);

        // JSON 문자열로 변환
        string saveData = JsonUtility.ToJson(container, true);

        // JSON 문자열을 파일로 저장
        File.WriteAllText(Application.persistentDataPath + $"/{fileName}.json", saveData);

        Debug.Log($"{fileName} 데이터 저장됨");
        Debug.Log($"saveData: {saveData}");
        Debug.Log($"저장경로: {Application.persistentDataPath}");

    }

    public static T Load<T>(string fileName)
    {
        Debug.Log("로드 시작");

        if (!CheckData(fileName))
        {
            Debug.Log($"{fileName}의 데이터가 존재하지 않습니다.");
            return default;
        }

        string loadData = File.ReadAllText(Application.persistentDataPath + $"/{fileName}.json");

        Debug.Log($"{fileName} 데이터 로드됨");
        Debug.Log($"{loadData} 데이터 로드됨");

        var container = JsonUtility.FromJson<Version<T>>(loadData);

        Debug.Log(Application.persistentDataPath + $"/{fileName}.json");
        Debug.Log($"{container.version} 데이터 로드됨");

        return container.version;
    }


    public static void SaveList<T>(List<T> data, string fileName)
    {
        Debug.Log("세이브 시작");

        // 리스트를 감싸는 Serialization 객체 생성
        var container = new Serialization<T>(data);

        // Data 컨테이너를 JSON 문자열로 변환
        string saveData = JsonUtility.ToJson(container, true); // true: 보기 좋은 포맷(줄 바꿈 포함)

        // JSON 문자열을 파일로 저장
        File.WriteAllText(Application.persistentDataPath + $"/{fileName}.json", saveData);

        Debug.Log($"{fileName} 데이터 리스트 저장됨");
        Debug.Log($"저장경로: {Application.persistentDataPath}");
    }

    public static List<T> LoadList<T>(string fileName)
    {
        if (!CheckData(fileName))
        {
            Debug.Log($"{fileName}의 데이터가 존재하지 않습니다.");
            return null;
        }

        string loadData = File.ReadAllText(Application.persistentDataPath + $"/{fileName}.json");

        // Json -> Serialization<T> 역직렬화
        var container = JsonUtility.FromJson<Serialization<T>>(loadData);

        Debug.Log($"{fileName} 데이터 로드됨");

        // 실제 List<T> 반환
        return container.target;
    }

    /// <summary>
    /// Dictionary<string, T>를 JSON으로 직렬화하여 파일로 저장합니다.
    /// </summary>
    public static void SaveDictionary<T>(Dictionary<string, T> dictionary, string fileName)
    {
        SerializableDictionary<T> serializableDict = new SerializableDictionary<T>();
        serializableDict.FromDictionary(dictionary);

        string saveData = JsonUtility.ToJson(serializableDict, true); // true: 보기 좋은 포맷(줄 바꿈 포함)

        Debug.Log("Serialized JSON: " + saveData);

        File.WriteAllText(Application.persistentDataPath + $"/{fileName}.json", saveData);
        Debug.Log("Dictionary saved to: " + fileName);
    }

    /// <summary>
    /// 파일에서 JSON을 읽어 Dictionary<string, T>로 역직렬화합니다.
    /// 파일이 없으면 빈 Dictionary를 반환합니다.
    /// </summary>
    public static Dictionary<string, T> LoadDictionary<T>(string fileName)
    {
        if (!CheckData(fileName))
        {
            Debug.Log($"{fileName}의 데이터가 존재하지 않습니다.");
            return null;
        }

        string loadData = File.ReadAllText(Application.persistentDataPath + $"/{fileName}.json");
        Debug.Log("Loaded JSON: " + loadData);

        SerializableDictionary<T> serializableDict = JsonUtility.FromJson<SerializableDictionary<T>>(loadData);
        if (serializableDict == null)
        {
            Debug.LogWarning("Deserialization failed.");
            return null;
        }

        Dictionary<string, T> result = serializableDict.ToDictionary();
        Debug.Log($"Deserialized Dictionary count: {result.Count}");
        return result;
    }

    //저장된 데이터 중 ID가 일치하는 데이터가 존재하는지 확인
    public static bool CheckData(string fileName)
    {
        return File.Exists(Application.persistentDataPath + $"/{fileName}.json");
    }

}

[System.Serializable]
public class Serialization<T>
{
    public List<T> target;

    // 생성자
    public Serialization(List<T> data)
    {
        target = data;
    }
}

[System.Serializable]
public class Version<T>
{
    public T version;

    // 생성자
    public Version(T data)
    {
        version = data;
    }
}

[Serializable]
public class SerializableDictionary<T>
{
    // 키는 string으로 고정, 값은 T 제네릭
    [SerializeField]
    private List<string> keys = new List<string>();

    [SerializeField]
    private List<T> values = new List<T>();

    /// <summary>
    /// Dictionary 데이터를 리스트 형태로 변환
    /// </summary>
    public void FromDictionary(Dictionary<string, T> dictionary)
    {
        keys.Clear();
        values.Clear();

        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    /// <summary>
    /// 리스트 데이터를 원래의 Dictionary<string, T> 형태로 복원
    /// </summary>
    public Dictionary<string, T> ToDictionary()
    {
        Dictionary<string, T> dict = new Dictionary<string, T>();
        int count = Mathf.Min(keys.Count, values.Count);
        for (int i = 0; i < count; i++)
        {
            dict[keys[i]] = values[i];
        }
        return dict;
    }
}
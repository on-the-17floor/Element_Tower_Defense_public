using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// 일단 임시
public enum SerchType
{
    uid,
    name,
    email
}

public static class FirebaseCRUD
{
    // 만들어야 하는 메서드 정리

    #region Create (생성): 새로운 데이터를 저장소에 추가하는 기능

    /// 지정 경로에 지정한 이름으로 데이터를 추가
    


    /// <summary>
    /// 지정 경로에 push()로 새로운 데이터를 추가하는 메서드
    /// </summary>
    public static async Task PushData<TData>(TData _data, string _dbPath = "userData")
    {
        string json = JsonUtility.ToJson(_data);
        
        // 받은 경로(dbPath)를 통해 reference 가져오기
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(_dbPath);

        await reference.Push()
        .SetRawJsonValueAsync(json)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log($"{_dbPath} 추가 완료");
            else
                Debug.LogError($"{_dbPath} 추가 실패: {task.Exception}");
        });

    }
    #endregion 


    #region Read (읽기): 저장된 데이터를 조회하거나 검색하는 기능

    /// <summary>
    /// 경로(dbPath)를 넣으면 서버에서 해당 경로의 데이터가 있는지 확인하고 DataSnapshot을 반환
    /// </summary>
    /// 
    /// <param name="dbPath">DB 경로</param>
    public static async Task<DataSnapshot> SearchData(string dbPath)
    {

        // 받은 경로(dbPath)를 통해 reference 가져오기
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(dbPath);

        // reference를 통해 데이터 가져오기
        // 만약 가져오는 작업을 실패하면 null, 아니라면 snapshot(t.Result)을 반환
        DataSnapshot snapshot = await reference.GetValueAsync()
            .ContinueWithOnMainThread(t => t.IsFaulted ? null : t.Result);

        if (snapshot != null && snapshot.Exists)
        {
            return snapshot;
        }
        else
        {
            Debug.LogError($"FetchListAsync: {dbPath} - 경로에 데이터가 존재하지 않음");
            return null;
        }
    }

    /// <summary> 지정 경로에서 특정 키워드(UID, 이름)로 검색해 DataSnapshot을 반환 </summary>
    /// 
    /// <param name="_idType">어떤 타입으로 검색 하는지</param>
    /// <param name="_id">검색할 ID값(임시)</param>
    /// <param name="_dbPath">DB 경로</param>
    public static async Task<DataSnapshot> SearchData(string _idType, string _id, string _dbPath = "userData")
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(_dbPath);

        // _dbPath에서 _idType를 기준으로 정렬하고, _id와 같은 값을 가진 데이터를 찾음
        Query query = reference
            .OrderByChild(_idType)
            .EqualTo(_id);

        var snap = await query.GetValueAsync();

        if (snap == null || !snap.Exists)
        {
            Debug.LogError($"FetchDataList: snapshot이 null입니다.");
            return null;
        }

        return snap;
    }



    /// <summary>DataSnapshot을 통해 TData 형식의 리스트를 반환하는 메서드</summary>
    /// 
    /// <typeparam name="TData">
    /// 데이터 클래스: ILoadableFromSnapshot를 상속받고 기본 생성자가 구현되어 있어야 함
    /// </typeparam>
    public static List<TData> GetDataList<TData>(DataSnapshot _snapshot)
    where TData : ILoadableFromSnapshot, new()
    {
        // TData(Data Class)형 리스트 생성
        var list = new List<TData>();

        if(_snapshot == null && !_snapshot.Exists)
        {
            Debug.LogError($"FetchDataList: snapshot이 null입니다.");
            return list;
        }

        foreach (var child in _snapshot.Children)
        {
            // child.Key는 개별 레코드(예: 몬스터)의 키(여기선 "001", "002" 등)
            // child 자체가 그 레코드 전체(attack, defence, speed 등 필드를 품고 있음)를 가리킵니다.

            TData data = new TData();
            data.LoadFromSnapshot(child);
            list.Add(data);

            // 코드 확인용 임시 코드
            Debug.Log($"FetchDataList: {child.Key} - {data}");
        }

        // 리스트 반환
        return list;
    }


    /// <summary>DataSnapshot을 통해 TData 형식의 데이터를 반환하는 메서드</summary>
    /// 
    /// <typeparam name="TData">
    /// 데이터 클래스: ILoadableFromSnapshot를 상속받고 기본 생성자가 구현되어 있어야 함
    /// </typeparam>
    public static TData GetData<TData>(DataSnapshot _snapshot)
    where TData : ILoadableFromSnapshot, new()
    {
        TData data = new TData();

        if (_snapshot == null && !_snapshot.Exists)
        {
            Debug.LogError($"FetchData: snapshot이 null입니다.");
            return data;
        }

        foreach (var child in _snapshot.Children)
        {
            // child.Key는 개별 레코드(예: 몬스터)의 키(여기선 "001", "002" 등)
            // child 자체가 그 레코드 전체(attack, defence, speed 등 필드를 품고 있음)를 가리킵니다.

            data.LoadFromSnapshot(child);

            // 코드 확인용 임시 코드
            Debug.Log($"FetchData: {child.Key} - {data}");

        }

        // 데이터 반환
        return data;
    }

    /// <summary>DataSnapshot을 통해 string 형식의 데이터를 반환하는 메서드</summary>
    public static string GetData(DataSnapshot _snapshot)
    {
        if (_snapshot == null && !_snapshot.Exists)
        {
            Debug.LogError($"FetchData: snapshot이 null입니다.");
            return null;
        }

        string data = _snapshot.Value.ToString();

        // 코드 확인용 임시 코드
        Debug.Log($"FetchData: {_snapshot.Key.ToString()} - {data}");

        // 데이터 반환
        return data;
    }

    #endregion


    #region Update (갱신): 기존 데이터를 수정하거나 변경하는 기능
    /// json으로 업로드
    /// 특정 경로의 특정 데이터만 수정
    #endregion 


    #region Delete (삭제): 저장된 데이터를 삭제하는 기능
    /// 지정 경로의 데이터 삭제
    #endregion 


}
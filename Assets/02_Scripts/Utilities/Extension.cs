using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class Extension 
{
    /// <summary>
    ///  모든 클래스의 기능을 확장할 수 있는 메서드
    ///  
    /// </summary>

    #region GameObject
    // T 타입의 컴포넌트를 return, 없으면 추가 후 return
    public static T AddAndReturnComponent<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component == null)
            gameObject.AddComponent<T>();

        return component;
    }

    // T 타입의 컴포넌트가 있으면 true 리턴, 없으면 false 리턴 
    public static bool HasComponent<T>(this GameObject gameObject)
    {
        return gameObject.GetComponent<T>() != null;
    }

    #endregion

    #region Trasnform

    // 부모가 있다면 부모 지정 후 위치, 회전, 크기 리셋 
    public static void InitTransform(this Transform trs, Transform parent = null)
    {
        if (parent != null)
            trs.SetParent(parent);

        trs.localPosition = Vector3.zero;
        trs.localRotation = Quaternion.identity;
        trs.localScale = Vector3.one;
    }

    // Transform의 SetActive
    public static void SetActive(this Transform trs, bool value)
    {
        if (trs != null)
            trs.gameObject.SetActive(value);
    }

    // Transform의 onoff 여부 return
    public static bool IsActive(this Transform trs)
    {
        return trs.gameObject.activeSelf;
    }

    #endregion

    #region Enum : 타입 자체에는 사용불가, 불가피하게 제네릭 사용 
    // Enum 의 길이 반환
    public static int EnumCount<TEnum>() where TEnum : Enum
    {
        return Enum.GetValues(typeof(TEnum)).Length;
    }

    // 인덱스로 Enum 요소 반환
    public static TEnum GetElement<TEnum>(int idx) where TEnum : Enum 
    {
        // enum 범위를 넘으면
        if (idx < 0 || idx >= EnumCount<Enum>()) 
        {
            throw new IndexOutOfRangeException($"Index {idx} is out of range for enum {typeof(TEnum).Name}");
        }

        Array values = Enum.GetValues(typeof(TEnum));
        return (TEnum)values.GetValue(idx);
    }

    // String을 같은 이름의 Enum으로 변환
    public static TEnum StringToEnum<TEnum>(string input) where TEnum : Enum
    {
        // return Enum.Parse<TEnum>(input);
        return (TEnum)Enum.Parse(typeof(TEnum), input);
    }

    // Enum을 Array 로 return
    public static Array ToArray<TEnum>() where TEnum : Enum
    {
        Array array = Enum.GetValues(typeof(TEnum));
        return array;
    }

    #endregion
}

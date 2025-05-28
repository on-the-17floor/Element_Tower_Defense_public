using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    GameObject newManager = new GameObject(typeof(T).Name, typeof(T));
                    instance = newManager.GetComponent<T>();
                }
            }

            return instance;
        }
    }

    public void Awake()
    {
        Initialize();
    }


    /// <summary>
    /// 초기화 메서드
    /// </summary>
    protected abstract void Initialize();
}

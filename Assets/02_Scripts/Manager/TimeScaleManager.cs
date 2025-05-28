using System;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleManager : Singleton<TimeScaleManager>
{
    public static event Action<float> onTimeScaleChanged;

    private float currentTimeScale = 1f;

    protected override void Initialize()
    {
    }
    public int ToggleSpeed()
    {
        int result = 0;

        if (Time.timeScale == 0f)
        {
            //게임플레이 시간 정지 
        }
        else
            currentTimeScale = currentTimeScale == 1f ? 2f : 1f;

        Time.timeScale = currentTimeScale;

        if (currentTimeScale == 1)
            result = 1;
        else if (currentTimeScale == 2)
            result = 2;

        onTimeScaleChanged?.Invoke(currentTimeScale);

        return result;
    }
    public float GetTimeScale()
    {
        return currentTimeScale;
    }
}

using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Localization : Singleton<Localization>
{
    private bool isChanging = false;
    public Action<LanguageType> changeLanguage;

    protected override void Initialize()
    {
        DontDestroyOnLoad(gameObject);

        // 이벤트 할당
        changeLanguage = ChangeLocale;

        // 저장된 언어로 초기화
        int index = PlayerPrefs.GetInt(Define.LANGUAGE_KEY, (int)LanguageType.Korean);
        changeLanguage.Invoke((LanguageType)index);
    }

    public async void ChangeLocale(LanguageType type)
    {
        if (isChanging) 
            return;

        await ApplyLanguageAsync(type);
    }

    public async Task ApplyLanguageAsync(LanguageType type)
    {
        // 중복 방지
        isChanging = true;
        int index = (int)type;

        var handle = LocalizationSettings.InitializationOperation;
        await handle.Task;

        if(handle.Status == AsyncOperationStatus.Succeeded)
        {
            // 예외처리
            if (index < 0 || index >= LocalizationSettings.AvailableLocales.Locales.Count)
                index = 0;

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

            // 언어 정보 저장
            PlayerPrefs.SetInt(Define.LANGUAGE_KEY, index);
            PlayerPrefs.Save();
        }

        isChanging = false;
    }
}

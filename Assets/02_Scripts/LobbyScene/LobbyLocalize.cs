using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLocalize : MonoBehaviour
{
    [SerializeField] private Button korean;
    [SerializeField] private Button english;

    void Start()
    {
        korean.onClick.AddListener(() =>
        {
            Localization.Instance.changeLanguage(LanguageType.Korean);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });
        english.onClick.AddListener(() =>
        {
            Localization.Instance.changeLanguage(LanguageType.English);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });
    }
}

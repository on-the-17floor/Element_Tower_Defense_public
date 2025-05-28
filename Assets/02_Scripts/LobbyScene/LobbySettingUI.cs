using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySettingUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject informationUI;
    [SerializeField] private GameObject gameSettingUI;

    [Header("Button")]
    [SerializeField] private Button informationButton;
    [SerializeField] private Button gameSettingButton;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        informationButton.onClick.AddListener(() =>
        {
            informationUI.SetActive(true);
            gameSettingUI.SetActive(false);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });

        gameSettingButton.onClick.AddListener(() =>
        {
            informationUI.SetActive(false);
            gameSettingUI.SetActive(true);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });

        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });
    }
}

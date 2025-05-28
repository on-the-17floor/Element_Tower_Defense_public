using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [Header(" UI ")]
    [SerializeField] private GameObject modeSelectUI;
    [SerializeField] private GameObject settingUI;

    [Header("모드 선택")]
    [SerializeField] private Button modeSelectButton;
    [SerializeField] private Button closeModeSelectButton;

    [Header("Setting")]
    [SerializeField] private Button settingButton;

    private void Start()
    {
        modeSelectButton?.onClick.AddListener(() =>
        {
            modeSelectUI.SetActive(true);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });

        closeModeSelectButton?.onClick.AddListener(() =>
        {
            modeSelectUI.SetActive(false);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });

        settingButton?.onClick.AddListener(() =>
        {
            settingUI.SetActive(true);
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
        });
    }
}

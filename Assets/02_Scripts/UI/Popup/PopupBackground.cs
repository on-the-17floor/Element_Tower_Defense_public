using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBackground : MonoBehaviour
{
    private Button closeUI;
    private bool isPopupOpen;

    public bool IsPopupOpen => isPopupOpen;
    private void Awake()
    {
        closeUI = GetComponent<Button>();

        closeUI.onClick.AddListener(OnHideBackground);

        gameObject.SetActive(false);
    }

    public void OnShowBackground()
    {
        // background 활성화
        gameObject.SetActive(true);
        isPopupOpen = true;

        // 버튼 비활성화 
        ButtonInteractable(false);
    }

    public void OnHideBackground()
    {
        // 켜져있는 PopupUI 전부 끄기
        foreach (var ui in UIManager.Instance.PopupUIs)
        {
            if (ui.gameObject.activeSelf)
                ui.Hide();
        }

        // background 비활성화
        gameObject.SetActive(false);
        isPopupOpen = false;

        // 버튼 활성화
        ButtonInteractable(true);
    }

    private void ButtonInteractable(bool active)
    {
        foreach(Button btn in UIManager.Instance.ButtonManager.Buttons)
        {
            btn.interactable = active;
        }
    }

    public void RemoveAllEvent()
    {
        closeUI.onClick.RemoveAllListeners();
    }
}

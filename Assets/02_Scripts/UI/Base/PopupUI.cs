using System;
using System.Net.Security;
using UnityEngine;

public abstract class PopupUI : MonoBehaviour
{
    public abstract void Initialize();
    public virtual void StartInit() { }

    protected Action OnShowEvent;
    protected Action OnHideEvent;

    public void Show()
    {
        // popup Background 활성화
        UIManager.Instance.PopupBackground.OnShowBackground();

        // UI 활성화
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        OnShowEvent?.Invoke();
    }

    public void Hide()
    {
        gameObject.SetActive(false);

        OnHideEvent?.Invoke();
    }
}

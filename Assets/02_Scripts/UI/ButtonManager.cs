using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{
    Setting,
    Reward,
    Mission,
    Upgrade
}

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private List<Button> buttons;

    public List<Button> Buttons => buttons;

    public void Initialize()
    {
        // 버튼 이벤트 초기화
        buttons[(int)ButtonType.Setting].onClick.AddListener(OnSetting);
        buttons[(int)ButtonType.Reward].onClick.AddListener(OnReward);
        buttons[(int)ButtonType.Mission].onClick.AddListener(OnMission);
        buttons[(int)ButtonType.Upgrade].onClick.AddListener(OnUpgrade);
    }

    private void OnSetting()
    {
        UIManager.Instance.PopupUIs[(int)PopupUIType.SettingsUI].Show();
    }

    private void OnReward()
    {
        UIManager.Instance.PopupUIs[(int)PopupUIType.RewardUI].Show();
    }

    private void OnMission()
    {
        UIManager.Instance.PopupUIs[(int)PopupUIType.MissionUI].Show();
    }

    private void OnUpgrade()
    {
        UIManager.Instance.PopupUIs[(int)PopupUIType.LevelupUI].Show();
    }
}
using TMPro;
using UnityEngine;

public class PlayerInfo : BaseUI
{
    [SerializeField] private TextMeshProUGUI summonTokenText;
    [SerializeField] private TextMeshProUGUI upgradeTokenText;
    [SerializeField] private TextMeshProUGUI lifeText;

    public override void Initialize()
    {
        UserManager.Instance.onSummonTokenChange += SetPlayerInfo;
        UserManager.Instance.onUpgradTokenChange += SetPlayerInfo;
        UserManager.Instance.onLifeChange += SetPlayerInfo;
        SetPlayerInfo();
    }

    public void SetPlayerInfo()
    {
        summonTokenText.text = UserManager.Instance.SummonToken.ToString();
        upgradeTokenText.text = UserManager.Instance.UpgradToken.ToString();
        lifeText.text = UserManager.Instance.Life.ToString();
    }

}

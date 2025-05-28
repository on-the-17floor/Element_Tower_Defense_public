using TMPro;
using UnityEngine;

public class RoundUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI roundText;

    public override void Initialize()
    {
        StageManager.Instance.OnRoundChange += SetRound;
        SetRound();
    }

    public void SetRound()
    {
        roundText.text = $"Round ( {StageManager.Instance.CurrentRound + 1} / 40 )";
    }
}

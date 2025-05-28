using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class RewardUI : PopupUI
{

    private RectTransform rectTransform;

    [SerializeField] private Button goldbtn;
    [SerializeField] private Button stonebtn;
    [SerializeField] private Button towerbtn;
    [SerializeField] private Button exitbtn;

    [SerializeField] private TextMeshProUGUI ticket;
    private PopupUI rewardSelectTower;
    public override void Initialize()
    {
        rewardSelectTower = UIManager.Instance.PopupUIs[(int)PopupUIType.RewardSelectTower];

        rectTransform = GetComponent<RectTransform>();

        OnShowEvent += ShowAnimation;

        goldbtn.onClick.AddListener(OngoldbtnClick);
        stonebtn.onClick.AddListener(OnstonbtnClick);
        towerbtn.onClick.AddListener(OntowerbtnClick);
        exitbtn.onClick.AddListener(() => 
        {
            UIManager.Instance.PopupBackground.OnHideBackground();
            Hide();
        });

        ticket.text = $"0";
        UserManager.Instance.onRewardTicketChange += SetTicket;
    }

    private void ShowAnimation()
    {
        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
    public void SetTicket()
    {
        ticket.text = UserManager.Instance.RewardTicket.ToString();
    }
    void OngoldbtnClick()
    {
        if (UserManager.Instance.HasRewardTicket())
        {
            UserManager.Instance.AddSummonToken(400);
            //UIManager.Instance.rewardMessageUI.OnMessage(Define.MESSAGE_KEY_TICKET, RewardType.SummonToken, 400);
            UserManager.Instance.RewardTicket--;

            // 애널리틱스 이벤트 전송
            Analytics.UseTicketEvent(TicketType.SummonToken, StageManager.Instance.CurrentRound);
        }

        UIManager.Instance.PopupBackground.OnHideBackground();
        Hide();
    }

    void OnstonbtnClick()
    {
        if (UserManager.Instance.HasRewardTicket())
        {
            UserManager.Instance.AddUpgradToken(400);
            //UIManager.Instance.rewardMessageUI.OnMessage(Define.MESSAGE_KEY_TICKET, RewardType.UpgradeToken, 400);
            UserManager.Instance.RewardTicket--;

            // 애널리틱스 이벤트 전송
            Analytics.UseTicketEvent(TicketType.UpgradeToken, StageManager.Instance.CurrentRound);
        }

        UIManager.Instance.PopupBackground.OnHideBackground();
        Hide();
    }

    void OntowerbtnClick()
    {
        if (UserManager.Instance.HasRewardTicket())
        {
            rewardSelectTower.Show();
        }
        else
        {
            UIManager.Instance.PopupBackground.OnHideBackground();
        }
        Hide();
    }
}

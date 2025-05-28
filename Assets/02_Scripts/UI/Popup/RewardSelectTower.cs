using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RewardSelectTower : PopupUI
{
    private RectTransform rectTransform;

    [SerializeField] private Button fireTower;
    [SerializeField] private Button grassTower;
    [SerializeField] private Button lightTower;
    [SerializeField] private Button earthTower;
    [SerializeField] private Button waterTower;

    public override void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();
        OnShowEvent += ShowAnimation;

        fireTower.onClick.AddListener(() => OnSelectTower(ElementType.Fire));
        grassTower.onClick.AddListener(() => OnSelectTower(ElementType.Grass));
        lightTower.onClick.AddListener(() => OnSelectTower(ElementType.Light));
        earthTower.onClick.AddListener(() => OnSelectTower(ElementType.Earth));
        waterTower.onClick.AddListener(() => OnSelectTower(ElementType.Water));
    }
    private void ShowAnimation()
    {
        rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
        rectTransform.DOAnchorPosY(0, 0.5f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    private void OnSelectTower(ElementType type)
    {

        TowerManager.Instance.TowerGenerator.UseTowerTicket(TowerTier.Blue, type);
        UserManager.Instance.AddSummonToken(100);
        UserManager.Instance.RewardTicket--;

        // 애널리틱스 이벤트 전송
        Analytics.UseTicketEvent(TicketType.BlueTower, StageManager.Instance.CurrentRound);

        UIManager.Instance.PopupBackground.OnHideBackground();
        Hide();
    }
}

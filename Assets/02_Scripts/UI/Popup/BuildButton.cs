using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuildButton : PopupUI
{
    private bool isBuilding;
    private RectTransform rectTransform;
    [SerializeField] private Button buildButton;

    public override void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();

        isBuilding = false;

        // 타워 생성 이벤트 할당
        buildButton.onClick.AddListener(OnBuild);

        OnShowEvent += ShowAnimation;
        OnHideEvent += CancelBuild;
    }

    private void OnBuild()
    {
        if (UserManager.Instance.HasEnoughSummonToken(Define.DEFAULT_SUMMON_COST))
        {
            if (!TowerManager.Instance.TowerData.CheckAssestLoaded())
                return;

            UserManager.Instance.UseSummonToken(Define.DEFAULT_SUMMON_COST);
            TowerManager.Instance.TowerGenerator.CreateNewTower(TowerTier.Gray);
        }

        UIManager.Instance.PopupBackground.OnHideBackground();
        Hide();
    }

    private void CancelBuild()
    {
        TowerManager.Instance.TileSelector.ResetTile();
        isBuilding = false;
    }

    private void ShowAnimation()
    {
        TowerTile tile = TowerManager.Instance.TileSelector.CurrentTile;

        if (isBuilding || tile == null)
        {
            UIManager.Instance.PopupBackground.OnHideBackground();
            Hide();
            return;
        }

        isBuilding = true;

        // 선택된 타일의 위치로 빌드버튼 이동
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tile.transform.position);
        transform.position = screenPos;

        // 빌드 버튼 사이즈 조절
        float size = CameraManager.Instance.buildButtonSize;
        Vector3 targetScale = new Vector3(size, size, size);

        rectTransform.localScale = Vector3.zero;
        rectTransform.DOScale(targetScale, 0.5f)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
}

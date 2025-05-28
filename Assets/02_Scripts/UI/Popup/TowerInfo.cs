using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class TowerInfo : PopupUI
{
    [Header("Element Base Colors")]
    [SerializeField] private Color[] typeColors;

    [Header("Tier Base Colors")]
    [SerializeField] private Color[] tierColors;

    [Header("Infomation HighLight")]
    [SerializeField] private Image typeBackground;
    [SerializeField] private Image tierBackground;

    [Header("Special Stat")]
    [SerializeField] private GameObject speicalStatUI;
    [SerializeField] private TextMeshProUGUI speicalText;

    [Header("Text Components")]
    [SerializeField] private TextMeshProUGUI towerElemental;
    [SerializeField] private TextMeshProUGUI towerTear;
    [SerializeField] private TextMeshProUGUI towerAttack;
    [SerializeField] private TextMeshProUGUI towerAttackSpeed;
    [SerializeField] private TextMeshProUGUI towerRange;
    [SerializeField] private TextMeshProUGUI towerIgnoreDefense;
    [SerializeField] private TextMeshProUGUI towerLevel;

    private TileClickHandler currentTileHandler;

    public override void Initialize()
    {
        OnShowEvent += ShowAnimation;
        OnHideEvent += HideAnimation;
    }

    private void ShowAnimation()
    {
        RectTransform rect = GetComponent<RectTransform>();
        rect.localScale = Vector3.zero;
        rect.DOScale(Vector3.one, 0.3f)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
    private void HideAnimation()
    {
        // 사거리 표시 OFF / 초기화
        currentTileHandler?.OnAttackRange(false);
        currentTileHandler = null;
        // 타일 선택 해제
        TowerManager.Instance.TileSelector?.ResetTile();
    }
    public void SetTowerInfo(TowerData data, int level, float buffRate, TileClickHandler tileHandler)
    {
        currentTileHandler = tileHandler;
        string localizedElement = GetLocalizedElement(data.elementType);
        string localizedTier = GetLootTier(data.tierType);

        SetTowerElemental(localizedElement, data.elementType);
        SetTowerTear(localizedTier, data.tierType);
        SetTowerAttack((int)data.damage, buffRate, data.elementType);
        SetTowerAttackSpeed(data.attackSpeed);
        SetTowerRange(data.attackRange);
        SetTowerIgnore(data.ignoreDefenseRate);
        SetTowerLevel(level);

        SetSpecialStat(data);
    }

    private string GetLocalizedElement(ElementType element)
    {
        // Enum 값을 Localization Table 키로 변환
        string key = element.ToString();

        var localizedElement = new LocalizedString("MyTable", key);
        return localizedElement.GetLocalizedString();
    }

    private string GetLootTier(TowerTier towertier)
    {
        string key = towertier.ToString();

        var localizedTier = new LocalizedString("MyTable", key);
        return localizedTier.GetLocalizedString();
    }

    private void SetSpecialStat(TowerData data)
    {
        speicalStatUI.SetActive(false);

        if (data.tierType >= TowerTier.Blue)
        {
            speicalStatUI.SetActive(true);

            string key = $"Special_{data.elementType.ToString()}";
            var localized = new LocalizedString($"MyTable", key);

            speicalText.text = localized.GetLocalizedString();
        }
    }

    private void SetTowerElemental(string elemental, ElementType type)
    {
        towerElemental.text = elemental;
        typeBackground.color = typeColors[(int)type];
    }
    private void SetTowerTear(string tear, TowerTier tier)
    {
        towerTear.text = tear;
        tierBackground.color = tierColors[(int)tier];
    }

    private void SetTowerAttack(float attack, float buffRate, ElementType elementType)
    {
        float upgradeAttack = (attack * TowerManager.Instance.TowerUpgrade.GetUpgradeRate(elementType)) - attack;
        string damageText = $"{attack:F1}";

        if (TowerManager.Instance.TowerUpgrade[elementType] > 0)
            damageText += $"( +{(upgradeAttack):F1} )";

        towerAttack.text = damageText;
    }
    private void SetTowerAttackSpeed(float attackSpeed)
    {
        towerAttackSpeed.text = attackSpeed.ToString();
    }
    private void SetTowerRange(float range)
    {
        towerRange.text = range.ToString();
    }

    private void SetTowerIgnore(float ignore)
    {
        float percent = 1f - ignore;
        towerIgnoreDefense.text = $"{(percent * 100)}%";
    }

    private void SetTowerLevel(int level)
    {
        towerLevel.text = level.ToString();
    }
}

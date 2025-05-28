using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class LevelupUI : PopupUI
{
    private RectTransform rectTransform;

    [Header("Upgrade")]
    [SerializeField] private List<Button> upgradeButtons;
    [SerializeField] private List<TextMeshProUGUI> upgradePriceText;
    [SerializeField] private List<TextMeshProUGUI> upgradeLevelText;

    [Header("Buy Button")]
    [SerializeField] private Button stonbuy;

    [Header("BuyToken Text Animation")]
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private RectTransform textOriginTransform;
    private Vector3 originalPos;
    private Queue<BuyTokenUI> textPool;

    public override void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();

        OnShowEvent += ShowAnimation;

        upgradeButtons[(int)ElementType.Fire].onClick.AddListener(() => TryUpgrade(ElementType.Fire));
        upgradeButtons[(int)ElementType.Water].onClick.AddListener(() => TryUpgrade(ElementType.Water));
        upgradeButtons[(int)ElementType.Earth].onClick.AddListener(() => TryUpgrade(ElementType.Earth));
        upgradeButtons[(int)ElementType.Grass].onClick.AddListener(() => TryUpgrade(ElementType.Grass));
        upgradeButtons[(int)ElementType.Light].onClick.AddListener(() => TryUpgrade(ElementType.Light));

        stonbuy.onClick.AddListener(BuyStone);

        originalPos = stonbuy.transform.position;
        textPool = new();
    }

    private void ShowAnimation()
    {
        rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
        rectTransform.DOAnchorPosY(0, 0.3f)
            .SetLink(gameObject)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    private void TryUpgrade(ElementType type)
    {
        StageManager.Instance.UpgradeElement(type, out int level);
        SetUpgradeText(type, level);
    }

    private void SetUpgradeText(ElementType type, int level)
    {
        int index = (int)type;
        upgradeLevelText[index].text = $"+{level}";
        upgradePriceText[index].text = $"{level + 1}";
    }

    private void BuyStone()
    {
        if(UserManager.Instance.SummonToUpgrade(out int token))
        {
            BuyStonTextAnimation(token);
        }
    }

    private void BuyStonTextAnimation(int value)
    {
        if(!textPool.TryDequeue(out BuyTokenUI tokenText))
        {
            GameObject text = Instantiate(textPrefab, textOriginTransform);
            tokenText = text.GetComponent<BuyTokenUI>();

            tokenText.Initialize(textPool);
        }

        tokenText.Play(value);
    }
}

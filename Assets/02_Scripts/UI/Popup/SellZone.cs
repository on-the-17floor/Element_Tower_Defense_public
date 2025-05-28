using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SellZone : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI valueText;

    public void OnSellZone(bool active, TowerTier tier = default)
    {
        gameObject.SetActive(active);

        if (!active)
            return;

        if (tier == TowerTier.Hyper)
        {
            var localized = new LocalizedString($"MyTable", "nonSale");
            valueText.text = localized.GetLocalizedString();
            return;
        }

        int cost = Define.DEFAULT_SELLCOST * (1 << (int)tier);
        valueText.text = $"{cost}";
    }
}

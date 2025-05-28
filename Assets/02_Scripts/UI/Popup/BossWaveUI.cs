using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BossWaveUI : MessageUI
{

    private RectTransform rect;
    private float rectY;
    public override void Initialize()
    {
        rect = GetComponent<RectTransform>();
        rectY = rect.anchoredPosition.y;
    }

    public void OnMessage()
    {
        gameObject.SetActive(true);
        rect.anchoredPosition = new Vector2(-Screen.width, rectY);
        rect.DOAnchorPos(new Vector2(0, rectY), 0.5f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject)
            .OnComplete(() => StartCoroutine(HideDelay()));
    }

    private  void HideAnimation()
    {
        rect.DOAnchorPos(new Vector2(Screen.width, rectY), 0.5f)
            .SetEase(Ease.InCubic)
            .SetLink(gameObject)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    IEnumerator HideDelay()
    {
        yield return new WaitForSeconds(5f);
        HideAnimation();
    }
}

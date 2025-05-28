using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuyTokenUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform rectTrasnform;
    private Queue<BuyTokenUI> poolReference;
    private Vector3 originPos;
    private Color defaultColor;

    public void Initialize(Queue<BuyTokenUI> pool)
    {

        rectTrasnform = GetComponent<RectTransform>();
        poolReference = pool;

        originPos = rectTrasnform.anchoredPosition;
        defaultColor = text.color;
    }

    public void Play(int value)
    {
        gameObject.SetActive(true);
        text.text = $"+ {value}";
        text.color = defaultColor;

        ShowAnimation();
    }

    private void ShowAnimation()
    {
        rectTrasnform.anchoredPosition = originPos;

        rectTrasnform.DOAnchorPosY(originPos.y + 50f, 0.5f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                StartCoroutine(TextDelay());
            });

    }

    private void HideAnimation()
    {
        text.DOFade(0, 0.3f).SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                gameObject.SetActive(false);
                poolReference.Enqueue(this);
            });
    }

    private IEnumerator TextDelay()
    {
        yield return new WaitForSeconds(0.3f);
        HideAnimation();
    }
}

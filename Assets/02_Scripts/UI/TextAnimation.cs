using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        text.DOFade(0.5f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

}

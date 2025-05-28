using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class RandomBackground : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private Image background;

    [Header("Resource")]
    [SerializeField] private Sprite[] backgroundSprites;

    int currentIndex;

    private float fadeDelay = 4f;
    private float fadeDuration = 0.5f;
    void Start()
    {

        if (backgroundSprites == null || backgroundSprites.Length == 0 || background == null)
            return;

        // 배열섞기
        currentIndex = 0;
        Random rand = new();
        backgroundSprites = backgroundSprites.OrderBy(x => rand.Next()).ToArray();

        ChangeNextSprite();
        FadeOut();
    }

    private void FadeOut()
    {
        DOVirtual.DelayedCall(fadeDelay, () => 
        {
            background.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                ChangeNextSprite();
                FadeIn();
            });
        });
    }

    private void FadeIn()
    {
        background.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            FadeOut();
        });
    }

    private void ChangeNextSprite()
    {
        currentIndex++;
        if (currentIndex >= backgroundSprites.Length)
            currentIndex = 0;

        background.sprite = backgroundSprites[currentIndex];
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public struct MessageData
{
    public string titleKey;
    public RewardType[] rewardTypeKeys;
    public int[] values;

    public MessageData(string title, RewardType[] rewardType, int[] rewardValue) : this()
    {
        this.titleKey = title;
        this.rewardTypeKeys = rewardType;
        this.values = rewardValue;
    }
}

public class RewardMessage : MessageUI
{
    [Header("Reward Message")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI rewardTitle;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private float messageDuration;
    [SerializeField] private float fadeDuration;

    private Queue<MessageData> messageQueue;
    private bool isShowing;

    public override void Initialize()
    {
        messageQueue = new();
        isShowing = false;
    }

    public void OnMessage(string title, RewardType[] rewardType, int[] rewardValue)
    {
        // 메세지 Queue 추가
        messageQueue.Enqueue(new MessageData(title, rewardType, rewardValue));

        // 현재 메세지 재생중이 아닐때
        if (!isShowing)
            ShowNextMessage();
    }

    public void OnMessage(string title, RewardType reward, int value)
    {
        RewardType[] rewardType = { reward };
        int[] rewardValue = { value };

        // 메세지 Queue 추가
        messageQueue.Enqueue(new MessageData(title, rewardType, rewardValue));

        // 현재 메세지 재생중이 아닐때
        if (!isShowing)
            ShowNextMessage();
    }

    private void ShowNextMessage()
    {
        // 더 이상 출력할 메세지가 없을때
        if (messageQueue.Count == 0)
        {
            isShowing = false;
            return;
        }

        // 재생중
        isShowing = true;

        // 텍스트 변경
        var data = messageQueue.Dequeue();
        string tmpText = "";
        for (int i = 0; i < data.rewardTypeKeys.Length; i++)
        {
            string rewardKey = $"Message_{data.rewardTypeKeys[i].ToString()}";
            string reward = LocalizationSettings.StringDatabase.GetLocalizedString("MyTable", rewardKey);
            tmpText += $"{reward} +{data.values[i]} ";
        }

        string localizedTitle = LocalizationSettings.StringDatabase.GetLocalizedString("MyTable", data.titleKey);
        rewardTitle.text = localizedTitle;
        rewardText.text = tmpText;

        // UI ON
        ShowAnimation();
    }

    private void ShowAnimation()
    {
        gameObject.SetActive(true); // 활성화
        canvasGroup.DOKill(); // DOTween 애니메이션 중지
        canvasGroup.alpha = 1f;

        // messageDuration초 후 페이드아웃
        DOVirtual.DelayedCall(messageDuration, () =>
        {
            canvasGroup.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                // UI 닫기
                gameObject.SetActive(false);

                // 다음 메세지 실행
                ShowNextMessage();
            });
        });
    }
}

using DG.Tweening;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : PopupUI
{
    private RectTransform rectTransform;

    [SerializeField] private Button[] missionButtons;   // 미션 몬스터 생성 버튼
    [SerializeField] private Image[] missionImages;     // 쿨타임 이미지
    [SerializeField] private TextMeshProUGUI[] missionCoolText;

    [SerializeField] private List<Mission> missionList;

    public override void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();

        OnShowEvent += ShowAnimation;
    }

    public override void StartInit()
    {
        // 예외 처리
        if (StageManager.Instance.missionList == null || StageManager.Instance.missionList.Count <= 0)
            return;
        
        // 미션 데이터 초기화
        missionList = StageManager.Instance.missionList;

        // 쿨타임 종료시 이벤트 할당

        int index = 0;
        foreach(Mission mission in missionList)
        {
            Button button = missionButtons[index];
            Image image = missionImages[index];
            TextMeshProUGUI text = missionCoolText[index];

            mission.OnCoolTimeEnded += () => CoolTimeEnd(button, image, text);
            mission.OnCoolDown += () => CoolTimeImage(mission, image, text);
            missionButtons[index].onClick.AddListener(() => OnMissionStart(mission, button, image));

            index++;
        }
    }

    private void ShowAnimation()
    {
        rectTransform.anchoredPosition = new Vector2(0, -Screen.height);
        rectTransform.DOAnchorPosY(0, 0.3f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    /// <summary>
    /// 쿨타임 종료 이벤트
    /// </summary>
    /// <param name="index"></param>
    private void CoolTimeEnd(Button missionButton, Image missionImage, TextMeshProUGUI missionCool)
    {
        missionButton.interactable = true;

        Color defaultColor = new Color(1f, 1f, 1f, 1f);
        missionImage.fillAmount = 1f;
        missionImage.color = defaultColor;

        missionCool.text = "";
    }

    /// <summary>
    /// 쿨타임 감소 연출 이벤트
    /// </summary>
    /// <param name="index"></param>
    private void CoolTimeImage(Mission mission, Image missionImage, TextMeshProUGUI missionCool)
    {
        float targetCoolTime = mission.data.coolTime;
        float currentCoolTime = mission.CoolTime;

        missionImage.fillAmount = Mathf.Abs(1f - (currentCoolTime / targetCoolTime));

        float m = Mathf.FloorToInt(currentCoolTime / 60f);
        float s = Mathf.FloorToInt(currentCoolTime % 60f);
        missionCool.text = $"{m:00}:{s:00}";
    }

    /// <summary>
    /// 미션 시작 
    /// </summary>
    /// <param name="mission"></param>
    /// <param name="index"></param>
    private void OnMissionStart(Mission mission, Button missionButton, Image missionImage)
    {
        // 미션 시작중
        if (mission.IsChallenging)
            return;

        // 미션 시작
        mission.MissionStart();

        // 버튼 비활성화
        missionButton.interactable = false;
        missionImage.fillAmount = 0f;
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGuideUI : MonoBehaviour
{
    [SerializeField]
    private Button guideButton; // 가이드 버튼
    [SerializeField]
    private GameObject guidePanel;
    [SerializeField]
    private GameObject[] guideContentList;

    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button leftArrow;
    [SerializeField]
    private Button rightArrow;

    [SerializeField]
    private TextMeshProUGUI pageText;

    [SerializeField] private int preIndex = 0;
    [SerializeField] private int currIndex = 0;

    private int minIndex;
    private int maxIndex;

    private void Awake()
    {
        minIndex = 0;
        maxIndex = guideContentList.Length - 1;

        // 켜져있으면 끄기 
        if (guidePanel.activeSelf == true)
            guidePanel.SetActive(false);

        // 초기 첫번째 panel 열어놓기
        ChangePanel(0);

        // panel 열기
        guideButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
            guidePanel.SetActive(true);
        } );
        // panel 닫기
        closeButton.onClick.AddListener(() => 
        {
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
            guidePanel.SetActive(false);
        } );

        // 왼 화살표 클릭
        leftArrow.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFXPlay(SFXType.UiClick);
            ChangePanel(-1);
        });

        // 오른 화살표 클릭
        rightArrow.onClick.AddListener(() =>
        {
            SoundManager.Instance.SFXPlay(SFXType.UiClick); 
            ChangePanel(1);
        });

    }

    private void ChangePanel(int dir) 
    {
        preIndex = currIndex;
        currIndex += dir;

        // 인덱스 보정
        if (currIndex < 0)
            currIndex = maxIndex;
        else if (currIndex >= guideContentList.Length)
            currIndex = minIndex;

        // 페이지 텍스트 업데이트 ex) 1 / 8
        pageText.text = (currIndex + 1).ToString() + " / " + (maxIndex + 1).ToString();

        // 이전 인덱스에 해당하는 panel Off
        guideContentList[preIndex].SetActive(false);

        // 인덱스에 해당하는 panel ON
        guideContentList[currIndex].SetActive(true);
    }

}

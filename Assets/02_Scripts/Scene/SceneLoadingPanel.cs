using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneLoadingPanel : MonoBehaviour
{
    [Header("===Panel===")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI loadingText;
    private string baseLoadingText = "Loading";
    private string[] loadingTextList = new string[5] { "", ".", "..", "...", "...." };
    private Coroutine coru;

    private void Awake()
    {
        // canvas 끄기
        LoadingPanelOnOff(false);
    }

    // SceneLoader에서 동작 시작
    public void OnLoadingPanel() 
    {
        // canvas 켜기
        LoadingPanelOnOff(true);

        // 로딩 텍스트 코루틴 시작 
        coru = StartCoroutine(AnimationLoadingText());
    }

    // SceneLoader에서 동작 끝
    public void EndLadingPanel() 
    {
        // 텍스트 코루틴 종료
        if (coru != null)
            StopCoroutine(coru);


        // canvas 끄기
        canvas.gameObject.SetActive(false);
    }

    // PanelOnOff
    private void LoadingPanelOnOff(bool flag) 
    {
        canvas.gameObject.SetActive(flag);
    }

    private IEnumerator AnimationLoadingText()
    {
        int currIdx = 0;

        while (true)
        {
            loadingText.text = baseLoadingText + loadingTextList[currIdx];

            // loadingTextList의 길이만큼
            currIdx++;
            currIdx %= loadingTextList.Length;

            yield return new WaitForSeconds(0.2f);
        }
    }
}

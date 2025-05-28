using UnityEngine;
using UnityEngine.UI;

public class ButtonSfx : MonoBehaviour
{
    private void Awake()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // 비활성 포함
        foreach (Button btn in buttons)
        {
            var capturedBtn = btn;
            capturedBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.SFXPlay(SFXType.UiClick);
            });
        }
    }
}

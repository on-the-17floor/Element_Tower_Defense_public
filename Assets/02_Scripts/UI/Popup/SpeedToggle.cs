using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class SpeedToggle : BaseUI
{
    [SerializeField] private Button toggleButton;

    [Header("UI")]
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Sprite speed1;
    [SerializeField] private Sprite speed2;
    [SerializeField] private Image speedimage;
    private Image sliderFillRect;

    public override void Initialize()
    {
        sliderFillRect = speedSlider.fillRect.GetComponent<Image>();
        toggleButton.onClick.AddListener(OnSpeed);
    }

    void OnSpeed()
    {
        int scale = TimeScaleManager.Instance.ToggleSpeed();

        if(scale == 1)
        {
            speedSlider.value = 0f;
            speedimage.sprite = speed1;
            sliderFillRect.color = new Color(1f, 1f, 1f, 1f);
        }

        else if( scale == 2)
        {
            speedSlider.value = 1f;
            speedimage.sprite = speed2;
            speedSlider.fillRect.GetComponent<Image>().color = new Color(181f / 255f, 101f / 255f, 29f / 255f, 1f);
        }
    }
}

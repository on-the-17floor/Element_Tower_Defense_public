using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyLevel : BaseUI
{
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private Image difficultyImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hardSprite;
    public override void Initialize()
    {
        difficultyText.text = "";
    }
    private void Start()
    {
        SetDifficultyType();
    }
    private void SetDifficultyType()
    {
        switch (DataStore.difficultyType)
        {
            case DifficultyType.Normal:
                difficultyText.text = "Normal";
                difficultyText.color = Color.white;
                difficultyImage.sprite = normalSprite;
                break;
            case DifficultyType.Hard:
                difficultyText.text = "Hard";
                difficultyText.color = Color.red;
                difficultyImage.sprite = hardSprite;
                break;
        }
    }
}

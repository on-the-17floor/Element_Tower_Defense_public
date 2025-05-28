using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DownloadScene : MonoBehaviour
{
    [Header("download UI")]
    [SerializeField] private GameObject downloadUI;
    [SerializeField] private TextMeshProUGUI downloadSizeText;
    
    [Header("progress")]
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image progress;

    [Header("button")]
    [SerializeField] private Button touchToStart;

    private void Start()
    {
        progressText.text = "download... 0%";
        progress.fillAmount = 0;

        touchToStart.gameObject.SetActive(false);

        touchToStart.onClick.AddListener(() => SceneManager.LoadScene("00_Load"));
    }

    public void UpdateProgressText(string text)
    {
        progressText.text = text;
    }

    public void UpdateProgress(float percent)
    {
        UpdateProgressText($"download... {percent * 100:F1}%");

        progress.fillAmount = percent;
    }

    public void OnStartButton()
    {
        touchToStart.gameObject.SetActive(true);
    }

    public void OnDownloadUI(bool active)
    {
        downloadUI.SetActive(active);
    }

    public void SetDownloadSize(long size)
    {
        downloadSizeText.text = $"{size / (1024f * 1024f):F2} MB";
    }
}

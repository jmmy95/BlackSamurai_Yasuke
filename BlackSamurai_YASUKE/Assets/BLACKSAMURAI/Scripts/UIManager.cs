using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TMP_Text coinText;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject levelCompletePanel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
            RefreshCoins(GameManager.Instance.Coins);

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
    }

    public void SetHealth(int current, int max)
    {
        if (healthBar == null) return;

        healthBar.maxValue = max;
        healthBar.value = current;
    }

    public void RefreshCoins(int amount)
    {
        if (coinText != null)
            coinText.text = amount.ToString();
    }

    public void TogglePause()
    {
        bool paused = Time.timeScale > 0f;
        Time.timeScale = paused ? 0f : 1f;

        if (pausePanel != null)
            pausePanel.SetActive(paused);
    }

    public void ShowLevelComplete()
    {
        Time.timeScale = 0f;

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
}

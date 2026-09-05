using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        // Hide panels when Main Menu starts
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    // PLAY
    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level01");
    }

    // CONTINUE
    public void ContinueGame()
    {
        Time.timeScale = 1f;

        int level = PlayerPrefs.GetInt("LastLevel", 1);

        if (level <= 1)
            SceneManager.LoadScene("Level01");
        else if (level == 2)
            SceneManager.LoadScene("Level02");
        else
            SceneManager.LoadScene("Level03");
    }

    // SHOP
    public void Shop()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Shop");
    }

    // OPTIONS OPEN
    public void OpenOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    // OPTIONS CLOSE
    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }

    // CREDITS OPEN
    public void OpenCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    // CREDITS CLOSE
    public void CloseCredits()
    {
        if (creditsPanel != null)
            creditsPanel.SetActive(false);
    }

    // QUIT
    public void Quit()
    {
        Debug.Log("Quitting BLACKSAMURAI...");
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int startingCoins = 0;
    public int Coins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Coins = PlayerPrefs.GetInt("BLACKSAMURAI_Coins", startingCoins);
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        PlayerPrefs.SetInt("BLACKSAMURAI_Coins", Coins);
        PlayerPrefs.Save();

        UIManager.Instance?.RefreshCoins(Coins);
    }

    public void SpendCoins(int amount)
    {
        if (amount <= 0 || Coins < amount) return;

        Coins -= amount;
        PlayerPrefs.SetInt("BLACKSAMURAI_Coins", Coins);
        PlayerPrefs.Save();

        UIManager.Instance?.RefreshCoins(Coins);
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

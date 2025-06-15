using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("In-Game UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText; // NEW: The text to display total coins

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int score = 0;
    private int totalCoins = 0; // NEW: To track coins
    private const string HighScoreKey = "HighScore";
    private const string TotalCoinsKey = "TotalCoins"; // NEW: For saving coins

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        score = 0;
        scoreText.text = "Score: " + score;

        // Load total coins from device memory
        totalCoins = PlayerPrefs.GetInt(TotalCoinsKey, 0);
        UpdateCoinText();
    }

    public void AddPoint()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    // --- NEW Coin Methods ---
    public void AddCoin()
    {
        totalCoins++;
        PlayerPrefs.SetInt(TotalCoinsKey, totalCoins); // Save immediately
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + totalCoins;
        }
    }
    // --- End of New Methods ---

    public int GetCurrentScore()
    {
        return score;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
        }

        PlayerPrefs.Save(); // Save both high score and any coins collected

        finalScoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }
}

using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("In-Game UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText; // Ensure this is assigned in the Inspector

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int score = 0;
    private const string HighScoreKey = "HighScore";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); } else { Instance = this; }
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        score = 0;
        scoreText.text = "Score: " + score;
        UpdateCoinDisplay(); // Update display when the game starts
    }

    // --- NEW Public method to update the coin display ---
    public void UpdateCoinDisplay()
    {
        if (coinText != null && InventoryManager.Instance != null)
        {
            // Always get the latest coin total from the single source of truth: InventoryManager
            coinText.text = "Coins: " + InventoryManager.Instance.GetTotalCoins();
        }
    }

    public void AddPoint()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

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

        PlayerPrefs.Save();

        finalScoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }
}

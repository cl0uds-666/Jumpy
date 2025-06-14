using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("In-Game UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

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
    }

    public void AddPoint()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    // --- NEW PUBLIC FUNCTION ---
    /// <summary>
    /// Returns the current score. Can be called by other scripts like the LevelSpawner.
    /// </summary>
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
            PlayerPrefs.Save();
        }

        finalScoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + highScore;
    }
}

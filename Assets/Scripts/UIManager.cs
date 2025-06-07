using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private int score = 0;
    private int highScore;
    private bool hasBeatenHighScoreThisRun = false; // Flag to ensure sound only plays once
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

        // Load the high score from memory at the start of the game
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        hasBeatenHighScoreThisRun = false; // Reset the flag for the new run
    }

    public void AddPoint()
    {
        score++;
        scoreText.text = "Score: " + score;

        // If we beat the high score and haven't played the sound yet this run...
        if (score > highScore && !hasBeatenHighScoreThisRun)
        {
            hasBeatenHighScoreThisRun = true; // Set the flag so it won't play again
            AudioManager.Instance.PlayHighScoreSound();
        }
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        // Check again at the end of the run and save if necessary
        if (score > highScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, score);
            PlayerPrefs.Save();
        }

        // Update the text fields on the panel
        finalScoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt(HighScoreKey, 0);
    }
}

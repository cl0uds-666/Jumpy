using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro

public class ScoreManager : MonoBehaviour
{
    // This is the singleton instance. Other scripts can access it via ScoreManager.Instance
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText; // Assign your score text object here

    private int score = 0;

    // Awake is called before Start
    void Awake()
    {
        // Set up the singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreText(); // Initialize the text on screen
    }

    public void AddPoint()
    {
        score++;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}

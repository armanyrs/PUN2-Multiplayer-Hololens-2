using UnityEngine;
using TMPro; // Or UnityEngine.UI for old Text
using System.Collections.Generic;

public class ScoreManager : MonoBehaviour
{
    public List<TextMeshProUGUI> scoreText;
    [SerializeField]
    private MainSceneManager mainSceneManager;
    public static ScoreManager instance;

    public int score = 0;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        scoreText.Add(mainSceneManager.getScoreText());
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            //scoreText.text = "Score: " + score.ToString();
            foreach (TextMeshProUGUI text in scoreText)
            {
                text.text = "Score: " + score.ToString();
            }
        }
    }
}
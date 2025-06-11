using TMPro;
using UnityEngine;

public class ScoreTextUpdater : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score;

    private void Update()
    {
        scoreText.text = $"Score: {score}";
    }
}

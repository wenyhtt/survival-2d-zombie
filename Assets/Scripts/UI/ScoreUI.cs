using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private PlayerScore playerScore;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerScore = playerObj.GetComponent<PlayerScore>();
            if (playerScore != null)
            {
                playerScore.OnScoreChanged += UpdateScoreText;
                UpdateScoreText(playerScore.CurrentScore);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerScore != null)
        {
            playerScore.OnScoreChanged -= UpdateScoreText;
        }
    }

    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }
}

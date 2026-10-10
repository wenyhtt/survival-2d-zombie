using UnityEngine;
using TMPro;

/// <summary>
/// Menampilkan skor pemain saat ini di elemen UI teks.
/// Merubahan skor dari komponen PlayerScore.
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private PlayerScore playerScore;

    /// <summary>
    /// Diinisialisasi saat mulai, mencari pemain dan mendaftar ke peristiwa perubahan skor.
    /// </summary>
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

    /// <summary>
    /// Dipanggil saat objek dihancurkan untuk mencegah kebocoran memori.
    /// </summary>
    private void OnDestroy()
    {
        if (playerScore != null)
        {
            playerScore.OnScoreChanged -= UpdateScoreText;
        }
    }

    /// <summary>
    /// Memperbarui teks UI skor dengan nilai skor terbaru.
    /// </summary>
    private void UpdateScoreText(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = $"{score}";
        }
    }
}

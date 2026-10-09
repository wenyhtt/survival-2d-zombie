using UnityEngine;

/// <summary>
/// Menyimpan nilai skor yang diberikan kepada pemain ketika musuh ini berhasil dikalahkan.
/// </summary>
public class EnemyScoreValue : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private int scoreValue = 10;
    
    /// <summary>
    /// Properti untuk mendapatkan nilai skor musuh.
    /// </summary>
    public int ScoreValue => scoreValue;
}

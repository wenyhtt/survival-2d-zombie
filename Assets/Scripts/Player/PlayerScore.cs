using UnityEngine;
using System;

/// <summary>
/// Mengelola sistem penilaian (skor) pemain.
/// Menyediakan fungsi untuk menambah dan mengurangi skor, serta event yang dipicu saat skor berubah.
/// </summary>
public class PlayerScore : MonoBehaviour
{
    private int currentScore;
    [SerializeField] private int startingScore = 0;

    public event Action<int> OnScoreChanged;

    public int CurrentScore => currentScore;

    /// <summary>Dipanggil sebelum pembaruan frame pertama.</summary>
    public void Start()
    {
        currentScore = startingScore;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>Menambahkan skor sebesar jumlah tertentu.</summary>
    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"Score: {currentScore} (+{amount})");
    }

    /// <summary>Mengurangi skor sebesar jumlah tertentu jika cukup.</summary>
    public bool SpendScore(int amount)
    {
        if (currentScore >= amount)
        {
            currentScore -= amount;
            OnScoreChanged?.Invoke(currentScore);
            return true;
        }
        return false;
    }
}

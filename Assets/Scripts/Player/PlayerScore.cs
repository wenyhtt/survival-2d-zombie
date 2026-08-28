using UnityEngine;
using System;

public class PlayerScore : MonoBehaviour
{
    private int currentScore;

    public event Action<int> OnScoreChanged;

    public int CurrentScore => currentScore;

    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"Score: {currentScore} (+{amount})");
    }
}

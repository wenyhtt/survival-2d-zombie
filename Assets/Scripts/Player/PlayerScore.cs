using UnityEngine;
using System;

public class PlayerScore : MonoBehaviour
{
    private int currentScore;
    [SerializeField] private int startingScore = 0;

    public event Action<int> OnScoreChanged;

    public int CurrentScore => currentScore;

    public void Start()
    {
        currentScore = startingScore;
        OnScoreChanged?.Invoke(currentScore);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        OnScoreChanged?.Invoke(currentScore);
        Debug.Log($"Score: {currentScore} (+{amount})");
    }

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

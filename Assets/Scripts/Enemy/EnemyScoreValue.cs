using UnityEngine;

public class EnemyScoreValue : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private int scoreValue = 10;
    public int ScoreValue => scoreValue;
}

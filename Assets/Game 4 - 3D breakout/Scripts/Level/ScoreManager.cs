using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager SP;
    public int CurrentScore { get; private set; }

    void Awake()
    {
        SP = this;
        CurrentScore = 0;
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
    }
}

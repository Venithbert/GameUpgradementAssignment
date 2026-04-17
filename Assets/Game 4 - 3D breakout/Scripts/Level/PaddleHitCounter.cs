using UnityEngine;

// Attach to the same GameObject as Paddle.
// Counts how many times the ball bounces off the paddle.
// After hitsAllowed bounces, triggers level evaluation.
public class PaddleHitCounter : MonoBehaviour
{
    public static PaddleHitCounter SP;
    public int hitsAllowed = 5;

    public int HitsUsed { get; private set; }
    public int HitsRemaining => hitsAllowed - HitsUsed;

    void Awake()
    {
        SP = this;
        HitsUsed = 0;
    }

    public void RegisterHit()
    {
        if (HitsUsed >= hitsAllowed) return;

        HitsUsed++;

        if (HitsRemaining <= 0)
            BreakoutGame.SP.EvaluateLevelEnd();
    }
}

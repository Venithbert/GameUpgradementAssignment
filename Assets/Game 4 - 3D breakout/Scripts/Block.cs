using UnityEngine;

/// <summary>
/// No changes from level-system version.
/// Handles both solid (OnCollisionEnter) and penetrable (OnTriggerEnter) modes.
/// BlockHealth.RefreshCollider() switches between them automatically.
/// </summary>
public class Block : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) HandleHit();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) HandleHit();
    }

    private void HandleHit()
    {
        BlockHealth health      = GetComponent<BlockHealth>();
        int         scoreEarned = health.TakeDamage();
        ScoreManager.SP.AddScore(scoreEarned);

        if (health.Value <= 0)
        {
            BreakoutGame.SP.HitBlock();
            Destroy(gameObject);
        }
    }
}

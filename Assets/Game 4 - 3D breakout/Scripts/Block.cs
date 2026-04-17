using UnityEngine;

// Requires BlockHealth and BlockValueDisplay on the same GameObject.
public class Block : MonoBehaviour
{
    // Normal collision: ball bounces (block value >= 2, isTrigger = false)
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        HandleHit();
    }

    // Trigger: ball passes through (block value == 1, isTrigger = true)
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        HandleHit();
    }

    private void HandleHit()
    {
        BlockHealth health = GetComponent<BlockHealth>();
        int scoreEarned = health.TakeDamage();
        ScoreManager.SP.AddScore(scoreEarned);

        if (health.Value <= 0)
        {
            BreakoutGame.SP.HitBlock();
            Destroy(gameObject);
        }
    }
}

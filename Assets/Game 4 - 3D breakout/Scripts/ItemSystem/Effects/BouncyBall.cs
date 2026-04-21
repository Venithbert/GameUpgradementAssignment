using UnityEngine;

/// <summary>
/// Attach to the BouncyBall prefab. Moves at a constant speed, bounces off anything
/// with a collider (walls, paddle, blocks), halves a block on contact, and self-
/// destructs after maxBounces collisions.
///
/// Setup: Rigidbody (useGravity = false, isKinematic = false), Collider with a
/// PhysicMaterial whose bounciness = 1 and friction = 0 (NoEnergyLoss works),
/// and this script. No physics material? It still works via the manual velocity
/// preservation in Update().
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BouncyBall : MonoBehaviour
{
    public int   maxBounces = 5;
    public float speed      = 18f;
    public float lifeTime   = 10f; // safety cap

    private Rigidbody _rb;
    private int       _bounces;

    void Start()
    {
        _rb             = GetComponent<Rigidbody>();
        _rb.useGravity  = false;
        _rb.isKinematic = false;

        // Launch downward with a slight random angle so it doesn't just bob vertically
        float angleDeg = Random.Range(-30f, 30f);
        float rad      = angleDeg * Mathf.Deg2Rad;
        _rb.linearVelocity = new Vector3(Mathf.Sin(rad) * speed, 0f, -Mathf.Cos(rad) * speed);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Keep it constant-speed so bounces don't drain energy
        Vector3 v   = _rb.linearVelocity;
        v.y         = 0f; // stay on play plane
        float mag   = v.magnitude;
        if (mag > 0.01f) _rb.linearVelocity = v * (speed / mag);
    }

    void OnCollisionEnter(Collision c)
    {
        _bounces++;

        // Halve blocks on contact (but don't double-count the bounce if destroyed)
        BlockHealth block = c.gameObject.GetComponent<BlockHealth>();
        if (block != null && BreakoutGame.SP != null)
            BreakoutGame.SP.HalveBlock(block);

        if (_bounces >= maxBounces) Destroy(gameObject);
    }
}

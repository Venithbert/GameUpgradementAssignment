using UnityEngine;

/// <summary>
/// Attach to the FallingObject prefab.
/// Falls straight down (negative Y). On hitting a block, destroys the block and itself.
/// Self-destructs after lifeTime seconds as a failsafe.
///
/// Setup: Add Rigidbody (useGravity = false), a Collider, and this script to the prefab.
/// Set Physics Material to have 0 bounce so it doesn't ricochet.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 15f;
    public float lifeTime  = 5f;

    void Start()
    {
        Rigidbody rb     = GetComponent<Rigidbody>();
        rb.useGravity    = false;
        rb.linearVelocity = new Vector3(0f, -fallSpeed, 0f);
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        BlockHealth block = collision.gameObject.GetComponent<BlockHealth>();
        if (block != null)
        {
            BreakoutGame.SP.ForceDestroyBlock(collision.gameObject);
            Destroy(gameObject);
        }
    }
}

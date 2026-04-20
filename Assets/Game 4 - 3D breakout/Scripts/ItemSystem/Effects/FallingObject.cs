using UnityEngine;

/// <summary>
/// Attach to the FallingObject prefab.
/// Falls straight down (negative Z). On overlapping a block, halves it and destroys itself.
/// Uses isTrigger so it passes through walls, paddle, and the ball — only blocks are detected.
/// Self-destructs after lifeTime seconds as a failsafe.
///
/// Setup: Add Rigidbody (useGravity = false), a Collider, and this script to the prefab.
/// The collider's Is Trigger flag is set at runtime; leave it unchecked in the prefab if desired.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 15f;
    public float lifeTime  = 5f;

    private bool _hit;

    void Start()
    {
        Rigidbody rb      = GetComponent<Rigidbody>();
        rb.isKinematic    = false;
        rb.useGravity     = false;
        rb.linearVelocity = new Vector3(0f, 0f, -fallSpeed);

        // Trigger = passes through walls, ball, and paddle; OnTriggerEnter filters blocks only
        GetComponent<Collider>().isTrigger = true;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_hit) return; // already claimed a block this frame

        BlockHealth block = other.GetComponent<BlockHealth>();
        if (block == null) return; // ignore ball, walls, paddle

        _hit = true;
        BreakoutGame.SP.HalveBlock(block);
        Destroy(gameObject);
    }
}

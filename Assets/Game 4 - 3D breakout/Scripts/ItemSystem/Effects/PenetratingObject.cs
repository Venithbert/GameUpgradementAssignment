using UnityEngine;

/// <summary>
/// Attach to the PenetratingObject prefab.
/// Passes through blocks via isTrigger, popping each one it overlaps.
/// Self-destructs after 1 second.
///
/// Setup: Add Rigidbody (useGravity = false, isKinematic = true), a Collider (Is Trigger = true),
/// and this script to the prefab.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PenetratingObject : MonoBehaviour
{
    public float fallSpeed = 15f;

    void Start()
    {
        Rigidbody rb      = GetComponent<Rigidbody>();
        rb.useGravity      = false;
        rb.isKinematic     = true; // kinematic + trigger = passes through everything

        GetComponent<Collider>().isTrigger = true;
        Destroy(gameObject, 1f);
    }

    void Update()
    {
        // Move manually since Rigidbody is kinematic
        transform.position += new Vector3(0f, -fallSpeed * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        BlockHealth block = other.GetComponent<BlockHealth>();
        if (block != null)
            BreakoutGame.SP.ForceDestroyBlock(other.gameObject);
    }
}

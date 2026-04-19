using UnityEngine;

/// <summary>
/// Changes from previous version:
///   - OnCollisionEnter: fires TriggerBus.BallHitsWall when hitting a "Wall"-tagged object.
///   - Before destroying: fires TriggerBus.BallDies.
/// Wall objects in the scene must be tagged "Wall".
/// </summary>
public class Ball : MonoBehaviour
{
    public float maxVelocity = 20;
    public float minVelocity = 15;

    void Awake()
    {
        GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, -18);
    }

    void Update()
    {
        // Clamp speed
        float v = GetComponent<Rigidbody>().linearVelocity.magnitude;
        if (v > maxVelocity)
            GetComponent<Rigidbody>().linearVelocity *= maxVelocity / v;
        else if (v < minVelocity)
            GetComponent<Rigidbody>().linearVelocity *= minVelocity / v;

        // Ball fell past the paddle
        if (transform.position.z <= -3)
        {
            TriggerBus.Fire(TriggerType.BallDies);
            BreakoutGame.SP.LostBall();
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
            TriggerBus.Fire(TriggerType.BallHitsWall);
    }
}

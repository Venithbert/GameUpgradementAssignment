using UnityEngine;

public class Paddle : MonoBehaviour
{
    public float moveSpeed       = 15f;
    public float maxBounceAngle  = 60f;
    public float paddleInfluence = 0.4f;

    private float   _paddleVelX;
    private Vector3 _prevPos;
    private bool    _hitMainFace;
    private float   _hitFraction;

    void Awake()
    {
        _prevPos = transform.position;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        transform.position += new Vector3(moveInput, 0, 0);

        float xPos = Mathf.Clamp(transform.position.x, -14f, 14f);
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);

        _paddleVelX = (transform.position.x - _prevPos.x) / Time.deltaTime;
        _prevPos    = transform.position;
    }

    void OnCollisionEnter(Collision c)
    {
        if (!c.gameObject.CompareTag("Player")) return;

        // Determine which face was hit: main bounce face (Z-normal) vs narrow side (X-normal)
        Vector3 normal = c.contacts[0].normal;
        _hitMainFace   = Mathf.Abs(normal.z) > Mathf.Abs(normal.x);

        float halfWidth = GetComponent<Collider>().bounds.extents.x;
        _hitFraction    = Mathf.Clamp(
            (c.rigidbody.position.x - transform.position.x) / halfWidth, -1f, 1f);
    }

    void OnCollisionExit(Collision c)
    {
        if (!c.gameObject.CompareTag("Player")) return;

        Rigidbody rb = c.rigidbody;

        if (_hitMainFace)
        {
            float angle = _hitFraction * maxBounceAngle * Mathf.Deg2Rad;
            float zDir  = rb.linearVelocity.z != 0f ? Mathf.Sign(rb.linearVelocity.z) : 1f;
            float speed = rb.linearVelocity.magnitude;

            rb.linearVelocity = new Vector3(
                Mathf.Sin(angle) * speed + _paddleVelX * paddleInfluence,
                0f,
                zDir * Mathf.Cos(angle) * speed
            );
        }
        else
        {
            // Side hit: let physics keep its direction, just zero Y and blend in paddle motion
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x + _paddleVelX * paddleInfluence,
                0f,
                rb.linearVelocity.z
            );
        }

        PaddleHitCounter.SP.RegisterHit();
        TriggerBus.Fire(TriggerType.BallBounces);
    }
}

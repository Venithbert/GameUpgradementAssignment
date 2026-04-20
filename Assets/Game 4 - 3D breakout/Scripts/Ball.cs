using UnityEngine;
using Object = UnityEngine.Object;

public class Ball : MonoBehaviour
{
    public float maxVelocity   = 22f;
    public float minVelocity   = 18f;
    public bool  launchOnClick = false;

    private Rigidbody _rb;
    private bool      _launched;
    private bool      _pendingLaunch;
    private Transform _paddle;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (!launchOnClick)
        {
            _rb.isKinematic    = false;
            _rb.linearVelocity = new Vector3(0f, 0f, -minVelocity);
            _launched          = true;
            return;
        }

        Paddle p = Object.FindAnyObjectByType<Paddle>();
        if (p != null)
        {
            _paddle = p.transform;
            _rb.MovePosition(new Vector3(_paddle.position.x, transform.position.y, transform.position.z));
        }

        _rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (_rb == null) return; // guard: FixedUpdate can run after Destroy(gameObject)

        // Follow paddle while waiting to launch
        if (!_launched && !_pendingLaunch && _paddle != null)
        {
            _rb.MovePosition(new Vector3(_paddle.position.x, _rb.position.y, _rb.position.z));
            return;
        }

        // Apply launch velocity the frame after isKinematic is disabled
        if (_pendingLaunch)
        {
            _rb.linearVelocity = new Vector3(0f, 0f, -minVelocity);
            _pendingLaunch     = false;
        }
    }

    void Update()
    {
        if (!_launched)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _rb.isKinematic = false;
                _pendingLaunch  = true;
                _launched       = true;
            }
            return;
        }

        // Speed clamp
        float v = _rb.linearVelocity.magnitude;
        if (v > maxVelocity)
            _rb.linearVelocity *= maxVelocity / v;
        else if (v < minVelocity)
            _rb.linearVelocity *= minVelocity / v;

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

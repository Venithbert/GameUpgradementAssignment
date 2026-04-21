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


    //vfx
    public GameObject bounceVfxPrefab;
    public float bounceVfxSurfaceOffset = 0.02f;





    //audio
    public AudioClip[] bounceClips;
    public AudioSource bounceAudioSource;




    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (bounceAudioSource == null)
            bounceAudioSource = GetComponent<AudioSource>();
    }













    void Start()
    {
        if (!launchOnClick)
        {
            _rb.isKinematic    = false;
            _rb.linearVelocity = new Vector3(0f, 0f, -minVelocity);
            _launched          = true;
            TriggerBus.Fire(TriggerType.BallLaunched);
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
                TriggerBus.Fire(TriggerType.BallLaunched);
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
        bool hitWall = collision.gameObject.CompareTag("Wall");
        bool hitPaddle = collision.gameObject.GetComponent<Paddle>() != null;
        bool hitBlock = collision.gameObject.GetComponent<Block>() != null;

        if (hitWall || hitPaddle || hitBlock)
            PlayRandomBounceSound();
            ContactPoint contact = collision.contacts[0];
            SpawnBounceVfx(contact.point, contact.normal);

        if (hitWall)
            TriggerBus.Fire(TriggerType.BallHitsWall);

    }



    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Block>() != null)
            PlayRandomBounceSound();
            SpawnBounceVfxFromTrigger(other);
    }



    void PlayRandomBounceSound()
    {
        if (bounceAudioSource == null) return;
        if (bounceClips == null || bounceClips.Length == 0) return;

        int randomIndex = Random.Range(0, bounceClips.Length);
        bounceAudioSource.PlayOneShot(bounceClips[randomIndex]);
    }




    void SpawnBounceVfx(Vector3 position, Vector3 normal)
    {
        if (bounceVfxPrefab == null) return;

        Vector3 spawnPos = position + normal * bounceVfxSurfaceOffset;
        Quaternion rotation = Quaternion.LookRotation(normal);

        Instantiate(bounceVfxPrefab, spawnPos, rotation);
    }

    void SpawnBounceVfxFromTrigger(Collider other)
    {
        if (bounceVfxPrefab == null) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        Vector3 normal = (transform.position - hitPoint).normalized;

        if (normal == Vector3.zero)
            normal = Vector3.up;

        SpawnBounceVfx(hitPoint, normal);
    }








}

using UnityEngine;

// Requires PaddleHitCounter on the same GameObject.
public class Paddle : MonoBehaviour
{
    public float moveSpeed = 15;

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        transform.position += new Vector3(moveInput, 0, 0);

        float max = 14.0f;
        if (transform.position.x <= -max || transform.position.x >= max)
        {
            float xPos = Mathf.Clamp(transform.position.x, -max, max);
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        // Only count ball (tagged "Player") bounces, not other objects
        if (!collisionInfo.gameObject.CompareTag("Player")) return;

        Rigidbody rigid = collisionInfo.rigidbody;
        float xDistance = rigid.position.x - transform.position.x;
        rigid.linearVelocity = new Vector3(
            rigid.linearVelocity.x + xDistance / 2,
            rigid.linearVelocity.y,
            rigid.linearVelocity.z
        );

        PaddleHitCounter.SP.RegisterHit();
    }
}

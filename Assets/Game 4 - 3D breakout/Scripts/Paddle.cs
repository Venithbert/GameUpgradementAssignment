using UnityEngine;

/// <summary>
/// Changes from previous version:
///   - OnCollisionExit: fires TriggerBus.BallBounces after ball leaves paddle.
/// </summary>
public class Paddle : MonoBehaviour
{
    public float moveSpeed = 15;

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        transform.position += new Vector3(moveInput, 0, 0);

        float max  = 14.0f;
        float xPos = Mathf.Clamp(transform.position.x, -max, max);
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (!collisionInfo.gameObject.CompareTag("Player")) return;

        Rigidbody rigid = collisionInfo.rigidbody;
        float xDist = rigid.position.x - transform.position.x;
        rigid.linearVelocity = new Vector3(
            rigid.linearVelocity.x + xDist / 2,
            rigid.linearVelocity.y,
            rigid.linearVelocity.z
        );

        PaddleHitCounter.SP.RegisterHit();
        TriggerBus.Fire(TriggerType.BallBounces);
    }
}

using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        BreakoutGame.SP.HitBlock();
        Destroy(gameObject);
    }
}
using UnityEngine;
using System.Collections;

public class AsteroidScript : MonoBehaviour {

    public float speed = 5.0f;

    private float maxHeight;

    private SpaceShipGameControllerScript gameController;

    // Use this for initialization
    void Start()
    {
        maxHeight = Camera.main.orthographicSize + 3.0f;
        gameController = FindObjectOfType<SpaceShipGameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0.0f, -speed * Time.deltaTime, 0.0f);

        if (transform.position.y < -maxHeight)
            Destroy(gameObject);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.tag.Equals("Player"))
            gameController.AddScore(1);

        Destroy(gameObject);
    }

}

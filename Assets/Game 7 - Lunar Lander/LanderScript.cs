using UnityEngine;
using System.Collections;

public class LanderScript : MonoBehaviour {

    private LunarLanderGameControllerScript gameController;



	// Use this for initialization
	void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<LunarLanderGameControllerScript>();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Space))
        {
            GetComponent<Rigidbody>().AddForce(transform.up * 1.8f);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, -1.0f));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f));
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Rotate(new Vector3(-1.0f, 0.0f, 0.0f));
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Rotate(new Vector3(1.0f, 0.0f, 0.0f));
        }

        if (GetComponent<Rigidbody>().IsSleeping())
        {
            gameController.LevelSucceeded();
            enabled = false;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        float angle = Vector3.Dot(transform.up, Vector3.up);
        angle = Mathf.Acos(angle);

        if (angle > 1.0f || collision.relativeVelocity.magnitude > 2.0f || collision.gameObject.tag == "Moon")
        {
            gameController.LevelFailed();
            enabled = false;
        }
        
    }

}

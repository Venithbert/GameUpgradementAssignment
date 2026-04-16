using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject lander;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 landerPosition = lander.transform.position;
        Vector3 targetPosition = new Vector3(landerPosition.x, landerPosition.y + 15, landerPosition.z - 25);


        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.8f);


    }
}

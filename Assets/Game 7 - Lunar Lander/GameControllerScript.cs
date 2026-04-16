using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameControllerScript : MonoBehaviour {

    private bool isGameOver;
    private bool levelResult = false;

    private GameObject lunarLander;

    private float speed;

    void Start()
    {
        lunarLander = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        speed = lunarLander.GetComponent<Rigidbody>().linearVelocity.magnitude;
    }

    public void LevelFailed()
    {
        isGameOver = true;
    }

    public void LevelSucceeded()
    {
        isGameOver = true;
        levelResult = true;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), "Speed : " + speed.ToString("0.0"));

        if (isGameOver)
        {

            if (levelResult)
            {
                GUI.Label(new Rect(Camera.main.pixelWidth / 2 - 40, Camera.main.pixelHeight / 2 - 65, 180, 30), "Congratulations!");
            }
            else
            {
                GUI.Label(new Rect(Camera.main.pixelWidth / 2 - 40, Camera.main.pixelHeight / 2 - 65, 180, 30), "You have failed...");
            }

            if (GUI.Button(new Rect(Camera.main.pixelWidth / 2 - 40, Camera.main.pixelHeight / 2 - 15, 80, 30), "Try again"))
            {
                int scene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(scene, LoadSceneMode.Single);
            }
        }

    }

}

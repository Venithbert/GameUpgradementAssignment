using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum BreakoutGameState { playing, won, lost };

public class BreakoutGame : MonoBehaviour
{
    public static BreakoutGame SP;

    public Transform ballPrefab;
    public Transform blockPrefab;
    public int rows = 5;
    public int columns = 8;
    public int blocksToSpawn = 20;
    public Vector3 firstBlockPosition = new Vector3(-12f, 1.0f, -18f);
    public float xSpacing = 3.5f;
    public float zSpacing = 2.5f;

    private int totalBlocks;
    private int blocksHit;
    private BreakoutGameState gameState;




    void Awake()
    {
        SP = this;
        blocksHit = 0;
        gameState = BreakoutGameState.playing;
        Time.timeScale = 1.0f;

        SpawnBlocks();
        SpawnBall();
    }


    void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(1.81f, 1.0f , 9.75f), Quaternion.identity);
    }






    void SpawnBlocks()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 spawnPosition = firstBlockPosition + new Vector3(column * xSpacing, 0, row * zSpacing);
                positions.Add(spawnPosition);
            }
        }

        for (int i = 0; i < positions.Count; i++)
        {
            int randomIndex = Random.Range(i, positions.Count);
            Vector3 temp = positions[i];
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }

        totalBlocks = Mathf.Min(blocksToSpawn, positions.Count);

        for (int i = 0; i < totalBlocks; i++)
        {
            Instantiate(blockPrefab, positions[i], Quaternion.identity);
        }
    }




























    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("  Hit: " + blocksHit + "/" + totalBlocks);

        if (gameState == BreakoutGameState.lost)
        {
            GUILayout.Label("You Lost!");
            if (GUILayout.Button("Try again"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else if (gameState == BreakoutGameState.won)
        {
            GUILayout.Label("You won!");
            if (GUILayout.Button("Play again"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }





    public void HitBlock()
    {
        blocksHit++;
        
        //For fun:
        if (blocksHit%10 == 0) //Every 10th block will spawn a new ball
        {
            SpawnBall();
        }

        
        if (blocksHit >= totalBlocks)
        {
            WonGame();
        }
    }

    public void WonGame()
    {
        Time.timeScale = 0.0f; //Pause game
        gameState = BreakoutGameState.won;
    }

    public void LostBall()
    {
        int ballsLeft = GameObject.FindGameObjectsWithTag("Player").Length;
        if(ballsLeft<=1){
            //Was the last ball..
            SetGameOver();
        }
    }

    public void SetGameOver()
    {
        Time.timeScale = 0.0f; //Pause game
        gameState = BreakoutGameState.lost;
    }
}

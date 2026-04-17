using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;




public enum BreakoutGameState { playing, levelComplete, won, lost }



// Requires LevelManager, ScoreManager, and PaddleHitCounter to be present in the scene.
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
        Instantiate(ballPrefab, new Vector3(1.81f, 1.0f, 5.75f), Quaternion.identity);
    }









    void SpawnBlocks()
    {
        List<Vector3> positions = new List<Vector3>();

        for (int row = 0; row < rows; row++)
            for (int col = 0; col < columns; col++)
                positions.Add(firstBlockPosition + new Vector3(col * xSpacing, 0, row * zSpacing));

        // Shuffle positions
        for (int i = 0; i < positions.Count; i++)
        {
            int r = Random.Range(i, positions.Count);
            Vector3 tmp = positions[i];
            positions[i] = positions[r];
            positions[r] = tmp;
        }

        totalBlocks = Mathf.Min(blocksToSpawn, positions.Count);
        LevelConfig level = LevelManager.SP.CurrentLevel;

        for (int i = 0; i < totalBlocks; i++)
        {
            GameObject block = Instantiate(blockPrefab, positions[i], Quaternion.identity).gameObject;
            block.GetComponent<BlockHealth>().SetValue(level.GetRandomBlockValue());
        }
    }





    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("  Level: " + LevelManager.CurrentLevelNumber);
        GUILayout.Label("  Score: " + ScoreManager.SP.CurrentScore + " / " + LevelManager.SP.CurrentLevel.requiredScore);
        GUILayout.Label("  Hits Left: " + PaddleHitCounter.SP.HitsRemaining);
        GUILayout.Label("  Blocks: " + blocksHit + " / " + totalBlocks);

        if (gameState == BreakoutGameState.levelComplete)
        {
            GUILayout.Label("Level Complete!");
            if (GUILayout.Button("Next Level"))
                LevelManager.SP.AdvanceLevel();
        }
        else if (gameState == BreakoutGameState.won)
        {
            GUILayout.Label("You Won!");
            if (GUILayout.Button("Play Again"))
                LevelManager.SP.RestartFromLevel1();
        }
        else if (gameState == BreakoutGameState.lost)
        {
            GUILayout.Label("You Lost!");
            if (GUILayout.Button("Try Again"))
                LevelManager.SP.RestartFromLevel1();
        }
    }







    // Called by Block when fully destroyed (value reaches 0)
    public void HitBlock()
    {
        blocksHit++;

        // Every 10 blocks: bonus ball
        if (blocksHit % 10 == 0)
            SpawnBall();
    }

    // Called by PaddleHitCounter when 5 hits are used up
    public void EvaluateLevelEnd()
    {
        Time.timeScale = 0f;

        bool passed = ScoreManager.SP.CurrentScore >= LevelManager.SP.CurrentLevel.requiredScore;

        if (passed)
            gameState = LevelManager.SP.IsLastLevel() ? BreakoutGameState.won : BreakoutGameState.levelComplete;
        else
            gameState = BreakoutGameState.lost;
    }






    // Called by Ball when it falls
    public void LostBall()
    {
        int ballsLeft = GameObject.FindGameObjectsWithTag("Player").Length;
        if (ballsLeft <= 1)
            SetGameOver();
    }




    public void SetGameOver()
    {
        Time.timeScale = 0f;
        gameState = BreakoutGameState.lost;
    }
}
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum BreakoutGameState { playing, levelComplete, itemPicking, won, lost }

/// <summary>
/// Changes from level-system version:
///   - HitBlock(): fires TriggerBus.BlockPopped on every block destruction.
///   - ForceDestroyBlock(): new — used by item effects to destroy blocks externally.
///   - EvaluateLevelEnd(): fires TriggerBus.PassLevelGoal on pass; shows item picker every 3 levels.
///   - levelEndEvaluated guard prevents double-calling EvaluateLevelEnd.
///   - LevelManager.RestartFromLevel1() now also calls PlayerInventory.ClearInventory().
/// </summary>
public class BreakoutGame : MonoBehaviour
{
    public static BreakoutGame SP;

    public Transform ballPrefab;
    public Transform blockPrefab;
    public int       rows            = 5;
    public int       columns         = 8;
    public int       blocksToSpawn   = 20;
    public Vector3   firstBlockPosition = new Vector3(-12f, 1.0f, -18f);
    public float     xSpacing        = 3.5f;
    public float     zSpacing        = 2.5f;

    private int               totalBlocks;
    private int               blocksHit;
    private BreakoutGameState gameState;
    private bool              levelEndEvaluated;

    // ── Lifecycle ────────────────────────────────────────────────────

    void Awake()
    {
        SP                 = this;
        blocksHit          = 0;
        levelEndEvaluated  = false;
        gameState          = BreakoutGameState.playing;
        Time.timeScale     = 1.0f;

   
    }


    void Start()
    {
        SpawnBlocks();
        SpawnBall(isInitial: true);
    }



    // ── Spawning ─────────────────────────────────────────────────────

    void SpawnBall(bool isInitial = false)
    {
        var go = Instantiate(ballPrefab, new Vector3(1.81f, 1.0f, -1.3f), Quaternion.identity).gameObject;
        if (isInitial)
            go.GetComponent<Ball>().launchOnClick = true;
    }

    void SpawnBlocks()
    {
        var positions = new List<Vector3>();
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < columns; col++)
                positions.Add(firstBlockPosition + new Vector3(col * xSpacing, 0, row * zSpacing));

        // Shuffle
        for (int i = 0; i < positions.Count; i++)
        {
            int r = Random.Range(i, positions.Count);
            (positions[i], positions[r]) = (positions[r], positions[i]);
        }

        totalBlocks       = Mathf.Min(blocksToSpawn, positions.Count);
        LevelConfig level = LevelManager.SP.CurrentLevel;

        for (int i = 0; i < totalBlocks; i++)
        {
            var block = Instantiate(blockPrefab, positions[i], Quaternion.identity).gameObject;
            block.GetComponent<BlockHealth>().SetValue(level.GetRandomBlockValue());
        }
    }

    // ── Public API used by Block, Ball, Paddle, and item effects ─────

    /// <summary>
    /// Called when a block is destroyed by the ball (via Block.cs).
    /// Fires BlockPopped trigger, handles bonus ball, checks if all blocks cleared.
    /// </summary>
    public void HitBlock()
    {
        blocksHit++;
        TriggerBus.Fire(TriggerType.BlockPopped);

        if (blocksHit % 10 == 0) SpawnBall();              // bonus ball every 10
        if (blocksHit >= totalBlocks) EvaluateLevelEnd();  // all blocks cleared
    }

    /// <summary>
    /// Called by item effects to destroy a block externally (not via ball collision).
    /// Awards full block value as score, counts as a block hit.
    /// </summary>
    public void ForceDestroyBlock(GameObject blockObj)
    {
        if (blockObj == null) return;
        BlockHealth h = blockObj.GetComponent<BlockHealth>();
        if (h != null) ScoreManager.SP.AddScore(h.Value);
        HitBlock();
        Destroy(blockObj);
    }

    /// <summary>
    /// Called by PaddleHitCounter after 5 paddle hits, and by HitBlock when all blocks cleared.
    /// Determines pass/fail, triggers item picker every 3 levels.
    /// </summary>
    public void EvaluateLevelEnd()
    {
        if (levelEndEvaluated) return;
        levelEndEvaluated = true;

        bool passed = ScoreManager.SP.CurrentScore >= LevelManager.SP.CurrentLevel.requiredScore;

        if (!passed)
        {
            Time.timeScale = 0f;
            gameState      = BreakoutGameState.lost;
            return;
        }

        TriggerBus.Fire(TriggerType.PassLevelGoal);

        if (LevelManager.SP.IsLastLevel())
        {
            Time.timeScale = 0f;
            gameState      = BreakoutGameState.won;
            return;
        }

        Time.timeScale = 0f;

        // Every 3rd level: pause and show item picker before advancing
        if (LevelManager.CurrentLevelNumber % 3 == 0)
        {
            gameState = BreakoutGameState.itemPicking;
            ItemPickerUI.SP.Show();
        }
        else
        {
            gameState = BreakoutGameState.levelComplete;
        }
    }

    public void LostBall()
    {
        int ballsLeft = GameObject.FindGameObjectsWithTag("Player").Length;
        if (ballsLeft <= 1) EvaluateLevelEnd();
    }

    public void HalveBlock(BlockHealth h)
    {
        if (h == null) return;
        int score = h.TakeDamage();
        ScoreManager.SP.AddScore(score);
        if (h.Value <= 0)
        {
            HitBlock();
            Destroy(h.gameObject);
        }
    }

    public void SetGameOver()
    {
        levelEndEvaluated = true;
        Time.timeScale    = 0f;
        gameState         = BreakoutGameState.lost;
    }

    public void WonGame()
    {
        Time.timeScale = 0f;
        gameState      = BreakoutGameState.won;
    }

    // ── Legacy HUD (replace with UI Toolkit HUDController) ───────────

    void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("  Level:      " + LevelManager.CurrentLevelNumber);
        GUILayout.Label("  Score:      " + ScoreManager.SP.CurrentScore
                                         + " / " + LevelManager.SP.CurrentLevel.requiredScore);
        GUILayout.Label("  Hits Left:  " + PaddleHitCounter.SP.HitsRemaining);
        GUILayout.Label("  Blocks:     " + blocksHit + " / " + totalBlocks);

        if (gameState == BreakoutGameState.levelComplete)
        {
            GUILayout.Label("Level Complete!");
            if (GUILayout.Button("Next Level")) LevelManager.SP.AdvanceLevel();
        }
        else if (gameState == BreakoutGameState.won)
        {
            GUILayout.Label("You Won!");
            if (GUILayout.Button("Play Again")) LevelManager.SP.RestartFromLevel1();
        }
        else if (gameState == BreakoutGameState.lost)
        {
            GUILayout.Label("You Lost!");
            if (GUILayout.Button("Try Again")) LevelManager.SP.RestartFromLevel1();
        }
        // itemPicking state is rendered entirely by ItemPickerUI
    }
}

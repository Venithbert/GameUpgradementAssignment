using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum BreakoutGameState { playing, levelComplete, itemPicking, relicPicking, won, lost }

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

        int lvl = LevelManager.CurrentLevelNumber;

        // Items: every 3rd level (3, 6, 9, 12, ...)
        if (lvl % 3 == 0)
        {
            gameState = BreakoutGameState.itemPicking;
            ItemPickerUI.SP.Show();
        }
        // Relics: starting at level 4, then every 3 levels (4, 7, 10, 13, ...)
        else if (lvl >= 4 && lvl % 3 == 1)
        {
            gameState = BreakoutGameState.relicPicking;
            RelicPickerUI.SP.Show();
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

    // ── End-of-level dialog (stats HUD lives in HudController) ───────

    void OnGUI()
    {
        if (gameState == BreakoutGameState.levelComplete)
        {
            CenteredDialog("Level Complete!", "Next Level",  LevelManager.SP.AdvanceLevel);
        }
        else if (gameState == BreakoutGameState.won)
        {
            CenteredDialog("You Won!",       "Play Again",  LevelManager.SP.RestartFromLevel1);
        }
        else if (gameState == BreakoutGameState.lost)
        {
            CenteredDialog("You Lost!",      "Try Again",   LevelManager.SP.RestartFromLevel1);
        }
        // itemPicking / relicPicking are rendered by their picker UIs
    }

    void CenteredDialog(string title, string button, System.Action onClick)
    {
        float s = Mathf.Min(Screen.width / 1366f, Screen.height / 768f);
        float w = 260f * s, h = 110f * s;
        Rect box = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
        GUI.Box(box, title);
        if (GUI.Button(new Rect(box.x + 30 * s, box.y + 50 * s, w - 60 * s, 40 * s), button)) onClick();
    }
}

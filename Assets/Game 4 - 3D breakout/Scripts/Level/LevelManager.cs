using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager SP;
    public static int CurrentLevelNumber = 1; // static = persists across scene reloads
    public static int MaxLevels = 5;

    public LevelConfig CurrentLevel { get; private set; }

    void Awake()
    {
        SP = this;
        CurrentLevel = LevelConfig.Create(CurrentLevelNumber);
    }

    public void AdvanceLevel()
    {
        CurrentLevelNumber++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartFromLevel1()
    {
        CurrentLevelNumber = 1;
        PlayerInventory.ClearInventory(); // wipe items on full restart
        RelicInventory.ClearRelics();     // wipe relics on full restart
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsLastLevel() => false; // infinite levels — no end
}

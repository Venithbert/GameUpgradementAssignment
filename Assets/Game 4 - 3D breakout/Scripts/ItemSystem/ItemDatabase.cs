using UnityEngine;

/// <summary>
/// Single place to register every item in the game.
/// To add a new item: add an entry to the allItems array in Awake().
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase SP;

    private ItemDefinition[] allItems;

    void Awake()
    {
        SP = this;

        allItems = new[]
        {
            new ItemDefinition {
                itemName       = "Pulse Coil",
                description    = "Every 1.5s → Doubles a random block's value.",
                triggerType    = TriggerType.EveryXSeconds,
                triggerInterval = 1.5f,
                effect         = new EffectDoubleRandomBlock()
            },
            new ItemDefinition {
                itemName    = "Ricochet",
                description = "Ball hits wall → Instantly pops 1 random block.",
                triggerType = TriggerType.BallHitsWall,
                effect      = new EffectPopRandomBlock()
            },
            new ItemDefinition {
                itemName    = "Chain Reaction",
                description = "Ball bounces → A random block explodes (up to 8 nearby).",
                triggerType = TriggerType.BallBounces,
                effect      = new EffectExplodeRandomBlock()
            },
            new ItemDefinition {
                itemName         = "Sky Caller",
                description      = "Every 5 blocks → A falling object drops and pops 1 block.",
                triggerType      = TriggerType.NthBlockPopped,
                triggerThreshold = 5,
                effect           = new EffectSummonFallingObject()
            }
        };
    }

    public ItemDefinition[] GetAllItems() => allItems;

    /// <summary>Returns `count` randomly shuffled items from the pool.</summary>
    public ItemDefinition[] GetRandomSelection(int count)
    {
        var copy = (ItemDefinition[])allItems.Clone();
        for (int i = 0; i < copy.Length; i++)
        {
            int r = Random.Range(i, copy.Length);
            (copy[i], copy[r]) = (copy[r], copy[i]);
        }
        int take   = Mathf.Min(count, copy.Length);
        var result = new ItemDefinition[take];
        for (int i = 0; i < take; i++) result[i] = copy[i];
        return result;
    }
}

using UnityEngine;

/// <summary>
/// Single place to register every item in the game.
/// To add a new item: add an entry to allItems in Awake() AND expand itemPreviewPrefabs in the Inspector.
/// </summary>
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase SP;

    /// <summary>
    /// Drag a prefab here for each item (same order as the allItems array below).
    /// Leave a slot empty → placeholder cube is used for that item.
    /// </summary>
    [Header("Preview prefabs — one per item, same order as allItems in Awake")]
    public GameObject[] itemPreviewPrefabs;

    private ItemDefinition[] allItems;

    void Awake()
    {
        SP = this;

        allItems = new[]
        {
            new ItemDefinition {
                itemName        = "Barrel",
                description     = "Every 1.5s → Doubles a random block's value.",
                triggerType     = TriggerType.EveryXSeconds,
                triggerInterval = 1.5f,
                effect          = new EffectDoubleRandomBlock()
            },
            new ItemDefinition {
                itemName    = "Ricochet",
                description = "Ball hits wall → Instantly halves 1 random block.",
                triggerType = TriggerType.BallHitsWall,
                effect      = new EffectPopRandomBlock()
            },
            new ItemDefinition {
                itemName    = "Broom",
                description = "Ball bounces → A random block explodes (up to 8 nearby).",
                triggerType = TriggerType.BallBounces,
                effect      = new EffectExplodeRandomBlock()
            },
            new ItemDefinition {
                itemName         = "Sky Caller",
                description      = "Every 3 ball hits → A falling object drops and halves 1 block.",
                triggerType      = TriggerType.NthBallHitBlock,
                triggerThreshold = 3,
                effect           = new EffectSummonFallingObject()
            },
            new ItemDefinition {
                itemName    = "Sword",
                description = "First block halved → 50% chance to double the value of EVERY block.",
                triggerType = TriggerType.FirstBlockHalved,
                effect      = new EffectDoubleAllBlocksChance()
            },
            new ItemDefinition {
                itemName        = "Envy",
                description     = "Every 1.5s → The lowest-value block becomes a copy of the highest.",
                triggerType     = TriggerType.EveryXSeconds,
                triggerInterval = 1.5f,
                effect          = new EffectCopyHighestToLowest()
            },
            new ItemDefinition {
                itemName    = "Pumpkin",
                description = "Ball launched → Mutates into the item to my LEFT for the rest of the level.",
                triggerType = TriggerType.BallLaunched,
                effect      = new EffectMutateToLeftItem()
            },
            new ItemDefinition {
                itemName         = "Blue Skull",
                description      = "Every 5 block halves → Spawns a bouncy ball from the top (5 bounces, then vanishes).",
                triggerType      = TriggerType.NthBallHitBlock,
                triggerThreshold = 5,
                effect           = new EffectSummonBouncyBall()
            },
            new ItemDefinition {
                itemName    = "Lucky Strike",
                description = "Block halved → 25% chance to double one random block.",
                triggerType = TriggerType.BallHitsBlock,
                effect      = new EffectDoubleRandomBlockChance()
            }
        };

        if (itemPreviewPrefabs != null)
            for (int i = 0; i < allItems.Length && i < itemPreviewPrefabs.Length; i++)
                allItems[i].previewPrefab = itemPreviewPrefabs[i];
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

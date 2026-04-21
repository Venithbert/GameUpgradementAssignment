using UnityEngine;

/// <summary>
/// Central relic registry. Mirrors ItemDatabase but for relics (unbounded inventory).
/// To add a new relic: append an entry to the allRelics array in Awake().
/// </summary>
public class RelicDatabase : MonoBehaviour
{
    public static RelicDatabase SP;

    /// <summary>
    /// Drag a prefab here for each relic (same order as the allRelics array below).
    /// Leave a slot empty → placeholder cube is used for that relic.
    /// </summary>
    [Header("Preview prefabs — one per relic, same order as allRelics in Awake")]
    public GameObject[] relicPreviewPrefabs;

    private RelicDefinition[] allRelics;

    void Awake()
    {
        SP = this;

        allRelics = new[]
        {
            new RelicDefinition {
                relicName   = "Last Breath",
                description = "Ball dies → 50% chance to force-trigger the right-most item.",
                triggerType = TriggerType.BallDies,
                effect      = new RelicForceTriggerRightmost { chance = 0.5f }
            },
            new RelicDefinition {
                relicName   = "Third Eye",
                description = "Block popped → 40% chance to force-trigger item in slot #3.",
                triggerType = TriggerType.BlockPopped,
                effect      = new RelicForceTriggerSlot { slotIndex = 2, chance = 0.4f }
            },
            new RelicDefinition {
                relicName       = "Metronome",
                description     = "Every 3s → Force-triggers ALL items.",
                triggerType     = TriggerType.EveryXSeconds,
                triggerInterval = 3f,
                effect          = new RelicForceTriggerAllItems()
            },
            new RelicDefinition {
                relicName   = "Genesis",
                description = "First block popped → Force-triggers all items in odd-numbered slots (#1, #3, #5, #7).",
                triggerType = TriggerType.FirstBlockPopped,
                effect      = new RelicForceTriggerOddSlots()
            },
            new RelicDefinition {
                relicName       = "Dice",
                description     = "Every 1s → Force-triggers one random item.",
                triggerType     = TriggerType.EveryXSeconds,
                triggerInterval = 1f,
                effect          = new RelicForceTriggerRandomItem()
            },
            new RelicDefinition {
                relicName   = "Lottery",
                description = "Ball bounces → 5% chance to force-trigger item in slot #5.",
                triggerType = TriggerType.BallBounces,
                effect      = new RelicForceTriggerSlot { slotIndex = 4, chance = 0.05f }
            },
            new RelicDefinition {
                relicName   = "Echo",
                description = "Pass level → Force-triggers item in slot #3, three times.",
                triggerType = TriggerType.PassLevelGoal,
                effect      = new RelicForceTriggerSlot { slotIndex = 2, chance = 1f, times = 3 }
            }
        };

        if (relicPreviewPrefabs != null)
            for (int i = 0; i < allRelics.Length && i < relicPreviewPrefabs.Length; i++)
                allRelics[i].previewPrefab = relicPreviewPrefabs[i];
    }

    public RelicDefinition[] GetAllRelics() => allRelics;

    /// <summary>Returns `count` randomly shuffled relics for the picker UI.</summary>
    public RelicDefinition[] GetRandomSelection(int count)
    {
        var copy = (RelicDefinition[])allRelics.Clone();
        for (int i = 0; i < copy.Length; i++)
        {
            int r = Random.Range(i, copy.Length);
            (copy[i], copy[r]) = (copy[r], copy[i]);
        }
        int take = Mathf.Min(count, copy.Length);
        var result = new RelicDefinition[take];
        for (int i = 0; i < take; i++) result[i] = copy[i];
        return result;
    }
}

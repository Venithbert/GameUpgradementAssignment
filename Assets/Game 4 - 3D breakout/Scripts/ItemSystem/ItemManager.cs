using UnityEngine;

/// <summary>
/// Bridges TriggerBus events to the effects of items currently in PlayerInventory.
/// Handles derived trigger types (FirstBlockPopped, NthBlockPopped, EveryXSeconds).
/// Reset level counters by calling ResetLevelCounters() at the start of each level.
/// </summary>
public class ItemManager : MonoBehaviour
{
    public static ItemManager SP;

    private float[] timers          = new float[PlayerInventory.SlotCount];
    private int     blockPoppedCount = 0;
    private bool    firstBlockFired  = false;

    // ── Lifecycle ────────────────────────────────────────────────────

    void Awake()
    {
        SP = this;
        ResetLevelCounters();
        TriggerBus.OnTrigger += HandleTrigger;
    }

    void OnDestroy() => TriggerBus.OnTrigger -= HandleTrigger;

    public void ResetLevelCounters()
    {
        blockPoppedCount = 0;
        firstBlockFired  = false;
        for (int i = 0; i < timers.Length; i++) timers[i] = 0f;
    }

    // ── Timer-based triggers (EveryXSeconds) ─────────────────────────

    void Update()
    {
        if (Time.timeScale == 0f) return;

        var slots = PlayerInventory.SP.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            var item = slots[i];
            if (item == null || item.triggerType != TriggerType.EveryXSeconds) continue;

            timers[i] += Time.deltaTime;
            if (timers[i] >= item.triggerInterval)
            {
                timers[i] = 0f;
                item.effect.Execute();
            }
        }
    }

    // ── TriggerBus handler ───────────────────────────────────────────

    private void HandleTrigger(TriggerType type)
    {
        if (Time.timeScale == 0f) return;

        // BlockPopped drives two derived trigger types; don't pass it to items directly.
        if (type == TriggerType.BlockPopped)
        {
            blockPoppedCount++;
            HandleBlockPoppedDerived();
            return;
        }

        FireMatchingSlots(type);
    }

    /// <summary>Resolves FirstBlockPopped and NthBlockPopped from raw BlockPopped count.</summary>
    private void HandleBlockPoppedDerived()
    {
        var slots = PlayerInventory.SP.GetSlots();

        // FirstBlockPopped: fires exactly once per level
        if (!firstBlockFired)
        {
            firstBlockFired = true;
            foreach (var item in slots)
                if (item != null && item.triggerType == TriggerType.FirstBlockPopped)
                    item.effect.Execute();
        }

        // NthBlockPopped: fires every time count hits a multiple of threshold
        foreach (var item in slots)
        {
            if (item == null || item.triggerType != TriggerType.NthBlockPopped) continue;
            if (blockPoppedCount % item.triggerThreshold == 0)
                item.effect.Execute();
        }
    }

    private void FireMatchingSlots(TriggerType type)
    {
        foreach (var item in PlayerInventory.SP.GetSlots())
            if (item != null && item.triggerType == type)
                item.effect.Execute();
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bridges TriggerBus events to the effects of relics in RelicInventory.
/// Mirrors ItemManager but uses a dynamic-sized timer list (relics are unbounded).
/// Force-triggering does NOT recursively re-fire trigger bus events.
/// </summary>
public class RelicManager : MonoBehaviour
{
    public static RelicManager SP;

    private List<float> timers = new List<float>();
    private bool        firstBlockFired = false;

    void Awake()
    {
        SP = this;
        ResetLevelCounters();
        TriggerBus.OnTrigger += HandleTrigger;
    }

    void OnDestroy() => TriggerBus.OnTrigger -= HandleTrigger;

    public void ResetLevelCounters()
    {
        firstBlockFired = false;
        timers.Clear();
    }

    // ── Timer-based triggers (EveryXSeconds) ─────────────────────────

    void Update()
    {
        if (Time.timeScale == 0f) return;

        var relics = RelicInventory.SP.GetAll();

        // Keep timers list aligned with relic count
        while (timers.Count < relics.Count) timers.Add(0f);

        for (int i = 0; i < relics.Count; i++)
        {
            RelicDefinition r = relics[i];
            if (r == null || r.triggerType != TriggerType.EveryXSeconds) continue;

            timers[i] += Time.deltaTime;
            if (timers[i] >= r.triggerInterval)
            {
                timers[i] = 0f;
                r.effect.Execute();
                FireSlotVfx(i);
            }
        }
    }

    // ── TriggerBus handler ───────────────────────────────────────────

    private void HandleTrigger(TriggerType type)
    {
        if (Time.timeScale == 0f) return;

        // FirstBlockPopped is not emitted on the bus — derive from BlockPopped here
        if (type == TriggerType.BlockPopped)
        {
            if (!firstBlockFired)
            {
                firstBlockFired = true;
                FireMatching(TriggerType.FirstBlockPopped);
            }
            FireMatching(TriggerType.BlockPopped);
            return;
        }

        FireMatching(type);
    }

    private void FireMatching(TriggerType type)
    {
        var slots = RelicInventory.SP.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            RelicDefinition r = slots[i];
            if (r != null && r.triggerType == type)
            {
                r.effect.Execute();
                FireSlotVfx(i);
            }
        }
    }

    private static void FireSlotVfx(int slotIndex)
    {
        if (HudController.SP != null)
            HudController.SP.PlayRelicSlotVfx(slotIndex);
    }
}

using UnityEngine;

/// <summary>
/// Force-triggers the item in a specific inventory slot.
/// - slotIndex is 0-based (slot #3 in UI = slotIndex 2).
/// - chance gates the activation (0..1).
/// - times repeats the Execute() call on success.
/// </summary>
public class RelicForceTriggerSlot : RelicEffect
{
    public int   slotIndex = 0;
    [Range(0f, 1f)] public float chance = 1f;
    public int   times     = 1;

    public override void Execute()
    {
        if (Random.value > chance) return;

        var slots = PlayerInventory.SP.GetSlots();
        if (slotIndex < 0 || slotIndex >= slots.Length) return;

        ItemDefinition item = slots[slotIndex];
        if (item == null || item.effect == null) return;

        for (int i = 0; i < times; i++)
            item.effect.Execute();
    }
}

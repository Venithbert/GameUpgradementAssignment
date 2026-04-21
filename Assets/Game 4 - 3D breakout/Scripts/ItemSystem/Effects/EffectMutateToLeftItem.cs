using UnityEngine;

/// <summary>
/// On ball launch: swaps the inventory slot holding this effect with a copy of
/// its LEFT NEIGHBOR's ItemDefinition. The host effectively becomes that item
/// for the rest of the level. Call RestoreMutation() at level reset to revert.
///
/// - Mutates at most ONCE per level (guarded by _mutatedSlot).
/// - Does nothing if the host is in slot 0 (no left neighbor) or if the left slot is empty.
/// - "Non-upgraded version": the base item effect is used directly (no upgrade tiers exist yet).
/// </summary>
public class EffectMutateToLeftItem : ItemEffect
{
    private static int            _mutatedSlot  = -1;
    private static ItemDefinition _originalItem = null;

    public override void Execute()
    {
        if (_mutatedSlot != -1) return; // already mutated this level

        var slots = PlayerInventory.SP.GetSlots();

        // Find the slot holding this effect instance
        int myIndex = -1;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].effect == this) { myIndex = i; break; }
        }
        if (myIndex <= 0) return; // not found, or no left neighbor

        ItemDefinition left = slots[myIndex - 1];
        if (left == null) return;

        // Snapshot the original so we can restore at level reset
        _originalItem = slots[myIndex];
        _mutatedSlot  = myIndex;

        // Replace with a shallow clone of the left item (reuses the same effect instance)
        slots[myIndex] = new ItemDefinition
        {
            itemName         = left.itemName,
            description      = left.description,
            triggerType      = left.triggerType,
            triggerInterval  = left.triggerInterval,
            triggerThreshold = left.triggerThreshold,
            effect           = left.effect
        };
    }

    /// <summary>Called by ItemManager.ResetLevelCounters to undo the mutation at level end.</summary>
    public static void RestoreMutation()
    {
        if (_mutatedSlot == -1) return;

        var slots = PlayerInventory.SP.GetSlots();
        if (_mutatedSlot < slots.Length)
            slots[_mutatedSlot] = _originalItem;

        _mutatedSlot  = -1;
        _originalItem = null;
    }
}

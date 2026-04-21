using UnityEngine;

/// <summary>
/// Force-triggers the RIGHT-MOST non-empty item slot in PlayerInventory.
/// </summary>
public class RelicForceTriggerRightmost : RelicEffect
{
    [Range(0f, 1f)] public float chance = 1f;

    public override void Execute()
    {
        if (Random.value > chance) return;

        var slots = PlayerInventory.SP.GetSlots();
        for (int i = slots.Length - 1; i >= 0; i--)
        {
            ItemDefinition item = slots[i];
            if (item != null && item.effect != null)
            {
                item.effect.Execute();
                return;
            }
        }
    }
}

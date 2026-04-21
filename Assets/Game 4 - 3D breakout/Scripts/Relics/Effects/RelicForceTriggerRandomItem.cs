using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Force-triggers exactly one non-empty item slot, picked uniformly at random.
/// </summary>
public class RelicForceTriggerRandomItem : RelicEffect
{
    public override void Execute()
    {
        var slots = PlayerInventory.SP.GetSlots();
        List<ItemDefinition> nonEmpty = new List<ItemDefinition>();
        foreach (ItemDefinition it in slots)
            if (it != null && it.effect != null)
                nonEmpty.Add(it);

        if (nonEmpty.Count == 0) return;

        nonEmpty[Random.Range(0, nonEmpty.Count)].effect.Execute();
    }
}

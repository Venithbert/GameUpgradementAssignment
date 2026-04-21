using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Finds the block with the lowest Value and sets its Value equal to the highest Value currently in play.
/// If they're already equal (all blocks same value), does nothing.
/// </summary>
public class EffectCopyHighestToLowest : ItemEffect
{
    public override void Execute()
    {
        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length < 2) return;

        BlockHealth lowest  = blocks[0];
        BlockHealth highest = blocks[0];
        foreach (BlockHealth b in blocks)
        {
            if (b == null) continue;
            if (b.Value < lowest.Value)  lowest  = b;
            if (b.Value > highest.Value) highest = b;
        }

        if (lowest == highest)            return; // all equal
        if (lowest.Value == highest.Value) return; // tie

        lowest.SetValue(highest.Value);
    }
}

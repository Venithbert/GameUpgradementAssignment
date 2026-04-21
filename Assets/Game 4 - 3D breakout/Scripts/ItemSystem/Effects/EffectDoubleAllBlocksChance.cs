using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 50% chance to double every block's value.
/// Used by the "First Block Halved" item — high risk, high reward payoff.
/// </summary>
public class EffectDoubleAllBlocksChance : ItemEffect
{
    [Range(0f, 1f)] public float chance = 0.5f;

    public override void Execute()
    {
        if (Random.value > chance) return;

        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        foreach (BlockHealth b in blocks)
            if (b != null) b.SetValue(b.Value * 2, true);
    }
}

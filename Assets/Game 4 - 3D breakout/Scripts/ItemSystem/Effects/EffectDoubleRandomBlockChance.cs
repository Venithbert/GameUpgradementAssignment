using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Chance-based variant of EffectDoubleRandomBlock. Rolls `chance` per call;
/// on success, doubles one random block's value.
/// </summary>
public class EffectDoubleRandomBlockChance : ItemEffect
{
    [Range(0f, 1f)] public float chance = 0.25f;

    public override void Execute()
    {
        if (Random.value > chance) return;

        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        BlockHealth target = blocks[Random.Range(0, blocks.Length)];
        target.SetValue(target.Value * 2, true);
    }
}

using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>Instantly destroys one random block in the scene.</summary>
public class EffectPopRandomBlock : ItemEffect
{
    public override void Execute()
    {
        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        BlockHealth target = blocks[Random.Range(0, blocks.Length)];
        BreakoutGame.SP.HalveBlock(target);
    }
}

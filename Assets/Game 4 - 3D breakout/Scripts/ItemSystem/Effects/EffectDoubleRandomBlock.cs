using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>Doubles the value of one random block (e.g. 4 → 8).</summary>
public class EffectDoubleRandomBlock : ItemEffect
{
    public override void Execute()
    {
        BlockHealth[] blocks = Object.FindObjectsOfType<BlockHealth>();
        if (blocks.Length == 0) return;

        BlockHealth target = blocks[Random.Range(0, blocks.Length)];
        target.SetValue(target.Value * 2);
    }
}

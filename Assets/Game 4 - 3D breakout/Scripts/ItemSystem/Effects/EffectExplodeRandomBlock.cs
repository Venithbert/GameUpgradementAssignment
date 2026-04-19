using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Picks a random block as the epicenter.
/// Destroys up to MaxKills blocks within ExplodeRadius, including the epicenter.
/// </summary>
public class EffectExplodeRandomBlock : ItemEffect
{
    private const float ExplodeRadius = 7f;
    private const int   MaxKills      = 8;

    public override void Execute()
    {
        BlockHealth[] blocks = Object.FindObjectsOfType<BlockHealth>();
        if (blocks.Length == 0) return;

        Vector3 epicenter = blocks[Random.Range(0, blocks.Length)].transform.position;

        int killed = 0;
        foreach (BlockHealth b in blocks)
        {
            if (killed >= MaxKills) break;
            if (b == null) continue; // already destroyed this frame
            if (Vector3.Distance(b.transform.position, epicenter) <= ExplodeRadius)
            {
                BreakoutGame.SP.ForceDestroyBlock(b.gameObject);
                killed++;
            }
        }
    }
}

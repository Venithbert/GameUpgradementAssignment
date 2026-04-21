using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Spawns a bouncy ball near the top wall. The ball itself (BouncyBall.cs) counts
/// bounces and self-destructs after maxBounces. Requires
/// PrefabRegistry.bouncyBallPrefab to be assigned.
/// </summary>
public class EffectSummonBouncyBall : ItemEffect
{
    // Same offset logic used by EffectSummonFallingObject — spawn just below top wall
    public float aboveTopBlockOffset = 8f;

    public override void Execute()
    {
        if (PrefabRegistry.SP == null || PrefabRegistry.SP.bouncyBallPrefab == null)
        {
            Debug.LogWarning("EffectSummonBouncyBall: bouncyBallPrefab not set in PrefabRegistry.");
            return;
        }

        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        float maxBlockZ = float.NegativeInfinity;
        foreach (BlockHealth b in blocks)
            if (b.transform.position.z > maxBlockZ)
                maxBlockZ = b.transform.position.z;

        float spawnZ = maxBlockZ + aboveTopBlockOffset;

        // Slight random X so repeated spawns don't stack exactly
        float spawnX = Random.Range(-10f, 10f);
        float spawnY = blocks[0].transform.position.y;

        Object.Instantiate(
            PrefabRegistry.SP.bouncyBallPrefab,
            new Vector3(spawnX, spawnY, spawnZ),
            Quaternion.identity
        );
    }
}

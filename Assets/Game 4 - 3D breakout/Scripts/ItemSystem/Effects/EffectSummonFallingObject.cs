using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Spawns a FallingObject on the same horizontal line just below the top wall.
/// Spawn Z is derived from the highest block row, not wall tags (which would catch
/// side-wall centres and land inside the block grid).
/// Requires PrefabRegistry.fallingObjectPrefab to be assigned in the Inspector.
/// </summary>
public class EffectSummonFallingObject : ItemEffect
{
    // Offset above the topmost block row.
    // Top wall ≈ maxBlockZ + 8–9, so 8 lands just below the wall surface.
    // Increase if objects still overshoot; decrease if they appear too high.
    public float aboveTopBlockOffset = 8f;

    public override void Execute()
    {
        if (PrefabRegistry.SP == null || PrefabRegistry.SP.fallingObjectPrefab == null)
        {
            Debug.LogWarning("EffectSummonFallingObject: fallingObjectPrefab not set in PrefabRegistry.");
            return;
        }

        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        // Find the highest Z among all remaining blocks (= top row)
        float maxBlockZ = float.NegativeInfinity;
        foreach (BlockHealth b in blocks)
            if (b.transform.position.z > maxBlockZ)
                maxBlockZ = b.transform.position.z;

        float spawnZ = maxBlockZ + aboveTopBlockOffset;

        // Use a random block for X; Y matches block height
        Vector3 blockPos = blocks[Random.Range(0, blocks.Length)].transform.position;
        Vector3 spawnPos = new Vector3(blockPos.x, blockPos.y, spawnZ);
        Object.Instantiate(PrefabRegistry.SP.fallingObjectPrefab, spawnPos, Quaternion.identity);
    }
}

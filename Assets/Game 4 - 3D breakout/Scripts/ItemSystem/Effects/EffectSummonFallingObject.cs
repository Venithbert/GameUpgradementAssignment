using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Spawns a FallingObject above a random block.
/// The FallingObject falls and destroys the first block it hits.
/// Requires PrefabRegistry.fallingObjectPrefab to be assigned in the Inspector.
/// </summary>
public class EffectSummonFallingObject : ItemEffect
{
    public override void Execute()
    {
        if (PrefabRegistry.SP == null || PrefabRegistry.SP.fallingObjectPrefab == null)
        {
            Debug.LogWarning("EffectSummonFallingObject: fallingObjectPrefab not set in PrefabRegistry.");
            return;
        }

        BlockHealth[] blocks = Object.FindObjectsOfType<BlockHealth>();
        if (blocks.Length == 0) return;

        Vector3 targetPos  = blocks[Random.Range(0, blocks.Length)].transform.position;
        Vector3 spawnPos   = targetPos + new Vector3(0f, 10f, 0f); // 10 units above the block
        Object.Instantiate(PrefabRegistry.SP.fallingObjectPrefab, spawnPos, Quaternion.identity);
    }
}

using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Spawns a PenetratingObject that passes THROUGH blocks, popping each one it touches.
/// Destroys itself after 1 second.
/// Ready to wire into any ItemDefinition — not assigned to a default item.
/// Requires PrefabRegistry.penetratingObjectPrefab to be set.
/// </summary>
public class EffectSummonPenetratingObject : ItemEffect
{
    public override void Execute()
    {
        if (PrefabRegistry.SP == null || PrefabRegistry.SP.penetratingObjectPrefab == null)
        {
            Debug.LogWarning("EffectSummonPenetratingObject: penetratingObjectPrefab not set in PrefabRegistry.");
            return;
        }

        BlockHealth[] blocks = Object.FindObjectsOfType<BlockHealth>();
        if (blocks.Length == 0) return;

        Vector3 targetPos = blocks[Random.Range(0, blocks.Length)].transform.position;
        Vector3 spawnPos  = targetPos + new Vector3(0f, 10f, 0f);
        Object.Instantiate(PrefabRegistry.SP.penetratingObjectPrefab, spawnPos, Quaternion.identity);
    }
}

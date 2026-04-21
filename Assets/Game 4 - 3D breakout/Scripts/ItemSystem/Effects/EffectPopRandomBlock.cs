using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>Halves one random block in the scene.</summary>
public class EffectPopRandomBlock : ItemEffect
{
    private static GameObject ricochetVfxPrefab;

    public static void SetVfxPrefab(GameObject prefab)
    {
        ricochetVfxPrefab = prefab;
    }

    private void SpawnVfx(Vector3 spawnPosition)
    {
        if (ricochetVfxPrefab == null) return;

        Object.Instantiate(ricochetVfxPrefab, spawnPosition, Quaternion.identity);
    }

    public override void Execute()
    {
        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        BlockHealth target = blocks[Random.Range(0, blocks.Length)];
        Vector3 spawnPosition = target.transform.position;

        BreakoutGame.SP.HalveBlock(target);
        SpawnVfx(spawnPosition);
    }
}
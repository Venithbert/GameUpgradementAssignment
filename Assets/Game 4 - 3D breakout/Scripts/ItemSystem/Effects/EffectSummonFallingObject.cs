using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Spawns a FallingObject just inside the top wall at a random block's X position.
/// All spawns share the same Z (a fixed horizontal line below the top wall),
/// so no object ever appears behind the wall.
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

        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        // Find the top wall (highest Z among all "Wall"-tagged objects)
        // and spawn 2 units below its centre so objects always appear inside the field.
        float spawnZ = -5f; // sensible fallback (above block rows at -8 to -18)
        float maxWallZ = float.NegativeInfinity;
        foreach (GameObject w in GameObject.FindGameObjectsWithTag("Wall"))
            if (w.transform.position.z > maxWallZ)
                maxWallZ = w.transform.position.z;
        if (!float.IsNegativeInfinity(maxWallZ))
            spawnZ = maxWallZ - 2f;

        // Use a random block for the X position; Y matches block height
        Vector3 blockPos = blocks[Random.Range(0, blocks.Length)].transform.position;
        Vector3 spawnPos = new Vector3(blockPos.x, blockPos.y, spawnZ);
        Object.Instantiate(PrefabRegistry.SP.fallingObjectPrefab, spawnPos, Quaternion.identity);
    }
}

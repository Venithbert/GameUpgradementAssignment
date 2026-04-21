using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// Picks a random block as the epicenter.
/// Destroys up to MaxKills blocks within ExplodeRadius, including the epicenter.
/// </summary>
public class EffectExplodeRandomBlock : ItemEffect
{
    private const float ExplodeRadius = 7f;
    private const int MaxKills = 8;

    private static GameObject chainReactionVfxPrefab;

    public static void SetVfxPrefab(GameObject prefab)
    {
        chainReactionVfxPrefab = prefab;
    }

    private void SpawnVfx(Vector3 spawnPosition)
    {
        if (chainReactionVfxPrefab == null) return;

        Object.Instantiate(chainReactionVfxPrefab, spawnPosition, Quaternion.identity);
    }

    public override void Execute()
    {
        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        Vector3 epicenter = blocks[Random.Range(0, blocks.Length)].transform.position;

        int halved = 0;
        foreach (BlockHealth b in blocks)
        {
            if (halved >= MaxKills) break;
            if (b == null) continue;

            if (Vector3.Distance(b.transform.position, epicenter) <= ExplodeRadius)
            {
                Vector3 spawnPosition = b.transform.position;

                BreakoutGame.SP.HalveBlock(b);
                SpawnVfx(spawnPosition);
                halved++;
            }
        }
    }
}

using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>Doubles the value of one random block (e.g. 4 → 8).</summary>
public class EffectDoubleRandomBlock : ItemEffect
{

    //vfx
    private static GameObject doubleValueVfxPrefab;








    public override void Execute()
    {
        BlockHealth[] blocks = Object.FindObjectsByType<BlockHealth>(FindObjectsSortMode.None);
        if (blocks.Length == 0) return;

        BlockHealth target = blocks[Random.Range(0, blocks.Length)];
        target.SetValue(target.Value * 2, true);
        
        SpawnVfx(target);
    }



    // Add a setup method so Unity can assign the prefab once
    public static void SetVfxPrefab(GameObject prefab)
    {
        doubleValueVfxPrefab = prefab;
    }

    //Add a helper method to spawn the VFX on the chosen block
    private void SpawnVfx(BlockHealth target)
    {
        if (doubleValueVfxPrefab == null || target == null) return;

        Vector3 spawnPosition = target.transform.position;
        Quaternion spawnRotation = Quaternion.identity;

        Object.Instantiate(doubleValueVfxPrefab, spawnPosition, spawnRotation);
    }

}





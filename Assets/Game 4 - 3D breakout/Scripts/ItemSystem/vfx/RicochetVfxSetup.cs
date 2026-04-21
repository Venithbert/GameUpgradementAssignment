using UnityEngine;

public class RicochetVfxSetup : MonoBehaviour
{
    public GameObject ricochetVfxPrefab;

    void Awake()
    {
        EffectPopRandomBlock.SetVfxPrefab(ricochetVfxPrefab);
    }
}
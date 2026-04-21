using UnityEngine;

public class ChainReactionVfxSetup : MonoBehaviour
{
    public GameObject chainReactionVfxPrefab;

    void Awake()
    {
        EffectExplodeRandomBlock.SetVfxPrefab(chainReactionVfxPrefab);
    }
}

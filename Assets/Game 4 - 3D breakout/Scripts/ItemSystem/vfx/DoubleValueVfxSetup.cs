using UnityEngine;

public class DoubleValueVfxSetup : MonoBehaviour
{
    public GameObject doubleValueVfxPrefab;

    void Awake()
    {
        EffectDoubleRandomBlock.SetVfxPrefab(doubleValueVfxPrefab);
    }

}

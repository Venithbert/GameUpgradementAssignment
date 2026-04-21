using UnityEngine;

public class RelicDefinition
{
    public string      relicName;
    public string      description;
    public TriggerType triggerType;
    public RelicEffect effect;

    public float triggerInterval = 1f;

    /// <summary>Optional 3D model shown in HUD and picker. Null = placeholder cube.</summary>
    public GameObject previewPrefab;
}

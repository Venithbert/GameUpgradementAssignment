using UnityEngine;

public class ItemDefinition
{
    public string      itemName;
    public string      description;
    public TriggerType triggerType;
    public ItemEffect  effect;

    public float triggerInterval  = 1.5f;
    public int   triggerThreshold = 5;

    /// <summary>Optional 3D model shown in HUD and picker. Null = placeholder cube.</summary>
    public GameObject previewPrefab;
}

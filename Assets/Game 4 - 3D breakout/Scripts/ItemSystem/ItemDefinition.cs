/// <summary>
/// Pairs a trigger condition with an effect.
/// Plain C# class — instances persist across scene reloads in PlayerInventory's static backing array.
/// </summary>
public class ItemDefinition
{
    public string     itemName;
    public string     description;
    public TriggerType triggerType;
    public ItemEffect  effect;

    // Used when triggerType == EveryXSeconds
    public float triggerInterval = 1.5f;

    // Used when triggerType == NthBlockPopped
    public int triggerThreshold = 5;
}

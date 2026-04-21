/// <summary>
/// Force-triggers every non-empty item slot in PlayerInventory in order.
/// </summary>
public class RelicForceTriggerAllItems : RelicEffect
{
    public override void Execute()
    {
        foreach (ItemDefinition item in PlayerInventory.SP.GetSlots())
        {
            if (item != null && item.effect != null)
                item.effect.Execute();
        }
    }
}

/// <summary>
/// Force-triggers all items in odd-numbered slots (1-indexed): #1, #3, #5, #7.
/// These map to 0-indexed array positions 0, 2, 4, 6.
/// </summary>
public class RelicForceTriggerOddSlots : RelicEffect
{
    public override void Execute()
    {
        var slots = PlayerInventory.SP.GetSlots();
        for (int i = 0; i < slots.Length; i += 2) // 0, 2, 4, 6 → slots #1, #3, #5, #7
        {
            ItemDefinition item = slots[i];
            if (item != null && item.effect != null)
                item.effect.Execute();
        }
    }
}

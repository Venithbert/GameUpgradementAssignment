/// <summary>
/// Base class for all relic effects.
/// A relic effect typically force-triggers one or more items in PlayerInventory
/// by calling that item's effect.Execute() directly.
/// </summary>
public abstract class RelicEffect
{
    public abstract void Execute();
}

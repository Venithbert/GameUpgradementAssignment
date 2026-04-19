using System;

/// <summary>
/// Central event bus. Game systems call Fire(); item listeners subscribe via OnTrigger.
/// To add a new event source: call TriggerBus.Fire(TriggerType.X) from anywhere.
/// </summary>
public static class TriggerBus
{
    public static event Action<TriggerType> OnTrigger;

    public static void Fire(TriggerType type)
    {
        OnTrigger?.Invoke(type);
    }
}

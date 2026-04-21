using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds 7 fixed relic slots. Fills left-to-right as relics are acquired.
/// Slots cannot be rearranged. Persists across scene reloads via static backing array.
/// </summary>
public class RelicInventory : MonoBehaviour
{
    public static RelicInventory SP;
    public const  int SlotCount = 7;

    private static RelicDefinition[] persistentSlots = new RelicDefinition[SlotCount];

    void Awake() => SP = this;

    /// <summary>Puts relic into the first empty slot. Returns false if all 7 are full.</summary>
    public bool AddRelic(RelicDefinition relic)
    {
        if (relic == null) return false;
        for (int i = 0; i < SlotCount; i++)
            if (persistentSlots[i] == null) { persistentSlots[i] = relic; return true; }
        return false;
    }

    public RelicDefinition[] GetSlots() => persistentSlots;

    // Kept for backward compat with RelicManager / RelicPickerUI
    public IReadOnlyList<RelicDefinition> GetAll()
    {
        var list = new List<RelicDefinition>();
        foreach (var s in persistentSlots)
            if (s != null) list.Add(s);
        return list;
    }

    /// <summary>Called on full game restart to wipe all relics.</summary>
    public static void ClearRelics()
    {
        persistentSlots = new RelicDefinition[SlotCount];
    }
}

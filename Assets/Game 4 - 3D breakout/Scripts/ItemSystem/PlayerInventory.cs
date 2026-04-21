using UnityEngine;

/// <summary>
/// Holds the 7 persistent item slots. Rendering + drag-swap UI lives in HudController.
/// The backing array is static so items survive scene reloads.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory SP;
    public const  int SlotCount = 7;

    private static ItemDefinition[] persistentSlots = new ItemDefinition[SlotCount];

    void Awake() => SP = this;

    /// <summary>Puts item into the first empty slot. Returns false if full.</summary>
    public bool AddItem(ItemDefinition item)
    {
        for (int i = 0; i < SlotCount; i++)
            if (persistentSlots[i] == null) { persistentSlots[i] = item; return true; }
        return false;
    }

    public ItemDefinition[] GetSlots() => persistentSlots;

    /// <summary>Swap two slot positions (used by the HUD drag-swap).</summary>
    public void Swap(int a, int b)
    {
        if (a < 0 || b < 0 || a >= SlotCount || b >= SlotCount || a == b) return;
        (persistentSlots[a], persistentSlots[b]) = (persistentSlots[b], persistentSlots[a]);
    }

    /// <summary>Called on full game restart to wipe all items.</summary>
    public static void ClearInventory()
    {
        persistentSlots = new ItemDefinition[SlotCount];
    }
}

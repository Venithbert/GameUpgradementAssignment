using UnityEngine;

/// <summary>
/// Manages 7 item slots that persist across scene reloads via a static backing array.
/// Renders a drag-rearrangeable slot bar at the bottom of the screen via OnGUI.
/// Items are active as long as they are in a slot.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory SP;
    public const  int SlotCount = 7;

    // Static so items survive scene reloads (same pattern as LevelManager.CurrentLevelNumber).
    private static ItemDefinition[] persistentSlots = new ItemDefinition[SlotCount];

    private int dragSource = -1; // slot index being dragged, -1 = none

    void Awake() => SP = this;

    // ── Public API ───────────────────────────────────────────────────

    /// <summary>Puts item into the first empty slot. Returns false if full.</summary>
    public bool AddItem(ItemDefinition item)
    {
        for (int i = 0; i < SlotCount; i++)
            if (persistentSlots[i] == null) { persistentSlots[i] = item; return true; }
        return false;
    }

    public ItemDefinition[] GetSlots() => persistentSlots;

    /// <summary>Called on full game restart to wipe all items.</summary>
    public static void ClearInventory()
    {
        persistentSlots = new ItemDefinition[SlotCount];
    }

    // ── OnGUI slot bar ───────────────────────────────────────────────

    void OnGUI()
    {
        const float W = 90f, H = 50f, PAD = 5f;
        float startX = (Screen.width  - SlotCount * (W + PAD)) / 2f;
        float startY =  Screen.height - H - 10f;

        for (int i = 0; i < SlotCount; i++)
        {
            Rect r = new Rect(startX + i * (W + PAD), startY, W, H);

            // Highlight source slot while dragging
            if (i == dragSource)
                GUI.backgroundColor = Color.yellow;

            GUI.Box(r, slots[i] != null ? slots[i].itemName : "(empty)");
            GUI.backgroundColor = Color.white;

            // Begin drag
            if (Event.current.type == EventType.MouseDown &&
                r.Contains(Event.current.mousePosition) &&
                slots[i] != null)
            {
                dragSource = i;
            }

            // Complete drag (drop onto this slot)
            if (Event.current.type == EventType.MouseUp &&
                dragSource >= 0 && dragSource != i &&
                r.Contains(Event.current.mousePosition))
            {
                (slots[i], slots[dragSource]) = (slots[dragSource], slots[i]);
                dragSource = -1;
            }
        }

        // Ghost label follows cursor while dragging
        if (dragSource >= 0 && slots[dragSource] != null)
        {
            GUI.color = new Color(1, 1, 0, 0.8f);
            GUI.Label(
                new Rect(Event.current.mousePosition.x + 5, Event.current.mousePosition.y - 10, W, H),
                slots[dragSource].itemName
            );
            GUI.color = Color.white;
        }

        // Cancel drag on any mouse-up
        if (Event.current.type == EventType.MouseUp) dragSource = -1;
    }

    // ── Shorthand ────────────────────────────────────────────────────

    private static ItemDefinition[] slots => persistentSlots;
}

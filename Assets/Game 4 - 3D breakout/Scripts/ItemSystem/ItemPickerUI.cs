using UnityEngine;

/// <summary>
/// Displays a popup with 3 randomly selected items after every 3rd level.
/// Call Show() to activate. Automatically closes after the player picks an item.
/// Attach to the GameManager object.
/// </summary>
public class ItemPickerUI : MonoBehaviour
{
    public static ItemPickerUI SP;

    private bool             active  = false;
    private ItemDefinition[] choices = null;

    void Awake() => SP = this;

    public void Show()
    {
        choices = ItemDatabase.SP.GetRandomSelection(3);
        active  = true;
    }

    void OnGUI()
    {
        if (!active) return;

        const float POP_W = 420f;
        const float POP_H = 320f;
        const float BTN_W = 360f;
        const float BTN_H = 70f;

        Rect bg = new Rect(
            (Screen.width  - POP_W) / 2f,
            (Screen.height - POP_H) / 2f,
            POP_W, POP_H
        );

        GUI.Box(bg, "Choose an Item  —  Level " + LevelManager.CurrentLevelNumber + " Clear!");

        for (int i = 0; i < choices.Length; i++)
        {
            Rect btn = new Rect(
                bg.x + (POP_W - BTN_W) / 2f,
                bg.y + 60f + i * (BTN_H + 10f),
                BTN_W, BTN_H
            );

            string label = choices[i].itemName + "\n<size=11>" + choices[i].description + "</size>";
            if (GUI.Button(btn, label))
            {
                bool added = PlayerInventory.SP.AddItem(choices[i]);
                if (!added)
                    Debug.Log("Inventory full — item not added.");

                active = false;
                LevelManager.SP.AdvanceLevel();
            }
        }
    }
}

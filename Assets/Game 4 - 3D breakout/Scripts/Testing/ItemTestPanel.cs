using UnityEngine;

/// <summary>
/// Dev-only test panel. Attach to any scene GameObject.
/// Shows a draggable window with one trigger button per item effect.
/// Does NOT touch game state — safe to remove at any time.
/// </summary>
public class ItemTestPanel : MonoBehaviour
{
    private bool _show   = true;
    private Rect _window;

    void Awake()
    {
        _window = new Rect(Screen.width - 260, 10, 250, 50);
    }

    void OnGUI()
    {
        // Toggle button — bottom-right corner
        if (GUI.Button(new Rect(Screen.width - 120, Screen.height - 35, 115, 28), "Item Test Panel"))
            _show = !_show;

        if (!_show || ItemDatabase.SP == null) return;

        _window = GUILayout.Window(9901, _window, DrawWindow, "Item Test Panel");
    }

    void DrawWindow(int id)
    {
        var items = ItemDatabase.SP.GetAllItems();

        foreach (var item in items)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("<b>" + item.itemName + "</b>");
            GUILayout.Label(item.description);
            if (GUILayout.Button("Trigger Effect"))
                item.effect.Execute();
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        GUI.DragWindow(new Rect(0, 0, _window.width, 20));
    }
}

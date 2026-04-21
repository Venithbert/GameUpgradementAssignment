using UnityEngine;

/// <summary>
/// Shows 3 item choices after every 3rd level. Each card has a spinning 3D preview
/// above the name + description + pick button. All sizes scale with the screen resolution.
/// </summary>
public class ItemPickerUI : MonoBehaviour
{
    public static ItemPickerUI SP;

    [SerializeField] Font hudFont;

    private bool             active   = false;
    private ItemDefinition[] choices  = null;
    private Transform[]      previews = new Transform[3];

    // Design-resolution card constants (authored at 1366x768)
    const float CARD_W    = 310f;
    const float CARD_H    = 500f;
    const float CARD_PAD  = 30f;
    const float PREVIEW_H = 180f;

    // Uniform scale so cards always fit regardless of aspect ratio
    float S => Mathf.Min(Screen.width / 1366f, Screen.height / 768f);

    void Awake() => SP = this;

    public void Show()
    {
        choices = ItemDatabase.SP.GetRandomSelection(3);
        active  = true;
        SpawnPreviews();
    }

    void SpawnPreviews()
    {
        DestroyPreviews();
        for (int i = 0; i < choices.Length; i++)
            previews[i] = HudController.MakePreview("ItemPickPreview_" + i, choices[i].previewPrefab, 0.4f);
    }

    void DestroyPreviews()
    {
        for (int i = 0; i < previews.Length; i++)
        {
            if (previews[i] != null) Destroy(previews[i].gameObject);
            previews[i] = null;
        }
    }

    Rect CardRect(int i)
    {
        float s    = S;
        float cw   = CARD_W   * s;
        float ch   = CARD_H   * s;
        float cpad = CARD_PAD * s;
        float totalW = choices.Length * cw + (choices.Length - 1) * cpad;
        float startX = (Screen.width  - totalW) * 0.5f;
        float startY = (Screen.height - ch)      * 0.5f;
        return new Rect(startX + i * (cw + cpad), startY, cw, ch);
    }

    void OnGUI()
    {
        if (!active) return;

        float s  = S;
        float cw   = CARD_W    * s;
        float ch   = CARD_H    * s;
        float cpad = CARD_PAD  * s;
        float ph   = PREVIEW_H * s;

        // Dim background
        GUI.color = new Color(0, 0, 0, 0.55f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Title
        float totalW = choices.Length * cw + (choices.Length - 1) * cpad;
        float startX = (Screen.width  - totalW) * 0.5f;
        float startY = (Screen.height - ch)      * 0.5f;
        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize  = Mathf.RoundToInt(43 * s),
            font      = hudFont
        };
        GUI.Label(new Rect(startX, startY - 36 * s, totalW, 36 * s),
                  "Choose an Item  —  Level " + LevelManager.CurrentLevelNumber + " Clear!",
                  titleStyle);

        float spinDelta = 50f * Time.unscaledDeltaTime;
        for (int i = 0; i < choices.Length; i++)
        {
            Rect card        = CardRect(i);
            Rect previewArea = new Rect(card.x + 6 * s, card.y + 6 * s, card.width - 12 * s, ph - 6 * s);
            GUI.Box(card, GUIContent.none);
            GUI.Box(previewArea, GUIContent.none);
            HudController.DrawPickerPreview(previews[i], previewArea, spinDelta);

            float textY = card.y + ph + 8 * s;

            var nameStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize  = Mathf.RoundToInt(32 * s),
                fontStyle = FontStyle.Bold,
                wordWrap  = true,
                font      = hudFont
            };
            GUI.Label(new Rect(card.x + 6 * s, textY, card.width - 12 * s, 40 * s),
                      choices[i].itemName, nameStyle);

            var descStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize  = Mathf.RoundToInt(23 * s),
                wordWrap  = true,
                font      = hudFont
            };
            GUI.Label(new Rect(card.x + 8 * s, textY + 44 * s, card.width - 16 * s, 200 * s),
                      choices[i].description, descStyle);

            Rect btn = new Rect(card.x + 10 * s, card.y + ch - 50 * s, card.width - 20 * s, 40 * s);
            if (GUI.Button(btn, "Pick"))
            {
                bool added = PlayerInventory.SP.AddItem(choices[i]);
                if (!added) Debug.Log("Inventory full — item not added.");
                Close();
                LevelManager.SP.AdvanceLevel();
            }
        }
    }

    void Close()
    {
        active = false;
        DestroyPreviews();
    }
}

using UnityEngine;

/// <summary>
/// Shows 3 relic choices after levels 4, 7, 10, … Each card has a spinning 3D placeholder
/// cube above the name + description + pick button. Attach to the GameManager object.
/// </summary>
public class RelicPickerUI : MonoBehaviour
{
    public static RelicPickerUI SP;

    [SerializeField] Font hudFont;

    private bool              active   = false;
    private RelicDefinition[] choices  = null;
    private Transform[]       previews = new Transform[3];

    const float CARD_W    = 310f;
    const float CARD_H    = 500f;
    const float CARD_PAD  = 30f;
    const float PREVIEW_H = 180f;

    void Awake() => SP = this;

    public void Show()
    {
        choices = RelicDatabase.SP.GetRandomSelection(3);
        active  = true;
        SpawnPreviews();
    }

    void SpawnPreviews()
    {
        DestroyPreviews();
        for (int i = 0; i < choices.Length; i++)
            previews[i] = HudController.MakePreview("RelicPickPreview_" + i, choices[i].previewPrefab, 0.4f);
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
        float totalW = choices.Length * CARD_W + (choices.Length - 1) * CARD_PAD;
        float startX = (Screen.width  - totalW) * 0.5f;
        float startY = (Screen.height - CARD_H)  * 0.5f;
        return new Rect(startX + i * (CARD_W + CARD_PAD), startY, CARD_W, CARD_H);
    }

    void OnGUI()
    {
        if (!active) return;

        GUI.color = new Color(0, 0, 0, 0.55f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        float totalW = choices.Length * CARD_W + (choices.Length - 1) * CARD_PAD;
        float startX = (Screen.width  - totalW) * 0.5f;
        float startY = (Screen.height - CARD_H)  * 0.5f;
        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize  = 43,
            font      = hudFont
        };
        GUI.Label(new Rect(startX, startY - 36, totalW, 30),
                  "Choose a Relic  —  Level " + LevelManager.CurrentLevelNumber + " Clear!",
                  titleStyle);

        float spinDelta = 50f * Time.unscaledDeltaTime;
        for (int i = 0; i < choices.Length; i++)
        {
            Rect card        = CardRect(i);
            Rect previewArea = new Rect(card.x + 6, card.y + 6, card.width - 12, PREVIEW_H - 6);
            GUI.Box(card, GUIContent.none);
            GUI.Box(previewArea, GUIContent.none);
            HudController.DrawPickerPreview(previews[i], previewArea, spinDelta);

            float textY = card.y + PREVIEW_H + 8;

            var nameStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                fontSize  = 32,
                fontStyle = FontStyle.Bold,
                wordWrap  = true,
                font      = hudFont
            };
            GUI.Label(new Rect(card.x + 6, textY, card.width - 12, 28), choices[i].relicName, nameStyle);

            var descStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize  = 23,
                wordWrap  = true,
                font      = hudFont
            };
            GUI.Label(new Rect(card.x + 8, textY + 40, card.width - 16, 200), choices[i].description, descStyle);

            Rect btn = new Rect(card.x + 10, card.y + CARD_H - 46, card.width - 20, 38);
            if (GUI.Button(btn, "Pick"))
            {
                RelicInventory.SP.AddRelic(choices[i]);
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

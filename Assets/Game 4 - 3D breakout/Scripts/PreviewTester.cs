using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Play-mode tool: drag a prefab in, adjust testScale, then click "Save Scale to Prefab"
/// to write the scale permanently into the prefab asset.
/// Disable or remove this component before shipping.
/// </summary>
public class PreviewTester : MonoBehaviour
{
    [Header("Prefab to test (leave empty = placeholder cube)")]
    public GameObject testPrefab;

    [Header("Scale — adjust live in Inspector during Play mode")]
    public float testScale = 0.35f;

    [Header("Slot to preview in")]
    public bool useRelicSlot = false;
    [Range(0, 9)]
    public int slotIndex = 0;

    private GameObject preview;
    private GameObject lastPrefab;
    private Camera     cam;

    void Start()
    {
        cam = Camera.main;
        RebuildPreview();
    }

    void OnDestroy()
    {
        if (preview != null) Destroy(preview);
    }

    void LateUpdate()
    {
        if (cam == null) cam = Camera.main;
        if (testPrefab != lastPrefab) RebuildPreview();
        if (preview == null) return;

        preview.transform.localScale = Vector3.one * testScale;

        Rect slot = SlotRect();
        Vector3 sp = new Vector3(
            slot.x + slot.width  * 0.5f,
            Screen.height - (slot.y + slot.height * 0.5f),
            HudController.SP != null ? HudController.SP.previewDepth : 1.5f);
        preview.transform.position = cam.ScreenToWorldPoint(sp);
        preview.transform.Rotate(cam.transform.forward, 50f * Time.unscaledDeltaTime, Space.World);
    }

    void RebuildPreview()
    {
        if (preview != null) Destroy(preview);
        preview = testPrefab != null
            ? Instantiate(testPrefab)
            : GameObject.CreatePrimitive(PrimitiveType.Cube);
        foreach (var col in preview.GetComponentsInChildren<Collider>())
            Destroy(col);
        preview.name = "[PreviewTester]";
        lastPrefab   = testPrefab;
    }

    Rect SlotRect()
    {
        var hud = HudController.SP;
        if (hud == null)
            return new Rect(Screen.width * 0.5f - 45f, Screen.height * 0.5f - 45f, 90f, 90f);

        float SX = Screen.width  / hud.referenceResolution.x;
        float SY = Screen.height / hud.referenceResolution.y;

        if (useRelicSlot)
            return new Rect(
                Screen.width - hud.relicRightMargin * SX,
                hud.relicTopMargin * SY + slotIndex * (hud.relicSlotH * SY + hud.relicSlotPad * SY),
                hud.relicSlotW * SX, hud.relicSlotH * SY);

        return new Rect(
            Screen.width - hud.itemRightMargin * SX,
            hud.itemTopMargin * SY + slotIndex * (hud.itemSlotH * SY + hud.itemSlotPad * SY),
            hud.itemSlotW * SX, hud.itemSlotH * SY);
    }

    // ── OnGUI ──────────────────────────────────────────────────────────────

    void OnGUI()
    {
        if (preview == null) return;

        Rect slot = SlotRect();
        var  lbl  = new GUIStyle(GUI.skin.label) { fontSize = 10, normal = { textColor = Color.yellow } };
        GUI.Label(new Rect(slot.x, slot.y - 18, 300, 16),
                  $"[TEST]  scale: {testScale:F4}", lbl);

#if UNITY_EDITOR
        if (testPrefab != null)
        {
            Rect btn = new Rect(slot.x, slot.y - 40, 160, 20);
            if (GUI.Button(btn, "Save Scale to Prefab"))
                SaveScaleToPrefab();
        }
#endif
    }

    // ── Save to prefab asset (Editor only) ─────────────────────────────────

#if UNITY_EDITOR
    [ContextMenu("Save Scale to Prefab")]
    void SaveScaleToPrefab()
    {
        if (testPrefab == null) { Debug.LogWarning("[PreviewTester] No prefab assigned."); return; }

        string path = AssetDatabase.GetAssetPath(testPrefab);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("[PreviewTester] Prefab not found in AssetDatabase.");
            return;
        }

        // Load the prefab asset, set scale, save back
        GameObject contents = PrefabUtility.LoadPrefabContents(path);
        contents.transform.localScale = Vector3.one * testScale;
        PrefabUtility.SaveAsPrefabAsset(contents, path);
        PrefabUtility.UnloadPrefabContents(contents);

        Debug.Log($"[PreviewTester] Saved scale {testScale:F4} to '{testPrefab.name}'");
    }
#endif
}

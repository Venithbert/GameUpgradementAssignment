using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unified in-game HUD.
///   - Item slots: right side, vertical column (7 slots top-to-bottom).
///   - Relic column: right side, narrower column to the right of items.
///   - Stats: bottom-left (level, score/required, hits left).
/// Also owns drag-swap interaction for item slots.
/// Preview objects use each item/relic's previewPrefab if set; otherwise a placeholder cube.
/// </summary>
public class HudController : MonoBehaviour
{
    public static HudController SP;

    [Header("Font")]
    public Font hudFont;

    [Header("Reference resolution — set this to your design resolution so layout scales correctly")]
    public Vector2 referenceResolution = new Vector2(1366f, 768f);

    [Header("Item column (right side)")]
    public float itemSlotW        = 90f;
    public float itemSlotH        = 90f;
    public float itemSlotPad      = 6f;
    public float itemTopMargin    = 40f;
    public float itemRightMargin  = 490f;

    [Header("Relic column (right of items)")]
    public float relicSlotW       = 80f;
    public float relicSlotH       = 80f;
    public float relicSlotPad     = 6f;
    public float relicTopMargin   = 40f;
    public float relicRightMargin = 370f;

    [Header("3D preview")]
    public float previewDepth     = 1.5f;
    public float previewScale     = 0.35f;
    public float previewSpinSpeed = 15f;
    [Range(0.1f, 1f)] public float previewFill = 0.8f;

    [Header("Relic trigger VFX")]
    public GameObject relicTriggerVfxPrefab;

    float SX => Screen.width  / referenceResolution.x;
    float SY => Screen.height / referenceResolution.y;

    private Camera cam;

    // Per-slot item tracking: detect changes to swap preview objects
    private ItemDefinition[]    knownItems    = new ItemDefinition[PlayerInventory.SlotCount];
    private Transform[]         itemPreviews  = new Transform[PlayerInventory.SlotCount];

    // Relic slots — fixed 7, same as items
    private RelicDefinition[] knownRelics   = new RelicDefinition[RelicInventory.SlotCount];
    private Transform[]       relicPreviews = new Transform[RelicInventory.SlotCount];

    private int       dragSource = -1;
    private Texture2D blackTex   = null;
    private float     _spinDelta = 0f;

    // ── Picker preview camera (renders on top of OnGUI overlays) ──────
    static Camera         s_pickerCam;
    static RenderTexture  s_pickerRT;
    static readonly Vector3 PICKER_STAGE = new Vector3(0f, 9999f, 0f);
    const  float PICKER_CAM_DIST   = 2.5f;
    const  float PICKER_CAM_UP     = 0.3f;
    const  float PICKER_OBJ_RADIUS = 0.6f; // world-space half-size to fill the RT frame

    Texture2D BlackTex()
    {
        if (blackTex == null)
        {
            blackTex = new Texture2D(1, 1);
            blackTex.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.85f));
            blackTex.Apply();
        }
        return blackTex;
    }

    void Awake()
    {
        SP  = this;
        cam = Camera.main;
        InitPickerCamera();
    }

    static void InitPickerCamera()
    {
        if (s_pickerCam != null) return;

        var go = new GameObject("PickerPreviewCam") { hideFlags = HideFlags.HideAndDontSave };
        Object.DontDestroyOnLoad(go);

        s_pickerCam                  = go.AddComponent<Camera>();
        s_pickerCam.clearFlags       = CameraClearFlags.SolidColor;
        s_pickerCam.backgroundColor  = Color.clear;
        s_pickerCam.fieldOfView      = 40f;
        s_pickerCam.nearClipPlane    = 0.1f;
        s_pickerCam.farClipPlane     = 15f;
        s_pickerCam.enabled          = false; // never auto-renders

        s_pickerRT = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        s_pickerCam.targetTexture = s_pickerRT;
    }

    /// <summary>
    /// Call inside OnGUI (Repaint only) to render a picker preview object on top of any overlay.
    /// </summary>
    public static void DrawPickerPreview(Transform obj, Rect guiRect, float spinDelta)
    {
        if (obj == null || s_pickerCam == null || Event.current.type != EventType.Repaint) return;

        // Scale object to a consistent visual size using its PreviewFit radius
        var fit = obj.GetComponent<PreviewFit>();
        if (fit != null && fit.localRadius > 0f)
            obj.localScale = Vector3.one * (PICKER_OBJ_RADIUS / fit.localRadius);

        // Move to staging area and spin
        obj.position = PICKER_STAGE;
        obj.Rotate(Vector3.up, spinDelta, Space.World);

        // Aim camera at the staged object
        s_pickerCam.transform.position = PICKER_STAGE + new Vector3(0f, PICKER_CAM_UP, -PICKER_CAM_DIST);
        s_pickerCam.transform.LookAt(PICKER_STAGE + new Vector3(0f, PICKER_CAM_UP * 0.5f, 0f));

        s_pickerCam.Render();

        // Draw the RT in the GUI — on top of whatever was drawn before
        GUI.DrawTexture(guiRect, s_pickerRT, ScaleMode.ScaleToFit, alphaBlend: true);

        // Park object away so it doesn't appear in other slots' renders this frame
        obj.position = PICKER_STAGE + Vector3.down * 9999f;
    }

    [ContextMenu("Apply Default Layout")]
    void ApplyDefaultLayout()
    {
        itemSlotW        = 90f;  itemSlotH        = 90f;
        itemSlotPad      = 6f;   itemTopMargin    = 40f;
        itemRightMargin  = 490f;
        relicSlotW       = 80f;  relicSlotH       = 80f;
        relicSlotPad     = 6f;   relicTopMargin   = 40f;
        relicRightMargin = 370f;
    }

    // ── Preview factory ────────────────────────────────────────────────

    /// <summary>
    /// Creates a preview object from <paramref name="prefab"/>, or a plain cube if null.
    /// All colliders are removed so it never interacts with gameplay.
    /// A PreviewFit component is attached with the local-space bounding-sphere radius
    /// so PlaceAt can auto-scale the object to fit any slot.
    /// </summary>
    public static Transform MakePreview(string goName, GameObject prefab = null, float scale = -1f)
    {
        GameObject go;
        if (prefab != null)
            go = Instantiate(prefab);
        else
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);

        foreach (var col in go.GetComponentsInChildren<Collider>())
            Destroy(col);

        go.name = goName;
        go.transform.localScale = Vector3.one;

        // Measure local-space bounding sphere radius from mesh bounds (rotation invariant).
        float radius = MeasureLocalRadius(go.transform);
        var fit = go.AddComponent<PreviewFit>();
        fit.localRadius = radius > 0f ? radius : 0.5f;
        return go.transform;
    }

    /// <summary>
    /// Computes the bounding-sphere radius of all meshes under <paramref name="root"/>
    /// expressed in the root's LOCAL space (i.e. unaffected by future rotation or scale
    /// applied to the root).
    /// </summary>
    static float MeasureLocalRadius(Transform root)
    {
        Matrix4x4 worldToRoot = root.worldToLocalMatrix;
        bool haveBounds = false;
        Bounds local = new Bounds();

        void AddMesh(Mesh m, Transform t) {
            if (m == null) return;
            Bounds mb = m.bounds; // local to t
            Vector3 c = mb.center, e = mb.extents;
            Matrix4x4 toRoot = worldToRoot * t.localToWorldMatrix;
            for (int i = 0; i < 8; i++) {
                Vector3 corner = new Vector3(
                    c.x + ((i & 1) != 0 ? e.x : -e.x),
                    c.y + ((i & 2) != 0 ? e.y : -e.y),
                    c.z + ((i & 4) != 0 ? e.z : -e.z));
                Vector3 p = toRoot.MultiplyPoint3x4(corner);
                if (!haveBounds) { local = new Bounds(p, Vector3.zero); haveBounds = true; }
                else             local.Encapsulate(p);
            }
        }

        foreach (var mf  in root.GetComponentsInChildren<MeshFilter>())         AddMesh(mf.sharedMesh, mf.transform);
        foreach (var smr in root.GetComponentsInChildren<SkinnedMeshRenderer>()) AddMesh(smr.sharedMesh, smr.transform);

        return haveBounds ? local.extents.magnitude : 0f;
    }

    // ── Per-frame preview placement ────────────────────────────────────

    void LateUpdate()
    {
        // Per-frame spin delta for DrawPickerPreview
        _spinDelta = previewSpinSpeed * Time.unscaledDeltaTime;

        // Items — rebuild preview when slot content changes; park offscreen
        var slots = PlayerInventory.SP != null ? PlayerInventory.SP.GetSlots() : null;
        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            var item = slots != null ? slots[i] : null;
            if (item != knownItems[i])
            {
                if (itemPreviews[i] != null) Destroy(itemPreviews[i].gameObject);
                knownItems[i]   = item;
                itemPreviews[i] = item != null ? MakePreview("ItemPreview_" + i, item.previewPrefab) : null;
            }
            if (itemPreviews[i] != null)
                itemPreviews[i].position = PICKER_STAGE + Vector3.right * 5f;
        }

        // Relics — rebuild preview when slot content changes; park offscreen
        var relicSlots = RelicInventory.SP != null ? RelicInventory.SP.GetSlots() : null;
        for (int i = 0; i < RelicInventory.SlotCount; i++)
        {
            var relic = relicSlots != null ? relicSlots[i] : null;
            if (relic != knownRelics[i])
            {
                if (relicPreviews[i] != null) Destroy(relicPreviews[i].gameObject);
                knownRelics[i]   = relic;
                relicPreviews[i] = relic != null ? MakePreview("RelicPreview_" + i, relic.previewPrefab) : null;
            }
            if (relicPreviews[i] != null)
                relicPreviews[i].position = PICKER_STAGE + Vector3.right * 5f;
        }
    }

    public void PlaceAt(Transform t, Rect guiRect, float spin)
    {
        // Slot centre in screen coords (bottom-left origin).
        float cx = guiRect.x + guiRect.width  * 0.5f;
        float cy = Screen.height - (guiRect.y + guiRect.height * 0.5f);

        // Place object at slot centre at fixed depth.
        Vector3 centre = cam.ScreenToWorldPoint(new Vector3(cx, cy, previewDepth));
        t.position = centre;

        // World units per screen pixel at previewDepth — works for both perspective
        // and orthographic cameras.
        Vector3 pX = cam.ScreenToWorldPoint(new Vector3(cx + 1f, cy, previewDepth));
        Vector3 pY = cam.ScreenToWorldPoint(new Vector3(cx, cy + 1f, previewDepth));
        float worldPerPxX = Vector3.Distance(centre, pX);
        float worldPerPxY = Vector3.Distance(centre, pY);

        // Target half-size in world units = shortest slot dimension × fill factor.
        float slotRadius = 0.5f * previewFill * Mathf.Min(
            guiRect.width  * worldPerPxX,
            guiRect.height * worldPerPxY);

        // Scale = target radius / prefab's local bounding-sphere radius (cached at spawn).
        var fit = t.GetComponent<PreviewFit>();
        float localR = fit != null ? fit.localRadius : 0.5f;
        if (localR > 0f)
            t.localScale = Vector3.one * (slotRadius / localR);

        t.Rotate(cam.transform.forward, spin, Space.World);
    }

    // ── Slot rect helpers ──────────────────────────────────────────────

    Rect ItemSlotRect(int i)
    {
        float w = itemSlotW * SX, h = itemSlotH * SY, pad = itemSlotPad * SY;
        return new Rect(
            Screen.width  - itemRightMargin * SX,
            itemTopMargin * SY + i * (h + pad),
            w, h);
    }

    Rect RelicSlotRect(int i)
    {
        float w = relicSlotW * SX, h = relicSlotH * SY, pad = relicSlotPad * SY;
        return new Rect(
            Screen.width  - relicRightMargin * SX,
            relicTopMargin * SY + i * (h + pad),
            w, h);
    }

    // ── Relic slot VFX ─────────────────────────────────────────────────

    public void PlayRelicSlotVfx(int slotIndex)
    {
        if (relicTriggerVfxPrefab == null) return;
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Rect r   = RelicSlotRect(slotIndex);
        float cx = r.x + r.width  * 0.5f;
        float cy = Screen.height - (r.y + r.height * 0.5f);
        Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(cx, cy, previewDepth));
        Instantiate(relicTriggerVfxPrefab, worldPos, Quaternion.identity);
    }

    // ── OnGUI ──────────────────────────────────────────────────────────

    void OnGUI()
    {
        DrawItemSlots();
        DrawRelicSlots();
        DrawStats();
    }

    void DrawItemSlots()
    {
        if (PlayerInventory.SP == null) return;
        var   slots = PlayerInventory.SP.GetSlots();
        Event e     = Event.current;

        GUI.Label(new Rect(Screen.width - itemRightMargin * SX, itemTopMargin * SY - 22 * SY, itemSlotW * SX, 20 * SY),
                  "items", CenterLabel(Mathf.RoundToInt(11 * SY)));

        for (int i = 0; i < PlayerInventory.SlotCount; i++)
        {
            Rect r = ItemSlotRect(i);
            if (i == dragSource) GUI.backgroundColor = Color.yellow;
            GUI.Box(r, GUIContent.none);
            GUI.backgroundColor = Color.white;

            if (slots[i] != null)
            {
                DrawPickerPreview(itemPreviews[i], new Rect(r.x - 6, r.y - 6, r.width + 12, r.height + 12), _spinDelta);
                GUI.Label(new Rect(r.x - 4, r.y + r.height - 16 * SY, r.width + 8, 20 * SY),
                          slots[i].itemName, CenterLabel(Mathf.RoundToInt(10 * SY)));
            }

            if (e.type == EventType.MouseDown && r.Contains(e.mousePosition) && slots[i] != null)
                dragSource = i;
            if (e.type == EventType.MouseUp && dragSource >= 0 && dragSource != i && r.Contains(e.mousePosition))
            {
                PlayerInventory.SP.Swap(dragSource, i);
                dragSource = -1;
            }
        }

        if (dragSource >= 0 && slots[dragSource] != null)
        {
            GUI.color = new Color(1, 1, 0, 0.85f);
            GUI.Label(new Rect(e.mousePosition.x + 6, e.mousePosition.y - 10, 150, 22),
                      slots[dragSource].itemName);
            GUI.color = Color.white;
        }
        if (e.type == EventType.MouseUp) dragSource = -1;
    }

    void DrawRelicSlots()
    {
        if (RelicInventory.SP == null) return;
        var slots = RelicInventory.SP.GetSlots();

        GUI.Label(
            new Rect(Screen.width - relicRightMargin * SX, relicTopMargin * SY - 22 * SY, relicSlotW * SX, 20 * SY),
            "relics", CenterLabel(Mathf.RoundToInt(11 * SY)));

        string hoveredDesc   = null;
        Rect   tooltipAnchor = default;
        var    nameLbl       = CenterLabel(Mathf.RoundToInt(9 * SY));

        for (int i = 0; i < RelicInventory.SlotCount; i++)
        {
            Rect r = RelicSlotRect(i);

            // Black background + border for every slot, filled or not
            GUI.DrawTexture(r, BlackTex());
            GUI.Box(r, GUIContent.none);

            if (slots[i] != null)
            {
                DrawPickerPreview(relicPreviews[i], new Rect(r.x - 6, r.y - 6, r.width + 12, r.height + 12), _spinDelta);
                // Name below filled slot
                GUI.Label(new Rect(r.x - 4, r.y + r.height + 2 * SY, r.width + 8, 26 * SY),
                          slots[i].relicName, nameLbl);

                // Hover — collect for tooltip drawn after loop
                if (r.Contains(Event.current.mousePosition))
                {
                    hoveredDesc   = slots[i].description;
                    tooltipAnchor = r;
                }
            }
        }

        if (hoveredDesc != null)
        {
            float tw = 200f * SX, th = 50f * SY;
            float tx = tooltipAnchor.x - tw - 6f * SX;
            float ty = Mathf.Max(tooltipAnchor.y, 4f);
            var tip = new GUIStyle(GUI.skin.box)
            {
                fontSize  = Mathf.RoundToInt(10 * SY),
                wordWrap  = true,
                alignment = TextAnchor.UpperLeft,
                padding   = new RectOffset(6, 6, 4, 4),
                font      = hudFont
            };
            GUI.Box(new Rect(tx, ty, tw, th), hoveredDesc, tip);
        }
    }

    void DrawStats()
    {
        float W = 240f * SX, H = 22f * SY, PAD = 4f * SY;
        float x = 12f  * SX;
        float y = Screen.height - (H * 3 + PAD * 2) - 14f * SY;

        var s     = new GUIStyle(GUI.skin.label) { fontSize = Mathf.RoundToInt(14 * SY), font = hudFont };
        int score = ScoreManager.SP     != null ? ScoreManager.SP.CurrentScore               : 0;
        int req   = LevelManager.SP     != null ? LevelManager.SP.CurrentLevel.requiredScore  : 0;
        int hits  = PaddleHitCounter.SP != null ? PaddleHitCounter.SP.HitsRemaining           : 0;

        GUI.Label(new Rect(x, y,                 W, H), "Level: " + LevelManager.CurrentLevelNumber, s);
        GUI.Label(new Rect(x, y + (H + PAD),     W, H), "Score: " + score + " / " + req, s);
        GUI.Label(new Rect(x, y + (H + PAD) * 2, W, H), "Hits Left: " + hits, s);
    }

    GUIStyle CenterLabel(int size) => new GUIStyle(GUI.skin.label)
    {
        alignment = TextAnchor.UpperCenter,
        fontSize  = size,
        wordWrap  = true,
        font      = hudFont
    };
}

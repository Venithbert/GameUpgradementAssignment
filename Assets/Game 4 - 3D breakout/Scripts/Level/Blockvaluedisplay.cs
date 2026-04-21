using System.Collections;
using UnityEngine;

public class BlockValueDisplay : MonoBehaviour
{
    [Header("Font")]
    [SerializeField] Font blockFont;

    [Header("Position / Size")]
    [Tooltip("World-space offset from the block's mesh centre. (0,0,0) = dead centre.")]
    [SerializeField] Vector3 worldOffset  = new Vector3(0f, 0f, 0f);
    [SerializeField] int     fontSize     = 28;
    [SerializeField] float   characterSize = 0.15f;

    [Header("Effects")]
    [SerializeField] Color halveColor      = Color.red;
    [SerializeField] Color doubleColor     = new Color(0.2f, 1f, 0.2f);
    [SerializeField] float effectDuration  = 0.5f;
    [SerializeField] float effectPeakScale = 2f;

    private TextMesh  textMesh;
    private Coroutine effectCoroutine;
    private float     baseCharSize;
    private Vector3   meshCenterWorld; // cached world-space centre of the block mesh
    private Camera    mainCam;

    void Awake()
    {
        mainCam = Camera.main;

        // Cache the block mesh centre in world space before creating the text child.
        Renderer blockRenderer = GetComponentInChildren<Renderer>();
        meshCenterWorld = blockRenderer != null ? blockRenderer.bounds.center : transform.position;

        GameObject textObj = new GameObject("BlockValueText");
        textObj.transform.SetParent(transform, false);
        textObj.transform.localScale = Vector3.one;

        textMesh               = textObj.AddComponent<TextMesh>();
        textMesh.alignment     = TextAlignment.Center;
        textMesh.anchor        = TextAnchor.MiddleCenter;
        textMesh.fontSize      = fontSize;
        textMesh.characterSize = characterSize;
        textMesh.color         = Color.white;
        baseCharSize           = characterSize;

        if (blockFont != null)
        {
            textMesh.font = blockFont;
            textMesh.GetComponent<MeshRenderer>().material = blockFont.material;
        }
    }

    void LateUpdate()
    {
        if (textMesh == null) return;
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        // Place at block mesh centre + user offset, always facing the camera.
        textMesh.transform.position = meshCenterWorld + worldOffset;
        textMesh.transform.rotation = mainCam.transform.rotation;
    }

    public void UpdateDisplay(int value)
    {
        if (textMesh != null)
            textMesh.text = value.ToString();
    }

    public void PlayHalveEffect()  => RunEffect(halveColor);
    public void PlayDoubleEffect() => RunEffect(doubleColor);

    void RunEffect(Color flash)
    {
        if (textMesh == null) return;
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(FlashEffect(flash));
    }

    IEnumerator FlashEffect(Color flash)
    {
        float peak = baseCharSize * effectPeakScale;
        float half = effectDuration * 0.5f;
        float e    = 0f;

        while (e < half)
        {
            if (textMesh == null) yield break;
            e += Time.deltaTime;
            float p = Mathf.Clamp01(e / half);
            textMesh.characterSize = Mathf.Lerp(baseCharSize, peak, p);
            textMesh.color         = Color.Lerp(Color.white, flash, p);
            yield return null;
        }

        e = 0f;

        while (e < half)
        {
            if (textMesh == null) yield break;
            e += Time.deltaTime;
            float p = Mathf.Clamp01(e / half);
            textMesh.characterSize = Mathf.Lerp(peak, baseCharSize, p);
            textMesh.color         = Color.Lerp(flash, Color.white, p);
            yield return null;
        }

        textMesh.characterSize = baseCharSize;
        textMesh.color         = Color.white;
        effectCoroutine        = null;
    }
}

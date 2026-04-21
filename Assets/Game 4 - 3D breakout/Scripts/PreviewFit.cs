using UnityEngine;

/// <summary>
/// Attached by HudController.MakePreview to every preview object.
/// Stores the prefab's local-space bounding-sphere radius (measured once at spawn,
/// unaffected by later rotation/scale) so PlaceAt can compute a fit scale per slot.
/// </summary>
public class PreviewFit : MonoBehaviour
{
    public float localRadius = 0.5f;
}

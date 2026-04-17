using UnityEngine;

// Add to Block prefab.
// Creates a TextMesh child at runtime to display the block's value.
// Adjust localPosition / localRotation to suit your camera angle.
public class BlockValueDisplay : MonoBehaviour
{
    private TextMesh textMesh;

    void Awake()
    {
        GameObject textObj = new GameObject("BlockValueText");
        textObj.transform.SetParent(transform);

        // Offset slightly above the block surface; rotate to face camera (top-down Z axis)
        textObj.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        textObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        textMesh = textObj.AddComponent<TextMesh>();
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontSize = 28;
        textMesh.characterSize = 0.15f;
        textMesh.color = Color.white;
    }

    public void UpdateDisplay(int value)
    {
        if (textMesh != null)
            textMesh.text = value.ToString();
    }
}

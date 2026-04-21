using UnityEngine;

/// <summary>
/// Holds prefab references that effect classes need to Instantiate.
/// Attach to the GameManager object and assign prefabs in the Inspector.
/// To add a new spawnable: add a public field here and reference it in your effect class.
/// </summary>
public class PrefabRegistry : MonoBehaviour
{
    public static PrefabRegistry SP;

    [Header("Item Effect Prefabs")]
    public GameObject fallingObjectPrefab;
    public GameObject penetratingObjectPrefab;
    public GameObject bouncyBallPrefab;

    void Awake() => SP = this;
}

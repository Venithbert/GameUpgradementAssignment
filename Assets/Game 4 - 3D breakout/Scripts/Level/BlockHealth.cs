using UnityEngine;

// Add to Block prefab alongside Block.cs
// Controls the block's current value and whether the ball can pass through it.
public class BlockHealth : MonoBehaviour
{
    public int Value { get; private set; }

    public bool IsPenetrable => Value <= 1;

    public void SetValue(int value, bool playDoubleEffect = false)
    {
        Value = value;
        RefreshCollider();
        var disp = GetComponent<BlockValueDisplay>();
        disp.UpdateDisplay(Value);
        if (playDoubleEffect) disp.PlayDoubleEffect();
    }

    // Called when ball hits this block.
    // Returns the score earned (the damage = current value before halving).
    public int TakeDamage()
    {
        int scoreEarned = Value;
        Value = Value / 2;
        var disp = GetComponent<BlockValueDisplay>();
        disp.UpdateDisplay(Value);
        disp.PlayHalveEffect();
        RefreshCollider();
        return scoreEarned;
    }

    // Blocks with value 1 become triggers so the ball passes through them.
    private void RefreshCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = IsPenetrable;
    }
}

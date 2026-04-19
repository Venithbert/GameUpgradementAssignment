/// <summary>
/// Base class for all item effects.
/// To add a new effect: create a class that extends ItemEffect, implement Execute().
/// Effects are plain C# objects — use UnityEngine statics (Object.FindObjectsOfType, etc.)
/// and singleton references (BreakoutGame.SP, ScoreManager.SP, etc.) inside Execute().
/// </summary>
public abstract class ItemEffect
{
    public abstract void Execute();
}

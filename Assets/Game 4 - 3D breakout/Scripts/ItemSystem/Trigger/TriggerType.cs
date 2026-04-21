/// <summary>
/// Used both as item trigger configuration (what an item listens for)
/// and as TriggerBus event keys (what game systems fire).
///
/// To add a new trigger:
///   1. Add entry here.
///   2. Call TriggerBus.Fire(TriggerType.YourTrigger) from the relevant game system.
///   3. Use it in ItemDatabase when defining items.
/// </summary>
public enum TriggerType
{
    // ---- Timer-based (handled by ItemManager.Update, never fired on TriggerBus) ----
    EveryXSeconds,

    // ---- Block-count-based (derived inside ItemManager from raw BlockPopped bus events) ----
    FirstBlockPopped,   // fires once on first BlockPopped of a level
    NthBlockPopped,     // fires every N BlockPopped; N set via ItemDefinition.triggerThreshold

    // ---- Ball-hit-based (derived inside ItemManager from raw BallHitsBlock bus events) ----
    NthBallHitBlock,    // fires every N ball-hits-to-blocks; N set via ItemDefinition.triggerThreshold
    FirstBlockHalved,   // fires exactly once per level on first BallHitsBlock

    // ---- Raw bus events (fired by game systems, can also be used directly as item triggers) ----
    BlockPopped,        // every single block destroyed (use NthBlockPopped for items instead)
    BallHitsBlock,      // every time ball hits a block (TakeDamage called) — also usable as direct item trigger
    BallHitsWall,
    BallBounces,        // off the paddle
    BallLaunched,       // fires every time a ball is launched (initial click OR bonus-ball spawn)
    PassLevelGoal,
    BallDies,
}

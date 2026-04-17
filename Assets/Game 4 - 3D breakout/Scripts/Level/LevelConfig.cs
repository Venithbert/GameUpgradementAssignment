using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    public int levelNumber;
    public int requiredScore;

    public static LevelConfig Create(int levelNumber)
    {
        return new LevelConfig
        {
            levelNumber = levelNumber,
            requiredScore = levelNumber * 10
        };
    }

    // Level 1: all 2s
    // Level 2: 70% twos, 30% fours
    // Level 3: 50% twos, 35% fours, 15% eights
    // Level 4+: 40% twos, 30% fours, 20% eights, 10% sixteens
    public int GetRandomBlockValue()
    {
        float[] weights = GetWeightsForLevel(levelNumber);
        float roll = Random.value;
        float cumulative = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (roll < cumulative)
                return (int)Mathf.Pow(2, i + 1); // tier 0 = 2, tier 1 = 4, tier 2 = 8, tier 3 = 16
        }

        return 2;
    }

    private float[] GetWeightsForLevel(int level)
    {
        switch (level)
        {
            case 1: return new float[] { 1f };
            case 2: return new float[] { 0.7f, 0.3f };
            case 3: return new float[] { 0.5f, 0.35f, 0.15f };
            default: return new float[] { 0.4f, 0.3f, 0.2f, 0.1f };
        }
    }
}

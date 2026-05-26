using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelDefinition
{
    public int levelNumber = 1;
    public int blocksToDestroy = 5;
    public int startingCoins = 30;
    public int maxPickaxeLevel = 3;
    public float blockHP = 12f;
    public List<int> blockSequence = new List<int> { 0 };
    public string zoneId = "stone_cave";
    public int gemsReward = 1;
}

[CreateAssetMenu(menuName = "MergeMining/LevelConfig", fileName = "LevelConfig")]
public class LevelData : ScriptableObject
{
    public List<LevelDefinition> levels = new List<LevelDefinition>();
    public int pickaxeBaseCost = 10;

    public LevelDefinition GetLevel(int oneBasedNumber)
    {
        if (levels.Count == 0) return null;
        int idx = Mathf.Clamp(oneBasedNumber - 1, 0, levels.Count - 1);
        return levels[idx];
    }

    public int Count => levels.Count;
}

using System.Collections.Generic;
using UnityEngine;

public enum ChestType { Free, Block, Premium }

[System.Serializable]
public class ChestReward
{
    public string id;
    public Color color = Color.white;
    public int minAmount = 1;
    public int maxAmount = 5;
    public float weight = 1f;
}

[System.Serializable]
public class ChestTypeData
{
    public ChestType type;
    public string displayName;
    public Color chestColor = new Color(0.5f, 0.35f, 0.2f);
    public Color accentColor = new Color(0.95f, 0.78f, 0.25f);
    public int rewardsCount = 4;
    public List<ChestReward> possibleRewards = new List<ChestReward>();
}

[CreateAssetMenu(menuName = "MergeMining/ChestConfig", fileName = "ChestConfig")]
public class ChestData : ScriptableObject
{
    public List<ChestTypeData> chests = new List<ChestTypeData>();
    public float freeChestCooldownSeconds = 300f;
    public int blocksPerChest = 20;
    public int premiumChestGemsCost = 50;

    public ChestTypeData Get(ChestType type)
    {
        foreach (var c in chests) if (c.type == type) return c;
        return null;
    }
}

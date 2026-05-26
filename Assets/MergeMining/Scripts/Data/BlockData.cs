using System.Collections.Generic;
using UnityEngine;

public enum BlockBehavior
{
    Normal,
    Iron,
    Diamond,
    Explosive,
    Healer
}

[System.Serializable]
public class BlockTypeData
{
    public string id = "stone";
    public string displayName = "Stone";
    public Color color = Color.gray;
    public Color darkColor = new Color(0.3f, 0.3f, 0.3f);
    public float hpMultiplier = 1f;
    public float rewardMultiplier = 1f;
    public BlockBehavior behavior = BlockBehavior.Normal;
    public int minPickaxeLevel = 1;
    public float damageMultiplier = 1f;
    public float explosionDamage = 5f;
    public float healPerSecond = 0f;
    public float healInterval = 2f;
}

[CreateAssetMenu(menuName = "MergeMining/BlockConfig", fileName = "BlockConfig")]
public class BlockData : ScriptableObject
{
    public List<BlockTypeData> types = new List<BlockTypeData>();
    public List<int> defaultSequence = new List<int>();
    public float baseHP = 10f;
    public float hpGrowth = 1.18f;
    public int baseReward = 3;
    public float rewardGrowth = 1.15f;

    public BlockTypeData GetTypeForBlock(int blocksDestroyed, List<int> overrideSequence = null)
    {
        List<int> seq = (overrideSequence != null && overrideSequence.Count > 0) ? overrideSequence : defaultSequence;
        if (types.Count == 0 || seq.Count == 0)
        {
            Debug.LogWarning("BlockData: empty config");
            return null;
        }
        int idx = seq[blocksDestroyed % seq.Count];
        if (idx < 0 || idx >= types.Count) idx = 0;
        return types[idx];
    }

    public float CalcHP(int blocksDestroyed, List<int> overrideSequence = null, float zoneMultiplier = 1f)
    {
        BlockTypeData t = GetTypeForBlock(blocksDestroyed, overrideSequence);
        float mult = t != null ? t.hpMultiplier : 1f;
        return baseHP * Mathf.Pow(hpGrowth, blocksDestroyed) * mult * zoneMultiplier;
    }

    public int CalcReward(int blocksDestroyed, List<int> overrideSequence = null, float zoneMultiplier = 1f)
    {
        BlockTypeData t = GetTypeForBlock(blocksDestroyed, overrideSequence);
        float mult = t != null ? t.rewardMultiplier : 1f;
        return Mathf.Max(1, Mathf.RoundToInt(baseReward * Mathf.Pow(rewardGrowth, blocksDestroyed) * mult * zoneMultiplier));
    }
}

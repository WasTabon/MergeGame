using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockTypeData
{
    public string id = "stone";
    public string displayName = "Stone";
    public Color color = Color.gray;
    public Color darkColor = new Color(0.3f, 0.3f, 0.3f);
    public float hpMultiplier = 1f;
    public float rewardMultiplier = 1f;
}

[CreateAssetMenu(menuName = "MergeMining/BlockConfig", fileName = "BlockConfig")]
public class BlockData : ScriptableObject
{
    public List<BlockTypeData> types = new List<BlockTypeData>();
    public List<int> sequence = new List<int>();
    public float baseHP = 10f;
    public float hpGrowth = 1.18f;
    public int baseReward = 3;
    public float rewardGrowth = 1.15f;

    public BlockTypeData GetTypeForBlock(int blocksDestroyed)
    {
        if (types.Count == 0 || sequence.Count == 0)
        {
            Debug.LogWarning("BlockData: empty config");
            return null;
        }
        int idx = sequence[blocksDestroyed % sequence.Count];
        if (idx < 0 || idx >= types.Count) idx = 0;
        return types[idx];
    }

    public float CalcHP(int blocksDestroyed)
    {
        BlockTypeData t = GetTypeForBlock(blocksDestroyed);
        float mult = t != null ? t.hpMultiplier : 1f;
        return baseHP * Mathf.Pow(hpGrowth, blocksDestroyed) * mult;
    }

    public int CalcReward(int blocksDestroyed)
    {
        BlockTypeData t = GetTypeForBlock(blocksDestroyed);
        float mult = t != null ? t.rewardMultiplier : 1f;
        return Mathf.Max(1, Mathf.RoundToInt(baseReward * Mathf.Pow(rewardGrowth, blocksDestroyed) * mult));
    }
}

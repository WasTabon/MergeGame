using System.Collections.Generic;
using UnityEngine;

public enum DailyRewardKind { Coins, Gems, PickaxeLevel }

[System.Serializable]
public class DailyRewardEntry
{
    public int day = 1;
    public DailyRewardKind kind = DailyRewardKind.Coins;
    public int amount = 100;
    public int pickaxeLevel = 1;
}

[CreateAssetMenu(menuName = "MergeMining/DailyRewardConfig", fileName = "DailyRewardConfig")]
public class DailyRewardData : ScriptableObject
{
    public List<DailyRewardEntry> entries = new List<DailyRewardEntry>();

    public DailyRewardEntry GetForDay(int day)
    {
        if (entries.Count == 0) return null;
        int idx = (day - 1) % entries.Count;
        if (idx < 0) idx = 0;
        return entries[idx];
    }
}

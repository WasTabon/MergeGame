using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneInfo
{
    public string id = "stone_cave";
    public string displayName = "Stone Cave";
    public Color bgTopColor = new Color(0.1f, 0.1f, 0.18f);
    public Color bgBottomColor = new Color(0.05f, 0.05f, 0.1f);
    public Color accentColor = new Color(0.55f, 0.55f, 0.6f);
    public int requiredPickaxeLevel = 1;
    public List<int> blockSequence = new List<int>();
    public float hpMultiplier = 1f;
    public float rewardMultiplier = 1f;
    public int gemsReward = 1;
}

[CreateAssetMenu(menuName = "MergeMining/ZoneConfig", fileName = "ZoneConfig")]
public class ZoneData : ScriptableObject
{
    public List<ZoneInfo> zones = new List<ZoneInfo>();

    public ZoneInfo GetZone(int index)
    {
        if (zones.Count == 0) return null;
        if (index < 0) index = 0;
        if (index >= zones.Count) index = zones.Count - 1;
        return zones[index];
    }

    public int Count => zones.Count;
}

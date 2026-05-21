using System.Collections.Generic;
using UnityEngine;

public enum BoosterType { SpeedX2, RewardsX2, AutoMerge, InstantDestroy }

[System.Serializable]
public class BoosterInfo
{
    public BoosterType type;
    public string displayName = "Booster";
    public Color color = Color.white;
    public float duration = 30f;
    public int gemsCost = 10;
}

[CreateAssetMenu(menuName = "MergeMining/BoosterConfig", fileName = "BoosterConfig")]
public class BoosterData : ScriptableObject
{
    public List<BoosterInfo> boosters = new List<BoosterInfo>();

    public BoosterInfo Get(BoosterType type)
    {
        foreach (var b in boosters) if (b.type == type) return b;
        return null;
    }
}

using System.Collections.Generic;
using UnityEngine;

public enum AchievementCondition
{
    BlocksDestroyed,
    PickaxesPurchased,
    HighestPickaxeLevel,
    ZonesUnlocked,
    ChestsOpened,
    BoostersUsed
}

[System.Serializable]
public class AchievementDefinition
{
    public string id;
    public string title;
    public string description;
    public AchievementCondition condition;
    public int target;
    public int gemsReward = 1;
}

[CreateAssetMenu(menuName = "MergeMining/AchievementConfig", fileName = "AchievementConfig")]
public class AchievementData : ScriptableObject
{
    public List<AchievementDefinition> achievements = new List<AchievementDefinition>();
}

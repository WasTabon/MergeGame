using UnityEngine;

public static class AchievementConfigProvider
{
    private static AchievementData cached;
    public static AchievementData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<AchievementData>("AchievementConfig");
                Debug.Assert(cached != null, "AchievementConfig.asset not found!");
            }
            return cached;
        }
    }
}

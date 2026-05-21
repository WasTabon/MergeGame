using UnityEngine;

public static class DailyRewardConfigProvider
{
    private static DailyRewardData cached;
    public static DailyRewardData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<DailyRewardData>("DailyRewardConfig");
                Debug.Assert(cached != null, "DailyRewardConfig.asset not found!");
            }
            return cached;
        }
    }
}

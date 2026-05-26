using UnityEngine;

public static class LevelConfigProvider
{
    private static LevelData cached;
    public static LevelData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<LevelData>("LevelConfig");
                Debug.Assert(cached != null, "LevelConfig.asset not found in Resources!");
            }
            return cached;
        }
    }
}

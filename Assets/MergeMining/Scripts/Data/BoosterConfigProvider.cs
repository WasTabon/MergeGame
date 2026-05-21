using UnityEngine;

public static class BoosterConfigProvider
{
    private static BoosterData cached;
    public static BoosterData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<BoosterData>("BoosterConfig");
                Debug.Assert(cached != null, "BoosterConfig.asset not found in Resources!");
            }
            return cached;
        }
    }
}

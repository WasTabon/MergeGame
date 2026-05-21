using UnityEngine;

public static class ChestConfigProvider
{
    private static ChestData cached;
    public static ChestData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<ChestData>("ChestConfig");
                Debug.Assert(cached != null, "ChestConfig.asset not found in Resources!");
            }
            return cached;
        }
    }
}

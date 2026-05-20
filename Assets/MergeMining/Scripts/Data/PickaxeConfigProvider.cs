using UnityEngine;

public static class PickaxeConfigProvider
{
    private static PickaxeData cached;

    public static PickaxeData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<PickaxeData>("PickaxeConfig");
                Debug.Assert(cached != null, "PickaxeConfig.asset not found in Resources folder!");
            }
            return cached;
        }
    }
}

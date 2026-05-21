using UnityEngine;

public static class ZoneConfigProvider
{
    private static ZoneData cached;
    public static ZoneData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<ZoneData>("ZoneConfig");
                Debug.Assert(cached != null, "ZoneConfig.asset not found in Resources!");
            }
            return cached;
        }
    }
}

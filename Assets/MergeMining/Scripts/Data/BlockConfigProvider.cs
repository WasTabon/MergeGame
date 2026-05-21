using UnityEngine;

public static class BlockConfigProvider
{
    private static BlockData cached;

    public static BlockData Config
    {
        get
        {
            if (cached == null)
            {
                cached = Resources.Load<BlockData>("BlockConfig");
                Debug.Assert(cached != null, "BlockConfig.asset not found in Resources folder!");
            }
            return cached;
        }
    }
}

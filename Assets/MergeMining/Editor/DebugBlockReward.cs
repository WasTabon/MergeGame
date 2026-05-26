#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text;

public static class DebugBlockReward
{
    [MenuItem("Tools/Merge Mining/DEBUG - Block Rewards State")]
    public static void Dump()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("=== BLOCK REWARDS DEBUG ===");

        FlyEffectSpawner spawner = Object.FindObjectOfType<FlyEffectSpawner>(true);
        if (spawner == null)
        {
            sb.AppendLine("FlyEffectSpawner: NOT FOUND on scene!");
        }
        else
        {
            sb.AppendLine("FlyEffectSpawner found on: " + spawner.gameObject.name + ", activeInHierarchy=" + spawner.gameObject.activeInHierarchy);
            sb.AppendLine("FlyEffectSpawner.Instance != null at edit-time: " + (FlyEffectSpawner.Instance != null));
        }

        CurrencyManager cm = Object.FindObjectOfType<CurrencyManager>(true);
        sb.AppendLine("CurrencyManager: " + (cm == null ? "NULL" : "OK, Coins=" + cm.Coins));

        GameObject coinsIcon = GameObject.Find("CoinsIcon");
        sb.AppendLine("CoinsIcon found: " + (coinsIcon != null));

        BlockData bd = Resources.Load<BlockData>("BlockConfig");
        if (bd == null) { sb.AppendLine("BlockConfig: NOT FOUND"); }
        else
        {
            sb.AppendLine("BlockConfig types count: " + bd.types.Count);
            for (int i = 0; i < bd.types.Count; i++)
            {
                var t = bd.types[i];
                int reward = Mathf.Max(1, Mathf.RoundToInt(3f * t.rewardMultiplier));
                sb.AppendLine("  [" + i + "] " + t.id + " - rewardMult=" + t.rewardMultiplier + " -> coins=" + reward);
            }
        }

        Debug.Log(sb.ToString());
    }
}
#endif

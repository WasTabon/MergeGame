#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration17_Setup
{
    private const string LEVEL_CONFIG_PATH = "Assets/MergeMining/Resources/LevelConfig.asset";
    private const string BLOCK_CONFIG_PATH = "Assets/MergeMining/Resources/BlockConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 17) Update ALL")]
    public static void UpdateAll()
    {
        UpdateBlockConfig();
        UpdateLevelConfig();
        Debug.Log("Iteration 17 ALL done. New block types added + level config updated.");
    }

    private static void UpdateBlockConfig()
    {
        BlockData data = AssetDatabase.LoadAssetAtPath<BlockData>(BLOCK_CONFIG_PATH);
        if (data == null) { Debug.LogError("BlockConfig.asset not found"); return; }

        data.types = new List<BlockTypeData>
        {
            new BlockTypeData
            {
                id = "stone", displayName = "Stone",
                color = new Color(0.62f, 0.62f, 0.65f), darkColor = new Color(0.35f, 0.35f, 0.38f),
                hpMultiplier = 1f, rewardMultiplier = 1f,
                behavior = BlockBehavior.Normal, damageMultiplier = 1f
            },
            new BlockTypeData
            {
                id = "iron", displayName = "Iron",
                color = new Color(0.45f, 0.45f, 0.55f), darkColor = new Color(0.25f, 0.25f, 0.3f),
                hpMultiplier = 1.2f, rewardMultiplier = 1.3f,
                behavior = BlockBehavior.Iron, minPickaxeLevel = 3, damageMultiplier = 1f
            },
            new BlockTypeData
            {
                id = "diamond", displayName = "Diamond",
                color = new Color(0.55f, 0.85f, 0.95f), darkColor = new Color(0.3f, 0.6f, 0.7f),
                hpMultiplier = 0.9f, rewardMultiplier = 2f,
                behavior = BlockBehavior.Diamond, damageMultiplier = 0.5f
            },
            new BlockTypeData
            {
                id = "explosive", displayName = "Explosive",
                color = new Color(0.9f, 0.3f, 0.25f), darkColor = new Color(0.5f, 0.15f, 0.1f),
                hpMultiplier = 0.7f, rewardMultiplier = 1.5f,
                behavior = BlockBehavior.Explosive, damageMultiplier = 1f, explosionDamage = 5f
            },
            new BlockTypeData
            {
                id = "healer", displayName = "Healer",
                color = new Color(0.45f, 0.85f, 0.4f), darkColor = new Color(0.25f, 0.5f, 0.2f),
                hpMultiplier = 0.8f, rewardMultiplier = 1.2f,
                behavior = BlockBehavior.Healer, damageMultiplier = 1f,
                healPerSecond = 4f, healInterval = 2f
            }
        };

        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("BlockConfig updated: 5 types (Stone, Iron, Diamond, Explosive, Healer).");
    }

    private static void UpdateLevelConfig()
    {
        LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(LEVEL_CONFIG_PATH);
        if (data == null) { Debug.LogError("LevelConfig.asset not found"); return; }

        float[] descend = { 8f, 10f, 12f, 14f, 18f, 22f, 26f, 30f, 33f, 35f, 38f, 40f, 42f, 44f, 45f };
        List<int>[] sequences = new List<int>[15];

        sequences[0] = new List<int> { 0, 0, 2, 0 };
        sequences[1] = new List<int> { 0, 2, 0, 3, 0 };
        sequences[2] = new List<int> { 0, 1, 2, 0, 4 };
        sequences[3] = new List<int> { 0, 3, 1, 0, 2, 4 };
        sequences[4] = new List<int> { 1, 0, 4, 2, 3, 0 };
        sequences[5] = new List<int> { 0, 1, 3, 4, 2, 1 };
        sequences[6] = new List<int> { 4, 1, 0, 3, 2, 1, 4 };
        sequences[7] = new List<int> { 1, 3, 4, 0, 2, 1, 3 };
        sequences[8] = new List<int> { 1, 4, 3, 2, 1, 0, 4, 3 };
        sequences[9] = new List<int> { 2, 1, 4, 3, 1, 4, 0, 2 };
        sequences[10] = new List<int> { 3, 1, 4, 2, 1, 3, 4, 1 };
        sequences[11] = new List<int> { 1, 3, 4, 2, 1, 4, 3, 2, 1 };
        sequences[12] = new List<int> { 4, 1, 3, 2, 1, 4, 3, 1, 2 };
        sequences[13] = new List<int> { 1, 4, 3, 2, 1, 3, 4, 2, 1, 3 };
        sequences[14] = new List<int> { 3, 4, 1, 2, 4, 1, 3, 2, 1, 4, 3 };

        for (int i = 0; i < 15; i++)
        {
            if (i < 2)
            {
                for (int k = 0; k < sequences[i].Count; k++)
                {
                    if (sequences[i][k] == 1) sequences[i][k] = 0;
                }
            }
        }

        for (int i = 0; i < data.levels.Count && i < 15; i++)
        {
            data.levels[i].blockDescendSpeed = descend[i];
            data.levels[i].blockSequence = sequences[i];
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("LevelConfig updated: descend speed from lvl 1, mixed block types per level.");
    }
}
#endif

#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;

public static class Iteration22_Setup
{
    private const string SPRITES_DIR = "Assets/MergeMining/Sprites";
    private const string PICKAXE_CONFIG_PATH = "Assets/MergeMining/Resources/PickaxeConfig.asset";
    private const string BLOCK_CONFIG_PATH = "Assets/MergeMining/Resources/BlockConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 22) Auto-Bind Sprites")]
    public static void AutoBind()
    {
        BindPickaxeSprites();
        BindBlockSprites();
        AssetDatabase.SaveAssets();
        Debug.Log("Iteration 22: sprite auto-binding done.");
    }

    private static void BindPickaxeSprites()
    {
        PickaxeData data = AssetDatabase.LoadAssetAtPath<PickaxeData>(PICKAXE_CONFIG_PATH);
        if (data == null) { Debug.LogError("PickaxeConfig.asset not found"); return; }

        int total = data.levels.Count;
        int bound = 0;
        for (int i = 0; i < total; i++)
        {
            int spriteNumber = total - i;
            string fileName = "lvl-" + spriteNumber.ToString("00");
            Sprite sp = FindSpriteByName(fileName);
            if (sp != null)
            {
                data.levels[i].spriteOverride = sp;
                bound++;
            }
            else
            {
                Debug.LogWarning("Pickaxe sprite not found: " + fileName);
            }
        }
        EditorUtility.SetDirty(data);
        Debug.Log("Pickaxe sprites bound: " + bound + "/" + total + " (lvl-15 → Pickaxe Lv1, lvl-14 → Lv2, ... lvl-01 → Lv15)");
    }

    private static void BindBlockSprites()
    {
        BlockData data = AssetDatabase.LoadAssetAtPath<BlockData>(BLOCK_CONFIG_PATH);
        if (data == null) { Debug.LogError("BlockConfig.asset not found"); return; }

        int bound = 0;
        foreach (var t in data.types)
        {
            string spriteName = GetSpriteNameForBlockId(t.id);
            if (string.IsNullOrEmpty(spriteName))
            {
                Debug.Log("Block '" + t.id + "' — no sprite override (will use placeholder color)");
                continue;
            }
            Sprite sp = FindSpriteByName(spriteName);
            if (sp != null)
            {
                t.spriteOverride = sp;
                bound++;
                Debug.Log("Block '" + t.id + "' bound to sprite '" + spriteName + "'");
            }
            else
            {
                Debug.LogWarning("Block sprite '" + spriteName + "' for type '" + t.id + "' NOT FOUND");
            }
        }
        EditorUtility.SetDirty(data);
        Debug.Log("Block sprites bound: " + bound + "/" + data.types.Count);
    }

    private static string GetSpriteNameForBlockId(string id)
    {
        switch (id)
        {
            case "stone": return "stone";
            case "iron": return "iron";
            case "diamond": return "iron-1";
            case "explosive": return "explosive";
            case "healer": return "healer";
            case "shell": return "shell";
            case "boss": return "boss";
            case "armored": return "shell";
            default: return null;
        }
    }

    private static Sprite FindSpriteByName(string name)
    {
        if (!Directory.Exists(SPRITES_DIR))
        {
            Debug.LogError("Sprites directory not found: " + SPRITES_DIR);
            return null;
        }
        string[] guids = AssetDatabase.FindAssets("t:Sprite " + name, new string[] { SPRITES_DIR });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName == name)
            {
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
        }
        return null;
    }
}
#endif

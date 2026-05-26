#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration20_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string LEVEL_CONFIG_PATH = "Assets/MergeMining/Resources/LevelConfig.asset";
    private const string PICKAXE_CONFIG_PATH = "Assets/MergeMining/Resources/PickaxeConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 20) Update ALL")]
    public static void UpdateAll()
    {
        UpdateLevelConfigCoins();
        UpdatePickaxeDurability();
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        RebuildTutorialGroups();
        ReattachStarter();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 20 done. Tutorial expanded + balance tweaked.");
    }

    [MenuItem("Tools/Merge Mining/(Iteration 20) DEBUG - Reset All Tutorials")]
    public static void ResetAllTuts()
    {
        TextTutorialPopup.ResetAll();
        Debug.Log("All tutorial flags reset.");
    }

    private static void UpdateLevelConfigCoins()
    {
        LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(LEVEL_CONFIG_PATH);
        if (data == null) { Debug.LogError("LevelConfig not found"); return; }

        for (int i = 0; i < data.levels.Count; i++)
        {
            int oldCoins = data.levels[i].startingCoins;
            data.levels[i].startingCoins = Mathf.RoundToInt(oldCoins * 1.5f);
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("LevelConfig starting coins boosted by 50%.");
    }

    private static void UpdatePickaxeDurability()
    {
        PickaxeData data = AssetDatabase.LoadAssetAtPath<PickaxeData>(PICKAXE_CONFIG_PATH);
        if (data == null) { Debug.LogError("PickaxeConfig not found"); return; }

        for (int i = 0; i < data.levels.Count; i++)
        {
            int old = data.levels[i].durability;
            data.levels[i].durability = Mathf.RoundToInt(old * 1.3f);
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("Pickaxe durability boosted by 30%.");
    }

    private static void RebuildTutorialGroups()
    {
        TextTutorialPopup popup = Object.FindObjectOfType<TextTutorialPopup>(true);
        if (popup == null)
        {
            Debug.LogError("TextTutorialPopup not found! Run Iter 15 setup first.");
            return;
        }

        SerializedObject so = new SerializedObject(popup);
        SerializedProperty groupsProp = so.FindProperty("groups");
        groupsProp.arraySize = 3;

        var group1Steps = new List<(string title, string body)>
        {
            ("WELCOME!", "Welcome to Merge Mining! Each level has a fixed number of blocks you need to destroy."),
            ("BUY PICKAXES", "Tap the SHOP button to buy a Lvl 1 pickaxe with your coins.\n\nEach level gives you a coin budget — spend it wisely!"),
            ("MERGE PICKAXES", "Drag a pickaxe onto another of the same level to merge into a stronger one.\n\nHigher level pickaxes deal much more damage!"),
            ("START THE BATTLE", "When you're ready, tap the green START button.\n\nYour pickaxes will automatically attack the blocks above."),
            ("WIN THE LEVEL", "Destroy all blocks to win!\n\nThe more pickaxes you save, the more stars you get.\n\nGood luck!")
        };
        FillGroup(groupsProp.GetArrayElementAtIndex(0), 1, "text_tutorial_done_1", group1Steps);

        var group2Steps = new List<(string title, string body)>
        {
            ("TIMER", "Each level has a time limit. If you run out, you lose!\n\nThe timer is at the top-left during battle."),
            ("FALLING BLOCKS", "Blocks slowly descend toward your pickaxes.\n\nIf they reach the grid — you lose. Move fast!"),
            ("DURABILITY", "Each pickaxe has limited durability (green bar at the bottom).\n\nAfter many attacks, it breaks and disappears."),
            ("BLOCK REGEN", "If you don't damage a block for ~1.5 seconds, its HP regenerates!\n\nFocus your fire."),
            ("SPECIAL BLOCKS", "Iron - only Lvl 3+ pickaxes can damage it\nDiamond - takes only 50% damage\nExplosive - hurts pickaxes when broken\nHealer - heals nearby blocks\nArmored - reduces damage by 3\nShell - regen after outer crack\nBOSS - destroys your weakest pickaxe every 6s"),
        };
        FillGroup(groupsProp.GetArrayElementAtIndex(1), 2, "text_tutorial_done_2", group2Steps);

        var group3Steps = new List<(string title, string body)>
        {
            ("MODIFIERS", "Before each level you'll choose 1 of 3 random modifiers.\n\nEach gives a bonus AND a drawback. Pick what fits your strategy!"),
            ("SACRIFICE", "In battle, the pink SACRIFICE button on the right destroys your strongest pickaxe.\n\nIn exchange — every block on the field dies instantly!"),
            ("READY!", "You now know all the rules. Time to crush some blocks!")
        };
        FillGroup(groupsProp.GetArrayElementAtIndex(2), 3, "text_tutorial_done_3", group3Steps);

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(popup);
        Debug.Log("Tutorial groups rebuilt: 3 groups for levels 1, 2, 3.");
    }

    private static void FillGroup(SerializedProperty groupProp, int level, string doneKey, List<(string title, string body)> steps)
    {
        groupProp.FindPropertyRelative("triggerLevel").intValue = level;
        groupProp.FindPropertyRelative("doneKey").stringValue = doneKey;
        SerializedProperty stepsProp = groupProp.FindPropertyRelative("steps");
        stepsProp.arraySize = steps.Count;
        for (int i = 0; i < steps.Count; i++)
        {
            SerializedProperty stepProp = stepsProp.GetArrayElementAtIndex(i);
            stepProp.FindPropertyRelative("title").stringValue = steps[i].title;
            stepProp.FindPropertyRelative("body").stringValue = steps[i].body;
        }
    }

    private static void ReattachStarter()
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null) return;

        ModifierChoiceStarter starter = canvasGo.GetComponent<ModifierChoiceStarter>();
        if (starter == null) return;

        TextTutorialPopup tutPopup = Object.FindObjectOfType<TextTutorialPopup>(true);
        if (tutPopup == null) return;

        SerializedObject so = new SerializedObject(starter);
        SerializedProperty tp = so.FindProperty("tutorialPopup");
        if (tp != null) tp.objectReferenceValue = tutPopup;
        so.ApplyModifiedProperties();
        Debug.Log("ModifierChoiceStarter reattached with tutorialPopup ref.");
    }
}
#endif

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration05_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string CONFIG_DIR = "Assets/MergeMining/Resources";
    private const string ZONE_CONFIG_PATH = "Assets/MergeMining/Resources/ZoneConfig.asset";
    private const string BLOCK_CONFIG_PATH = "Assets/MergeMining/Resources/BlockConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 5) Update Game Scene")]
    public static void UpdateGameScene()
    {
        CreateZoneConfig();
        UpgradeBlockConfig();

        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        GameObject canvasGo = GameObject.Find("Canvas");
        Debug.Assert(canvasGo != null);

        Image bgImage = FindBackgroundImage(canvasGo);
        GameObject bgGradient = EnsureGradientBackground(canvasGo, bgImage);
        GameObject zoneHud = EnsureZoneHud(canvasGo);
        UpgradeZoneCompletePopupVisual();

        AttachZoneManager(canvasGo, bgGradient, zoneHud);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 5: Game scene updated.");
    }

    private static Image FindBackgroundImage(GameObject canvas)
    {
        Transform t = canvas.transform.Find("Background");
        if (t == null) return null;
        return t.GetComponent<Image>();
    }

    private static GameObject EnsureGradientBackground(GameObject canvasGo, Image originalBg)
    {
        Transform existing = canvasGo.transform.Find("ZoneBackground");
        if (existing != null) return existing.gameObject;

        if (originalBg != null) originalBg.gameObject.SetActive(false);

        GameObject root = new GameObject("ZoneBackground");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(canvasGo.transform, false);
        rt.SetSiblingIndex(0);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        GameObject top = UIBuildUtils.CreateImage("BgTop", root.transform, new Color(0.1f, 0.1f, 0.18f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(top, new Vector2(0f, 0.5f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        top.GetComponent<Image>().raycastTarget = false;

        GameObject bottom = UIBuildUtils.CreateImage("BgBottom", root.transform, new Color(0.05f, 0.05f, 0.1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(bottom, new Vector2(0f, 0f), new Vector2(1f, 0.5f), Vector2.zero, Vector2.zero);
        bottom.GetComponent<Image>().raycastTarget = false;

        return root;
    }

    private static GameObject EnsureZoneHud(GameObject canvasGo)
    {
        GameObject existing = GameObject.Find("ZoneHud");
        if (existing != null) return existing;

        Transform safeAreaT = canvasGo.transform.Find("SafeArea");
        Transform parent = safeAreaT != null ? safeAreaT : canvasGo.transform;

        GameObject root = new GameObject("ZoneHud");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -180f);
        rt.sizeDelta = new Vector2(700f, 100f);

        GameObject panel = UIBuildUtils.CreateImage("Panel", root.transform, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(panel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject zoneName = UIBuildUtils.CreateText("ZoneName", panel.transform, "STONE CAVE", 44, Color.white);
        UIBuildUtils.SetRect(zoneName, new Vector2(0f, 0.4f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        zoneName.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject zoneIndex = UIBuildUtils.CreateText("ZoneIndex", panel.transform, "ZONE 1/5", 28, new Color(1f, 1f, 1f, 0.7f));
        UIBuildUtils.SetRect(zoneIndex, new Vector2(0f, 0f), new Vector2(1f, 0.4f), Vector2.zero, Vector2.zero);

        return root;
    }

    private static void UpgradeZoneCompletePopupVisual()
    {
        ZoneCompletePopup popup = Object.FindObjectOfType<ZoneCompletePopup>(true);
        if (popup == null)
        {
            Debug.LogWarning("ZoneCompletePopup not found. Run Iteration 4 first.");
            return;
        }

        Transform content = popup.transform.Find("Content");
        if (content == null) return;

        Transform existingCompleted = content.Find("CompletedZoneName");
        Transform existingNext = content.Find("NextZoneName");

        TextMeshProUGUI completedTmp;
        if (existingCompleted == null)
        {
            GameObject completed = UIBuildUtils.CreateText("CompletedZoneName", content, "STONE CAVE COMPLETED", 42, UIColors.ACCENT);
            UIBuildUtils.SetSize(completed, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -460f), new Vector2(750f, 70f));
            completed.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            completedTmp = completed.GetComponent<TextMeshProUGUI>();
        }
        else completedTmp = existingCompleted.GetComponent<TextMeshProUGUI>();

        TextMeshProUGUI nextTmp;
        if (existingNext == null)
        {
            GameObject next = UIBuildUtils.CreateText("NextZoneName", content, "NEXT: IRON MINE", 34, new Color(1f, 1f, 1f, 0.85f));
            UIBuildUtils.SetSize(next, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 350f), new Vector2(750f, 60f));
            nextTmp = next.GetComponent<TextMeshProUGUI>();
        }
        else nextTmp = existingNext.GetComponent<TextMeshProUGUI>();

        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("completedZoneNameText").objectReferenceValue = completedTmp;
        so.FindProperty("nextZoneNameText").objectReferenceValue = nextTmp;
        so.ApplyModifiedProperties();
    }

    private static void AttachZoneManager(GameObject canvasGo, GameObject bgGradient, GameObject zoneHud)
    {
        ZoneManager mgr = Object.FindObjectOfType<ZoneManager>();
        if (mgr == null) mgr = canvasGo.AddComponent<ZoneManager>();

        Image bgTop = bgGradient.transform.Find("BgTop").GetComponent<Image>();
        Image bgBottom = bgGradient.transform.Find("BgBottom").GetComponent<Image>();
        TextMeshProUGUI zoneNameTmp = zoneHud.transform.Find("Panel/ZoneName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI zoneIndexTmp = zoneHud.transform.Find("Panel/ZoneIndex").GetComponent<TextMeshProUGUI>();
        ZoneCompletePopup zonePopup = Object.FindObjectOfType<ZoneCompletePopup>(true);

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("bgTopImage").objectReferenceValue = bgTop;
        so.FindProperty("bgBottomImage").objectReferenceValue = bgBottom;
        so.FindProperty("zoneNameText").objectReferenceValue = zoneNameTmp;
        so.FindProperty("zoneIndexText").objectReferenceValue = zoneIndexTmp;
        so.FindProperty("zoneCompletePopup").objectReferenceValue = zonePopup;
        so.ApplyModifiedProperties();
    }

    private static void UpgradeBlockConfig()
    {
        BlockData data = AssetDatabase.LoadAssetAtPath<BlockData>(BLOCK_CONFIG_PATH);
        if (data == null)
        {
            CreateBlockConfigFromScratch();
            return;
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
    }

    private static void CreateBlockConfigFromScratch()
    {
        if (!Directory.Exists(CONFIG_DIR)) Directory.CreateDirectory(CONFIG_DIR);
        BlockData data = ScriptableObject.CreateInstance<BlockData>();
        BlockTypeData stone = new BlockTypeData { id = "stone", displayName = "Stone", color = new Color(0.55f, 0.55f, 0.6f), darkColor = new Color(0.35f, 0.35f, 0.4f), hpMultiplier = 1f, rewardMultiplier = 1f };
        BlockTypeData iron = new BlockTypeData { id = "iron", displayName = "Iron", color = new Color(0.65f, 0.45f, 0.3f), darkColor = new Color(0.4f, 0.28f, 0.18f), hpMultiplier = 1.4f, rewardMultiplier = 1.6f };
        BlockTypeData gold = new BlockTypeData { id = "gold", displayName = "Gold", color = new Color(0.95f, 0.78f, 0.25f), darkColor = new Color(0.7f, 0.55f, 0.15f), hpMultiplier = 2f, rewardMultiplier = 3f };
        BlockTypeData crystal = new BlockTypeData { id = "crystal", displayName = "Crystal", color = new Color(0.35f, 0.78f, 0.92f), darkColor = new Color(0.18f, 0.45f, 0.7f), hpMultiplier = 3f, rewardMultiplier = 5f };
        BlockTypeData lava = new BlockTypeData { id = "lava", displayName = "Lava", color = new Color(0.95f, 0.4f, 0.15f), darkColor = new Color(0.6f, 0.18f, 0.05f), hpMultiplier = 4f, rewardMultiplier = 7f };
        data.types = new List<BlockTypeData> { stone, iron, gold, crystal, lava };
        data.defaultSequence = new List<int> { 0, 0, 1, 0, 0, 1, 2, 0, 1, 0, 1, 2 };
        data.baseHP = 10f; data.hpGrowth = 1.18f; data.baseReward = 3; data.rewardGrowth = 1.15f;
        AssetDatabase.CreateAsset(data, BLOCK_CONFIG_PATH);
        AssetDatabase.SaveAssets();
    }

    private static void CreateZoneConfig()
    {
        if (!Directory.Exists(CONFIG_DIR)) Directory.CreateDirectory(CONFIG_DIR);

        ZoneData existing = AssetDatabase.LoadAssetAtPath<ZoneData>(ZONE_CONFIG_PATH);
        if (existing != null) return;

        ZoneData data = ScriptableObject.CreateInstance<ZoneData>();
        data.zones = new List<ZoneInfo>
        {
            new ZoneInfo {
                id = "stone_cave", displayName = "Stone Cave",
                bgTopColor = new Color(0.16f, 0.16f, 0.24f),
                bgBottomColor = new Color(0.08f, 0.08f, 0.14f),
                accentColor = new Color(0.65f, 0.65f, 0.7f),
                requiredPickaxeLevel = 1,
                blockSequence = new List<int> { 0, 0, 1, 0, 0, 1 },
                hpMultiplier = 1f, rewardMultiplier = 1f, gemsReward = 1
            },
            new ZoneInfo {
                id = "iron_mine", displayName = "Iron Mine",
                bgTopColor = new Color(0.28f, 0.18f, 0.12f),
                bgBottomColor = new Color(0.14f, 0.08f, 0.05f),
                accentColor = new Color(0.78f, 0.5f, 0.3f),
                requiredPickaxeLevel = 3,
                blockSequence = new List<int> { 1, 0, 1, 2, 1, 0 },
                hpMultiplier = 1.2f, rewardMultiplier = 1.4f, gemsReward = 2
            },
            new ZoneInfo {
                id = "gold_mine", displayName = "Gold Mine",
                bgTopColor = new Color(0.32f, 0.22f, 0.06f),
                bgBottomColor = new Color(0.18f, 0.12f, 0.03f),
                accentColor = new Color(0.95f, 0.78f, 0.25f),
                requiredPickaxeLevel = 6,
                blockSequence = new List<int> { 2, 1, 2, 1, 2, 0 },
                hpMultiplier = 1.5f, rewardMultiplier = 2f, gemsReward = 3
            },
            new ZoneInfo {
                id = "crystal_cave", displayName = "Crystal Cave",
                bgTopColor = new Color(0.1f, 0.18f, 0.32f),
                bgBottomColor = new Color(0.04f, 0.08f, 0.18f),
                accentColor = new Color(0.4f, 0.85f, 0.95f),
                requiredPickaxeLevel = 9,
                blockSequence = new List<int> { 3, 2, 3, 1, 3, 2 },
                hpMultiplier = 1.8f, rewardMultiplier = 2.5f, gemsReward = 5
            },
            new ZoneInfo {
                id = "lava_core", displayName = "Lava Core",
                bgTopColor = new Color(0.3f, 0.08f, 0.04f),
                bgBottomColor = new Color(0.15f, 0.03f, 0.02f),
                accentColor = new Color(0.95f, 0.45f, 0.15f),
                requiredPickaxeLevel = 12,
                blockSequence = new List<int> { 4, 3, 4, 2, 4, 3 },
                hpMultiplier = 2.2f, rewardMultiplier = 3.5f, gemsReward = 10
            },
        };
        AssetDatabase.CreateAsset(data, ZONE_CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("ZoneConfig.asset created with 5 zones");
    }
}
#endif

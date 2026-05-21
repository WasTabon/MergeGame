#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration10_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string CONFIG_DIR = "Assets/MergeMining/Resources";
    private const string DAILY_CONFIG_PATH = "Assets/MergeMining/Resources/DailyRewardConfig.asset";
    private const string ACHIEVEMENT_CONFIG_PATH = "Assets/MergeMining/Resources/AchievementConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 10) Update Game Scene")]
    public static void UpdateGameScene()
    {
        CreateDailyConfig();
        CreateAchievementConfig();

        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        DailyRewardPopup dailyPopup = BuildDailyRewardPopup();
        AchievementToast toast = BuildAchievementToast();

        AttachDailyRewardManager(dailyPopup);
        AttachAchievementManager(toast);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 10: Game scene updated. Game is feature-complete.");
    }

    private static DailyRewardPopup BuildDailyRewardPopup()
    {
        GameObject old = GameObject.Find("DailyRewardPopup");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("DailyRewardPopupCanvas", 250);

        GameObject popupGo = new GameObject("DailyRewardPopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 1100f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "DAILY REWARD", 80, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -130f), new Vector2(800f, 110f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject dayText = UIBuildUtils.CreateText("DayText", content.transform, "DAY 1", 50, new Color(1f, 1f, 1f, 0.85f));
        UIBuildUtils.SetSize(dayText, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -250f), new Vector2(700f, 70f));

        GameObject rewardIcon = UIBuildUtils.CreateImage("RewardIcon", content.transform, new Color(1f, 0.85f, 0.4f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(rewardIcon, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(260f, 260f));
        rewardIcon.GetComponent<Image>().raycastTarget = false;

        GameObject rewardAmount = UIBuildUtils.CreateText("RewardAmount", content.transform, "+100", 90, Color.white);
        UIBuildUtils.SetSize(rewardAmount, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -150f), new Vector2(700f, 110f));
        rewardAmount.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject claimBtn = UIBuildUtils.CreateButton("ClaimButton", content.transform, "CLAIM", UIColors.GREEN, Color.white, 56);
        UIBuildUtils.SetSize(claimBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 120f), new Vector2(600f, 160f));

        DailyRewardPopup popup = popupGo.AddComponent<DailyRewardPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("dayText").objectReferenceValue = dayText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("rewardIcon").objectReferenceValue = rewardIcon.GetComponent<RectTransform>();
        so.FindProperty("rewardIconImage").objectReferenceValue = rewardIcon.GetComponent<Image>();
        so.FindProperty("rewardAmountText").objectReferenceValue = rewardAmount.GetComponent<TextMeshProUGUI>();
        so.FindProperty("claimButton").objectReferenceValue = claimBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static AchievementToast BuildAchievementToast()
    {
        GameObject old = GameObject.Find("AchievementToast");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("AchievementToastCanvas", 260);

        GameObject root = new GameObject("AchievementToast");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(canvasGo.transform, false);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        CanvasGroup cg = root.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        GameObject content = UIBuildUtils.CreateImage("Content", root.transform, new Color(0.1f, 0.1f, 0.18f, 0.95f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -380f), new Vector2(900f, 200f));

        GameObject icon = UIBuildUtils.CreateImage("Icon", content.transform, UIColors.ACCENT, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(icon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(70f, 0f), new Vector2(120f, 120f));
        icon.GetComponent<Image>().raycastTarget = false;

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "UNLOCKED: ACHIEVEMENT", 38, UIColors.ACCENT, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(title, new Vector2(0f, 0.55f), new Vector2(1f, 1f), new Vector2(160f, 0f), new Vector2(-30f, 0f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject desc = UIBuildUtils.CreateText("Description", content.transform, "Description goes here", 30, new Color(1f, 1f, 1f, 0.85f), TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(desc, new Vector2(0f, 0.15f), new Vector2(1f, 0.55f), new Vector2(160f, 0f), new Vector2(-200f, 0f));

        GameObject reward = UIBuildUtils.CreateText("Reward", content.transform, "+1", 50, UIColors.GEM, TextAlignmentOptions.MidlineRight);
        UIBuildUtils.SetRect(reward, new Vector2(0f, 0.15f), new Vector2(1f, 0.55f), new Vector2(0f, 0f), new Vector2(-50f, 0f));
        reward.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        AchievementToast toast = root.AddComponent<AchievementToast>();
        SerializedObject so = new SerializedObject(toast);
        so.FindProperty("canvasGroup").objectReferenceValue = cg;
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("descriptionText").objectReferenceValue = desc.GetComponent<TextMeshProUGUI>();
        so.FindProperty("rewardText").objectReferenceValue = reward.GetComponent<TextMeshProUGUI>();
        so.FindProperty("showDuration").floatValue = 3f;
        so.ApplyModifiedProperties();

        return toast;
    }

    private static void AttachDailyRewardManager(DailyRewardPopup popup)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        DailyRewardManager mgr = Object.FindObjectOfType<DailyRewardManager>();
        if (mgr == null) mgr = canvasGo.AddComponent<DailyRewardManager>();

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("popup").objectReferenceValue = popup;
        so.ApplyModifiedProperties();
    }

    private static void AttachAchievementManager(AchievementToast toast)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        AchievementManager mgr = Object.FindObjectOfType<AchievementManager>();
        if (mgr == null) mgr = canvasGo.AddComponent<AchievementManager>();

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("toast").objectReferenceValue = toast;
        so.ApplyModifiedProperties();
    }

    private static void CreateDailyConfig()
    {
        if (!Directory.Exists(CONFIG_DIR)) Directory.CreateDirectory(CONFIG_DIR);
        DailyRewardData existing = AssetDatabase.LoadAssetAtPath<DailyRewardData>(DAILY_CONFIG_PATH);
        if (existing != null) return;

        DailyRewardData data = ScriptableObject.CreateInstance<DailyRewardData>();
        data.entries = new List<DailyRewardEntry>
        {
            new DailyRewardEntry { day = 1, kind = DailyRewardKind.Coins, amount = 100 },
            new DailyRewardEntry { day = 2, kind = DailyRewardKind.Coins, amount = 200 },
            new DailyRewardEntry { day = 3, kind = DailyRewardKind.Gems, amount = 3 },
            new DailyRewardEntry { day = 4, kind = DailyRewardKind.Coins, amount = 500 },
            new DailyRewardEntry { day = 5, kind = DailyRewardKind.PickaxeLevel, pickaxeLevel = 3 },
            new DailyRewardEntry { day = 6, kind = DailyRewardKind.Gems, amount = 10 },
            new DailyRewardEntry { day = 7, kind = DailyRewardKind.Gems, amount = 25 },
        };
        AssetDatabase.CreateAsset(data, DAILY_CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("DailyRewardConfig.asset created");
    }

    private static void CreateAchievementConfig()
    {
        if (!Directory.Exists(CONFIG_DIR)) Directory.CreateDirectory(CONFIG_DIR);
        AchievementData existing = AssetDatabase.LoadAssetAtPath<AchievementData>(ACHIEVEMENT_CONFIG_PATH);
        if (existing != null) return;

        AchievementData data = ScriptableObject.CreateInstance<AchievementData>();
        data.achievements = new List<AchievementDefinition>
        {
            new AchievementDefinition { id = "first_blocks_10", title = "Beginner Miner", description = "Destroy 10 blocks", condition = AchievementCondition.BlocksDestroyed, target = 10, gemsReward = 2 },
            new AchievementDefinition { id = "blocks_100", title = "Persistent Miner", description = "Destroy 100 blocks", condition = AchievementCondition.BlocksDestroyed, target = 100, gemsReward = 5 },
            new AchievementDefinition { id = "blocks_500", title = "Block Crusher", description = "Destroy 500 blocks", condition = AchievementCondition.BlocksDestroyed, target = 500, gemsReward = 10 },
            new AchievementDefinition { id = "pickaxes_5", title = "Stocking Up", description = "Buy 5 pickaxes", condition = AchievementCondition.PickaxesPurchased, target = 5, gemsReward = 2 },
            new AchievementDefinition { id = "pickaxes_50", title = "Big Spender", description = "Buy 50 pickaxes", condition = AchievementCondition.PickaxesPurchased, target = 50, gemsReward = 10 },
            new AchievementDefinition { id = "merge_lvl5", title = "Skilled Merger", description = "Reach pickaxe level 5", condition = AchievementCondition.HighestPickaxeLevel, target = 5, gemsReward = 3 },
            new AchievementDefinition { id = "merge_lvl10", title = "Merge Master", description = "Reach pickaxe level 10", condition = AchievementCondition.HighestPickaxeLevel, target = 10, gemsReward = 10 },
            new AchievementDefinition { id = "zones_3", title = "Explorer", description = "Unlock 3 zones", condition = AchievementCondition.ZonesUnlocked, target = 3, gemsReward = 5 },
            new AchievementDefinition { id = "chests_10", title = "Treasure Hunter", description = "Open 10 chests", condition = AchievementCondition.ChestsOpened, target = 10, gemsReward = 5 },
            new AchievementDefinition { id = "boosters_5", title = "Power User", description = "Use 5 boosters", condition = AchievementCondition.BoostersUsed, target = 5, gemsReward = 3 },
        };
        AssetDatabase.CreateAsset(data, ACHIEVEMENT_CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("AchievementConfig.asset created with 10 achievements");
    }
}
#endif

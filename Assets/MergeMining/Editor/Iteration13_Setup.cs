#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration13_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string CONFIG_DIR = "Assets/MergeMining/Resources";
    private const string LEVEL_CONFIG_PATH = "Assets/MergeMining/Resources/LevelConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 13) Update Game Scene")]
    public static void UpdateGameScene()
    {
        CreateLevelConfig();

        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        LevelVictoryPopup victoryPopup = BuildLevelVictoryPopup();
        GameObject levelHudGo = BuildLevelHud();
        WireLevelHud(levelHudGo);
        AttachLevelManager(victoryPopup, levelHudGo.GetComponent<LevelHud>());

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 13: Game scene updated with level system.");
    }

    private static LevelVictoryPopup BuildLevelVictoryPopup()
    {
        GameObject old = GameObject.Find("LevelVictoryPopup");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("LevelVictoryPopupCanvas", 225);

        GameObject popupGo = new GameObject("LevelVictoryPopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 1300f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "VICTORY", 110, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -130f), new Vector2(800f, 130f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject levelLabel = UIBuildUtils.CreateText("LevelText", content.transform, "LEVEL 1 COMPLETE", 50, Color.white);
        UIBuildUtils.SetSize(levelLabel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -260f), new Vector2(800f, 70f));

        GameObject starsRow = new GameObject("StarsRow");
        RectTransform srt = starsRow.AddComponent<RectTransform>();
        srt.SetParent(content.transform, false);
        srt.anchorMin = new Vector2(0.5f, 0.5f);
        srt.anchorMax = new Vector2(0.5f, 0.5f);
        srt.pivot = new Vector2(0.5f, 0.5f);
        srt.anchoredPosition = new Vector2(0f, 200f);
        srt.sizeDelta = new Vector2(700f, 220f);

        GameObject star1 = UIBuildUtils.CreateText("Star1", starsRow.transform, "★", 180, new Color(0.25f, 0.25f, 0.3f));
        UIBuildUtils.SetSize(star1, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-220f, 0f), new Vector2(220f, 220f));
        GameObject star2 = UIBuildUtils.CreateText("Star2", starsRow.transform, "★", 200, new Color(0.25f, 0.25f, 0.3f));
        UIBuildUtils.SetSize(star2, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 20f), new Vector2(240f, 240f));
        GameObject star3 = UIBuildUtils.CreateText("Star3", starsRow.transform, "★", 180, new Color(0.25f, 0.25f, 0.3f));
        UIBuildUtils.SetSize(star3, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(220f, 0f), new Vector2(220f, 220f));

        GameObject gemIcon = UIBuildUtils.CreateImage("GemIcon", content.transform, UIColors.GEM, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemIcon, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-90f, -50f), new Vector2(110f, 110f));
        gemIcon.GetComponent<Image>().raycastTarget = false;

        GameObject gemsReward = UIBuildUtils.CreateText("GemsReward", content.transform, "+1", 80, Color.white, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetSize(gemsReward, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(50f, -50f), new Vector2(280f, 100f));
        gemsReward.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject nextBtn = UIBuildUtils.CreateButton("NextButton", content.transform, "NEXT LEVEL", UIColors.GREEN, Color.white, 52);
        UIBuildUtils.SetSize(nextBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 300f), new Vector2(700f, 150f));

        GameObject restartBtn = UIBuildUtils.CreateButton("RestartButton", content.transform, "RESTART", UIColors.PRIMARY, Color.white, 44);
        UIBuildUtils.SetSize(restartBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180f, 130f), new Vector2(320f, 130f));

        GameObject menuBtn = UIBuildUtils.CreateButton("MenuButton", content.transform, "MENU", new Color(0.4f, 0.4f, 0.45f), Color.white, 44);
        UIBuildUtils.SetSize(menuBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(180f, 130f), new Vector2(320f, 130f));

        LevelVictoryPopup popup = popupGo.AddComponent<LevelVictoryPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("levelText").objectReferenceValue = levelLabel.GetComponent<TextMeshProUGUI>();
        so.FindProperty("star1Text").objectReferenceValue = star1.GetComponent<TextMeshProUGUI>();
        so.FindProperty("star2Text").objectReferenceValue = star2.GetComponent<TextMeshProUGUI>();
        so.FindProperty("star3Text").objectReferenceValue = star3.GetComponent<TextMeshProUGUI>();
        so.FindProperty("gemsRewardText").objectReferenceValue = gemsReward.GetComponent<TextMeshProUGUI>();
        so.FindProperty("gemIcon").objectReferenceValue = gemIcon.GetComponent<RectTransform>();
        so.FindProperty("nextButton").objectReferenceValue = nextBtn.GetComponent<Button>();
        so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        so.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static GameObject BuildLevelHud()
    {
        GameObject existing = GameObject.Find("LevelHud");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject canvasGo = GameObject.Find("Canvas");
        Transform safeAreaT = canvasGo.transform.Find("SafeArea");

        GameObject root = new GameObject("LevelHud");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(safeAreaT != null ? safeAreaT : canvasGo.transform, false);
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -190f);
        rt.sizeDelta = new Vector2(720f, 220f);

        GameObject panel = UIBuildUtils.CreateImage("Panel", root.transform, new Color(0f, 0f, 0f, 0.45f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(panel, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        panel.GetComponent<Image>().raycastTarget = false;

        GameObject levelText = UIBuildUtils.CreateText("LevelText", panel.transform, "LEVEL 1", 52, Color.white);
        UIBuildUtils.SetRect(levelText, new Vector2(0f, 0.65f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        levelText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject phaseText = UIBuildUtils.CreateText("PhaseText", panel.transform, "BUY & MERGE PICKAXES", 30, UIColors.ACCENT);
        UIBuildUtils.SetRect(phaseText, new Vector2(0f, 0.4f), new Vector2(1f, 0.65f), Vector2.zero, Vector2.zero);

        GameObject progressBg = UIBuildUtils.CreateImage("ProgressBg", panel.transform, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(progressBg, new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.32f), Vector2.zero, Vector2.zero);
        progressBg.GetComponent<Image>().raycastTarget = false;

        GameObject progressFill = UIBuildUtils.CreateImage("ProgressFill", progressBg.transform, new Color(0.4f, 0.78f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(progressFill, Vector2.zero, Vector2.one, new Vector2(4f, 4f), new Vector2(-4f, -4f));
        Image fillImg = progressFill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg.fillAmount = 0f;
        fillImg.raycastTarget = false;

        GameObject progressText = UIBuildUtils.CreateText("ProgressText", progressBg.transform, "0/5", 28, Color.white);
        UIBuildUtils.SetRect(progressText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        progressText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        progressText.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        GameObject startBtn = UIBuildUtils.CreateButton("StartBattleButton", canvasGo.transform, "START!", UIColors.GREEN, Color.white, 64);
        UIBuildUtils.SetSize(startBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 460f), new Vector2(640f, 160f));
        Transform startLabelT = startBtn.transform.Find("Label");
        TextMeshProUGUI startLabel = startLabelT != null ? startLabelT.GetComponent<TextMeshProUGUI>() : null;

        GameObject restartBtn = UIBuildUtils.CreateIconButton("RestartButton", root.transform, new Color(0f, 0f, 0f, 0.6f), "↻", 50);
        UIBuildUtils.SetSize(restartBtn, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-10f, -10f), new Vector2(90f, 90f));

        LevelHud hud = root.AddComponent<LevelHud>();
        SerializedObject so = new SerializedObject(hud);
        so.FindProperty("levelText").objectReferenceValue = levelText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("phaseText").objectReferenceValue = phaseText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("blocksProgressText").objectReferenceValue = progressText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("blocksProgressFill").objectReferenceValue = fillImg;
        so.FindProperty("startBattleButton").objectReferenceValue = startBtn.GetComponent<Button>();
        so.FindProperty("startButtonLabel").objectReferenceValue = startLabel;
        so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return root;
    }

    private static void WireLevelHud(GameObject hud) { }

    private static void AttachLevelManager(LevelVictoryPopup victoryPopup, LevelHud hud)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        LevelManager mgr = Object.FindObjectOfType<LevelManager>();
        if (mgr == null) mgr = canvasGo.AddComponent<LevelManager>();

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("victoryPopup").objectReferenceValue = victoryPopup;
        so.FindProperty("levelHud").objectReferenceValue = hud;
        so.ApplyModifiedProperties();
    }

    private static void CreateLevelConfig()
    {
        if (!Directory.Exists(CONFIG_DIR)) Directory.CreateDirectory(CONFIG_DIR);
        LevelData existing = AssetDatabase.LoadAssetAtPath<LevelData>(LEVEL_CONFIG_PATH);
        if (existing != null) return;

        LevelData data = ScriptableObject.CreateInstance<LevelData>();
        data.pickaxeBaseCost = 10;
        data.levels = new List<LevelDefinition>();

        int[] blockCounts = { 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 35 };
        int[] coins = { 30, 40, 50, 60, 80, 100, 120, 140, 170, 200, 240, 280, 320, 380, 450 };
        int[] maxLvl = { 2, 2, 3, 3, 4, 4, 5, 5, 6, 7, 8, 9, 10, 12, 15 };
        float[] hp = { 12f, 16f, 22f, 30f, 45f, 65f, 95f, 140f, 200f, 280f, 400f, 580f, 850f, 1200f, 2000f };
        int[] gems = { 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 7, 8, 10, 15 };

        for (int i = 0; i < 15; i++)
        {
            int n = i + 1;
            LevelDefinition lvl = new LevelDefinition
            {
                levelNumber = n,
                blocksToDestroy = blockCounts[i],
                startingCoins = coins[i],
                maxPickaxeLevel = maxLvl[i],
                blockHP = hp[i],
                blockSequence = new List<int> { 0 },
                zoneId = "stone_cave",
                gemsReward = gems[i]
            };
            data.levels.Add(lvl);
        }

        AssetDatabase.CreateAsset(data, LEVEL_CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("LevelConfig.asset created with 15 levels");
    }
}
#endif

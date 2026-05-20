#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class Iteration01_Setup
{
    private const string SCENES_DIR = "Assets/MergeMining/Scenes";
    private const string MENU_SCENE_PATH = "Assets/MergeMining/Scenes/MainMenu.unity";
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 1) Setup MainMenu Scene")]
    public static void SetupMainMenu()
    {
        EnsureScenesDir();
        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildMainMenu();
        EditorSceneManager.SaveScene(scene, MENU_SCENE_PATH);
        AddSceneToBuildSettings(MENU_SCENE_PATH);
        Debug.Log("MainMenu scene created at " + MENU_SCENE_PATH);
    }

    [MenuItem("Tools/Merge Mining/(Iteration 1) Setup Game Scene")]
    public static void SetupGameScene()
    {
        EnsureScenesDir();
        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildGameScene();
        EditorSceneManager.SaveScene(scene, GAME_SCENE_PATH);
        AddSceneToBuildSettings(GAME_SCENE_PATH);
        Debug.Log("Game scene created at " + GAME_SCENE_PATH);
    }

    [MenuItem("Tools/Merge Mining/(Iteration 1) Setup ALL Scenes")]
    public static void SetupAll()
    {
        SetupMainMenu();
        SetupGameScene();
        EditorSceneManager.OpenScene(MENU_SCENE_PATH);
        Debug.Log("ALL scenes set up. Build Settings updated. Start from MainMenu.");
    }

    private static void EnsureScenesDir()
    {
        if (!Directory.Exists(SCENES_DIR))
        {
            Directory.CreateDirectory(SCENES_DIR);
            AssetDatabase.Refresh();
        }
    }

    private static void AddSceneToBuildSettings(string path)
    {
        var existing = EditorBuildSettings.scenes;
        foreach (var s in existing)
        {
            if (s.path == path) return;
        }
        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(existing);
        list.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = list.ToArray();
    }

    private static GameObject BuildManagers()
    {
        GameObject managers = new GameObject("Managers");
        managers.AddComponent<GameInitializer>();
        managers.AddComponent<SoundManager>();
        managers.AddComponent<HapticManager>();
        managers.AddComponent<TransitionManager>();
        managers.AddComponent<CurrencyManager>();
        managers.AddComponent<PopupManager>();

        Camera cam = new GameObject("Main Camera").AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = UIBuildUtils.BG_DARK;
        cam.orthographic = true;
        cam.transform.position = new Vector3(0f, 0f, -10f);

        UIBuildUtils.CreateEventSystemIfNeeded();
        return managers;
    }

    private static void BuildMainMenu()
    {
        BuildManagers();

        GameObject canvasGo = UIBuildUtils.CreateCanvas("Canvas", 0);

        GameObject bg = UIBuildUtils.CreateImage("Background", canvasGo.transform, UIBuildUtils.BG_DARK);
        UIBuildUtils.SetRect(bg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject safeRoot = new GameObject("SafeArea");
        RectTransform safeRt = safeRoot.AddComponent<RectTransform>();
        safeRt.SetParent(canvasGo.transform, false);
        safeRt.anchorMin = Vector2.zero;
        safeRt.anchorMax = Vector2.one;
        safeRt.offsetMin = Vector2.zero;
        safeRt.offsetMax = Vector2.zero;
        safeRoot.AddComponent<SafeAreaFitter>();

        GameObject topHud = new GameObject("TopHUD");
        RectTransform topHudRt = topHud.AddComponent<RectTransform>();
        topHudRt.SetParent(safeRoot.transform, false);
        topHudRt.anchorMin = new Vector2(0f, 1f);
        topHudRt.anchorMax = new Vector2(1f, 1f);
        topHudRt.pivot = new Vector2(0.5f, 1f);
        topHudRt.anchoredPosition = new Vector2(0f, -30f);
        topHudRt.sizeDelta = new Vector2(0f, 140f);

        GameObject coinsPanel = UIBuildUtils.CreateImage("CoinsPanel", topHud.transform, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(coinsPanel, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(60f, 0f), new Vector2(320f, 100f));

        GameObject coinsIcon = UIBuildUtils.CreateImage("CoinsIcon", coinsPanel.transform, UIBuildUtils.ACCENT, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(coinsIcon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(50f, 0f), new Vector2(70f, 70f));

        GameObject coinsText = UIBuildUtils.CreateText("CoinsText", coinsPanel.transform, "0", 52, UIBuildUtils.WHITE, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(coinsText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(110f, 0f), new Vector2(-20f, 0f));

        GameObject gemsPanel = UIBuildUtils.CreateImage("GemsPanel", topHud.transform, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(gemsPanel, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-60f, 0f), new Vector2(320f, 100f));

        GameObject gemsIcon = UIBuildUtils.CreateImage("GemsIcon", gemsPanel.transform, new Color(0.4f, 0.85f, 0.9f, 1f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemsIcon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(50f, 0f), new Vector2(70f, 70f));

        GameObject gemsText = UIBuildUtils.CreateText("GemsText", gemsPanel.transform, "0", 52, UIBuildUtils.WHITE, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(gemsText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(110f, 0f), new Vector2(-20f, 0f));

        MenuHUD menuHud = canvasGo.AddComponent<MenuHUD>();
        SerializedObject menuHudSo = new SerializedObject(menuHud);
        menuHudSo.FindProperty("coinsText").objectReferenceValue = coinsText.GetComponent<TextMeshProUGUI>();
        menuHudSo.FindProperty("gemsText").objectReferenceValue = gemsText.GetComponent<TextMeshProUGUI>();
        menuHudSo.ApplyModifiedProperties();

        GameObject titleGo = UIBuildUtils.CreateText("Title", safeRoot.transform, "MERGE\nMINING", 160, UIBuildUtils.WHITE);
        TextMeshProUGUI titleTmp = titleGo.GetComponent<TextMeshProUGUI>();
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.outlineWidth = 0.3f;
        UIBuildUtils.SetSize(titleGo, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 400f), new Vector2(900f, 500f));

        GameObject playBtn = UIBuildUtils.CreateButton("PlayButton", safeRoot.transform, "PLAY", UIBuildUtils.PRIMARY, UIBuildUtils.WHITE, 80);
        UIBuildUtils.SetSize(playBtn, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -100f), new Vector2(560f, 180f));

        GameObject settingsBtn = UIBuildUtils.CreateIconButton("SettingsButton", safeRoot.transform, new Color(0f, 0f, 0f, 0.5f), "⚙", 64);
        UIBuildUtils.SetSize(settingsBtn, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -340f), new Vector2(160f, 160f));

        GameObject settingsPopupGo = BuildSettingsPopup(canvasGo.transform);

        MainMenuController controller = canvasGo.AddComponent<MainMenuController>();
        SerializedObject ctrlSo = new SerializedObject(controller);
        ctrlSo.FindProperty("playButton").objectReferenceValue = playBtn.GetComponent<Button>();
        ctrlSo.FindProperty("settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        ctrlSo.FindProperty("settingsPopup").objectReferenceValue = settingsPopupGo.GetComponent<SettingsPopup>();
        ctrlSo.ApplyModifiedProperties();
    }

    private static GameObject BuildSettingsPopup(Transform canvasParent)
    {
        GameObject popupCanvas = UIBuildUtils.CreateCanvas("PopupCanvas", 100);

        GameObject popupGo = new GameObject("SettingsPopup");
        RectTransform popupRt = popupGo.AddComponent<RectTransform>();
        popupRt.SetParent(popupCanvas.transform, false);
        popupRt.anchorMin = Vector2.zero;
        popupRt.anchorMax = Vector2.one;
        popupRt.offsetMin = Vector2.zero;
        popupRt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        ColorBlock cb = backdropBtn.colors;
        cb.normalColor = Color.white; cb.highlightedColor = Color.white;
        backdropBtn.colors = cb;
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 1100f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "SETTINGS", 80, UIBuildUtils.WHITE);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(700f, 100f));

        GameObject sfxToggle = UIBuildUtils.CreateToggle("SfxToggle", content.transform, "SFX", true);
        UIBuildUtils.SetSize(sfxToggle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -240f), new Vector2(700f, 100f));

        GameObject sfxSlider = UIBuildUtils.CreateSlider("SfxSlider", content.transform, 1f);
        UIBuildUtils.SetSize(sfxSlider, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -340f), new Vector2(700f, 60f));

        GameObject musicToggle = UIBuildUtils.CreateToggle("MusicToggle", content.transform, "MUSIC", true);
        UIBuildUtils.SetSize(musicToggle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -460f), new Vector2(700f, 100f));

        GameObject musicSlider = UIBuildUtils.CreateSlider("MusicSlider", content.transform, 0.6f);
        UIBuildUtils.SetSize(musicSlider, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -560f), new Vector2(700f, 60f));

        GameObject hapticToggle = UIBuildUtils.CreateToggle("HapticToggle", content.transform, "VIBRATION", true);
        UIBuildUtils.SetSize(hapticToggle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -680f), new Vector2(700f, 100f));

        GameObject restartBtn = UIBuildUtils.CreateButton("RestartProgressButton", content.transform, "RESET PROGRESS", UIBuildUtils.RED, UIBuildUtils.WHITE, 48);
        UIBuildUtils.SetSize(restartBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 260f), new Vector2(600f, 130f));

        GameObject closeBtn = UIBuildUtils.CreateButton("CloseButton", content.transform, "CLOSE", UIBuildUtils.PRIMARY, UIBuildUtils.WHITE, 56);
        UIBuildUtils.SetSize(closeBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 100f), new Vector2(600f, 140f));

        SettingsPopup popup = popupGo.AddComponent<SettingsPopup>();
        SerializedObject pso = new SerializedObject(popup);
        pso.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        pso.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        pso.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        pso.FindProperty("closeOnBackdrop").boolValue = true;
        pso.FindProperty("sfxToggle").objectReferenceValue = sfxToggle.GetComponent<Toggle>();
        pso.FindProperty("musicToggle").objectReferenceValue = musicToggle.GetComponent<Toggle>();
        pso.FindProperty("hapticToggle").objectReferenceValue = hapticToggle.GetComponent<Toggle>();
        pso.FindProperty("sfxSlider").objectReferenceValue = sfxSlider.GetComponent<Slider>();
        pso.FindProperty("musicSlider").objectReferenceValue = musicSlider.GetComponent<Slider>();
        pso.FindProperty("closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
        pso.FindProperty("restartProgressButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        pso.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        pso.ApplyModifiedProperties();

        return popupGo;
    }

    private static void BuildGameScene()
    {
        BuildManagers();

        GameObject canvasGo = UIBuildUtils.CreateCanvas("Canvas", 0);

        GameObject bg = UIBuildUtils.CreateImage("Background", canvasGo.transform, UIBuildUtils.BG_DARK);
        UIBuildUtils.SetRect(bg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject safeRoot = new GameObject("SafeArea");
        RectTransform safeRt = safeRoot.AddComponent<RectTransform>();
        safeRt.SetParent(canvasGo.transform, false);
        safeRt.anchorMin = Vector2.zero;
        safeRt.anchorMax = Vector2.one;
        safeRt.offsetMin = Vector2.zero;
        safeRt.offsetMax = Vector2.zero;
        safeRoot.AddComponent<SafeAreaFitter>();

        GameObject topHud = new GameObject("TopHUD");
        RectTransform topHudRt = topHud.AddComponent<RectTransform>();
        topHudRt.SetParent(safeRoot.transform, false);
        topHudRt.anchorMin = new Vector2(0f, 1f);
        topHudRt.anchorMax = new Vector2(1f, 1f);
        topHudRt.pivot = new Vector2(0.5f, 1f);
        topHudRt.anchoredPosition = new Vector2(0f, -30f);
        topHudRt.sizeDelta = new Vector2(0f, 140f);

        GameObject coinsPanel = UIBuildUtils.CreateImage("CoinsPanel", topHud.transform, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(coinsPanel, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(60f, 0f), new Vector2(320f, 100f));

        GameObject coinsIcon = UIBuildUtils.CreateImage("CoinsIcon", coinsPanel.transform, UIBuildUtils.ACCENT, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(coinsIcon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(50f, 0f), new Vector2(70f, 70f));

        GameObject coinsText = UIBuildUtils.CreateText("CoinsText", coinsPanel.transform, "0", 52, UIBuildUtils.WHITE, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(coinsText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(110f, 0f), new Vector2(-20f, 0f));

        GameObject gemsPanel = UIBuildUtils.CreateImage("GemsPanel", topHud.transform, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(gemsPanel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(320f, 100f));

        GameObject gemsIcon = UIBuildUtils.CreateImage("GemsIcon", gemsPanel.transform, new Color(0.4f, 0.85f, 0.9f, 1f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemsIcon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(50f, 0f), new Vector2(70f, 70f));

        GameObject gemsText = UIBuildUtils.CreateText("GemsText", gemsPanel.transform, "0", 52, UIBuildUtils.WHITE, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(gemsText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(110f, 0f), new Vector2(-20f, 0f));

        GameObject pauseBtn = UIBuildUtils.CreateIconButton("PauseButton", topHud.transform, new Color(0f, 0f, 0f, 0.5f), "II", 56);
        UIBuildUtils.SetSize(pauseBtn, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-60f, 0f), new Vector2(120f, 120f));

        GameObject blocksRow = new GameObject("BlocksRow");
        RectTransform blocksRt = blocksRow.AddComponent<RectTransform>();
        blocksRt.SetParent(safeRoot.transform, false);
        blocksRt.anchorMin = new Vector2(0.5f, 1f);
        blocksRt.anchorMax = new Vector2(0.5f, 1f);
        blocksRt.pivot = new Vector2(0.5f, 1f);
        blocksRt.anchoredPosition = new Vector2(0f, -260f);
        blocksRt.sizeDelta = new Vector2(960f, 320f);

        GameObject blocksHint = UIBuildUtils.CreateText("HintText", blocksRow.transform, "BLOCKS ROW\n(iteration 3)", 40, new Color(1f, 1f, 1f, 0.25f));
        UIBuildUtils.SetRect(blocksHint, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject pickaxeGrid = new GameObject("PickaxeGrid");
        RectTransform pgRt = pickaxeGrid.AddComponent<RectTransform>();
        pgRt.SetParent(safeRoot.transform, false);
        pgRt.anchorMin = new Vector2(0.5f, 0f);
        pgRt.anchorMax = new Vector2(0.5f, 0f);
        pgRt.pivot = new Vector2(0.5f, 0f);
        pgRt.anchoredPosition = new Vector2(0f, 300f);
        pgRt.sizeDelta = new Vector2(960f, 760f);

        int cols = 5;
        int rows = 4;
        float slotSize = 170f;
        float spacing = 12f;
        float gridWidth = cols * slotSize + (cols - 1) * spacing;
        float gridHeight = rows * slotSize + (rows - 1) * spacing;
        float startX = -gridWidth * 0.5f + slotSize * 0.5f;
        float startY = gridHeight * 0.5f - slotSize * 0.5f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject slot = UIBuildUtils.CreateImage("Slot_" + r + "_" + c, pickaxeGrid.transform, UIBuildUtils.SLOT_BG, UIBuildUtils.GetUISprite());
                float x = startX + c * (slotSize + spacing);
                float y = startY - r * (slotSize + spacing);
                UIBuildUtils.SetSize(slot, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(x, y), new Vector2(slotSize, slotSize));

                GameObject border = UIBuildUtils.CreateImage("Border", slot.transform, UIBuildUtils.SLOT_BORDER, UIBuildUtils.GetUISprite());
                UIBuildUtils.SetRect(border, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
                border.GetComponent<Image>().raycastTarget = false;
            }
        }

        GameObject shopBtn = UIBuildUtils.CreateButton("ShopButton", safeRoot.transform, "SHOP", new Color(0.5f, 0.5f, 0.55f, 1f), UIBuildUtils.WHITE, 60);
        UIBuildUtils.SetSize(shopBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 130f), new Vector2(500f, 140f));
        shopBtn.GetComponent<Button>().interactable = false;

        GameHUD hud = canvasGo.AddComponent<GameHUD>();
        SerializedObject hudSo = new SerializedObject(hud);
        hudSo.FindProperty("coinsText").objectReferenceValue = coinsText.GetComponent<TextMeshProUGUI>();
        hudSo.FindProperty("gemsText").objectReferenceValue = gemsText.GetComponent<TextMeshProUGUI>();
        hudSo.FindProperty("pauseButton").objectReferenceValue = pauseBtn.GetComponent<Button>();
        hudSo.FindProperty("coinsIcon").objectReferenceValue = coinsIcon.GetComponent<RectTransform>();
        hudSo.FindProperty("gemsIcon").objectReferenceValue = gemsIcon.GetComponent<RectTransform>();
        hudSo.ApplyModifiedProperties();
    }
}
#endif

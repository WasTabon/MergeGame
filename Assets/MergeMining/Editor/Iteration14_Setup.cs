#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration14_Setup
{
    private const string SCENES_DIR = "Assets/MergeMining/Scenes";
    private const string MENU_SCENE_PATH = "Assets/MergeMining/Scenes/MainMenu.unity";
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string LEVELSELECT_SCENE_PATH = "Assets/MergeMining/Scenes/LevelSelect.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 14) Update ALL")]
    public static void UpdateAll()
    {
        CreateLevelSelectScene();
        CleanupGameScene();
        UpdateVictoryPopupStars();
        EditorSceneManager.OpenScene(MENU_SCENE_PATH);
        Debug.Log("Iteration 14: ALL done.");
    }

    [MenuItem("Tools/Merge Mining/(Iteration 14) Create LevelSelect Scene")]
    public static void CreateLevelSelectScene()
    {
        if (!Directory.Exists(SCENES_DIR)) Directory.CreateDirectory(SCENES_DIR);

        UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        BuildLevelSelectScene();
        EditorSceneManager.SaveScene(scene, LEVELSELECT_SCENE_PATH);
        AddSceneToBuildSettings(LEVELSELECT_SCENE_PATH);
        Debug.Log("LevelSelect scene created at " + LEVELSELECT_SCENE_PATH);
    }

    [MenuItem("Tools/Merge Mining/(Iteration 14) Cleanup Game Scene")]
    public static void CleanupGameScene()
    {
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        DestroyByName("ChestOpeningPopupCanvas");
        DestroyByName("ChestHud");
        DestroyByName("BoosterEdgeGlow");
        DestroyByName("ZoneHud");
        DestroyByName("ZoneCompletePopupCanvas");
        DestroyByName("OfflineRewardsPopupCanvas");
        DestroyByName("TutorialOverlayCanvas");
        DestroyByName("AchievementToastCanvas");
        DestroyByName("DailyRewardPopupCanvas");
        DestroyByName("BoosterHud");

        DestroyComponentByType<ChestManager>();
        DestroyComponentByType<BoosterManager>();
        DestroyComponentByType<ZoneManager>();
        DestroyComponentByType<TutorialManager>();
        DestroyComponentByType<OfflineRewardsManager>();
        DestroyComponentByType<AchievementManager>();
        DestroyComponentByType<DailyRewardManager>();

        UpdateVictoryPopupStars();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Game scene cleaned up.");
    }

    [MenuItem("Tools/Merge Mining/(Iteration 14) Update Victory Popup Stars")]
    public static void UpdateVictoryPopupStars()
    {
        if (EditorSceneManager.GetActiveScene().path != GAME_SCENE_PATH)
        {
            EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        }

        LevelVictoryPopup popup = Object.FindObjectOfType<LevelVictoryPopup>(true);
        if (popup == null)
        {
            Debug.LogWarning("LevelVictoryPopup not found! Run Iter13 first.");
            return;
        }

        Transform contentT = popup.transform.Find("Content");
        if (contentT == null) return;

        Transform oldRow = contentT.Find("StarsRow");
        if (oldRow != null) Object.DestroyImmediate(oldRow.gameObject);

        GameObject starsRow = new GameObject("StarsRow");
        RectTransform srt = starsRow.AddComponent<RectTransform>();
        srt.SetParent(contentT, false);
        srt.anchorMin = new Vector2(0.5f, 0.5f);
        srt.anchorMax = new Vector2(0.5f, 0.5f);
        srt.pivot = new Vector2(0.5f, 0.5f);
        srt.anchoredPosition = new Vector2(0f, 200f);
        srt.sizeDelta = new Vector2(700f, 220f);

        Image star1 = CreateStarIcon(starsRow.transform, "Star1", new Vector2(-220f, 0f), 180f);
        Image star2 = CreateStarIcon(starsRow.transform, "Star2", new Vector2(0f, 20f), 220f);
        Image star3 = CreateStarIcon(starsRow.transform, "Star3", new Vector2(220f, 0f), 180f);

        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("star1Icon").objectReferenceValue = star1;
        so.FindProperty("star2Icon").objectReferenceValue = star2;
        so.FindProperty("star3Icon").objectReferenceValue = star3;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Victory popup stars updated.");
    }

    private static Image CreateStarIcon(Transform parent, string name, Vector2 position, float size)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(size, size);

        Image img = go.AddComponent<Image>();
        img.sprite = UIBuildUtils.GetKnobSprite();
        img.color = new Color(0.25f, 0.25f, 0.3f);
        img.preserveAspect = true;
        img.raycastTarget = false;

        GameObject star = new GameObject("StarSymbol");
        RectTransform srt = star.AddComponent<RectTransform>();
        srt.SetParent(go.transform, false);
        srt.anchorMin = Vector2.zero;
        srt.anchorMax = Vector2.one;
        srt.offsetMin = Vector2.zero;
        srt.offsetMax = Vector2.zero;
        TextMeshProUGUI tmp = star.AddComponent<TextMeshProUGUI>();
        tmp.text = "★";
        tmp.fontSize = Mathf.RoundToInt(size * 0.85f);
        tmp.color = new Color(1f, 1f, 1f, 0.9f);
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;

        return img;
    }

    private static void DestroyByName(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go != null) Object.DestroyImmediate(go);
    }

    private static void DestroyComponentByType<T>() where T : Component
    {
        T comp = Object.FindObjectOfType<T>(true);
        if (comp != null) Object.DestroyImmediate(comp);
    }

    private static void AddSceneToBuildSettings(string path)
    {
        var existing = EditorBuildSettings.scenes;
        foreach (var s in existing) if (s.path == path) return;
        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(existing);
        list.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = list.ToArray();
    }

    private static void BuildLevelSelectScene()
    {
        GameObject managers = new GameObject("Managers");
        managers.AddComponent<GameInitializer>();
        managers.AddComponent<SoundManager>();
        managers.AddComponent<HapticManager>();
        managers.AddComponent<TransitionManager>();
        managers.AddComponent<CurrencyManager>();
        managers.AddComponent<PopupManager>();
        managers.AddComponent<SfxLibrary>();

        Camera cam = new GameObject("Main Camera").AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = UIBuildUtils.BG_DARK;
        cam.orthographic = true;
        cam.transform.position = new Vector3(0f, 0f, -10f);

        UIBuildUtils.CreateEventSystemIfNeeded();

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

        GameObject topPanel = UIBuildUtils.CreateImage("TopPanel", safeRoot.transform, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(topPanel, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -30f), new Vector2(1000f, 140f));

        GameObject coinsPanel = UIBuildUtils.CreateImage("CoinsPanel", topPanel.transform, new Color(0f, 0f, 0f, 0.3f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(coinsPanel, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(60f, 0f), new Vector2(280f, 90f));

        GameObject coinsIcon = UIBuildUtils.CreateImage("CoinsIcon", coinsPanel.transform, UIBuildUtils.ACCENT, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(coinsIcon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(45f, 0f), new Vector2(60f, 60f));

        GameObject coinsText = UIBuildUtils.CreateText("CoinsText", coinsPanel.transform, "0", 48, UIBuildUtils.WHITE, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(coinsText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(100f, 0f), new Vector2(-20f, 0f));

        GameObject gemsPanel = UIBuildUtils.CreateImage("GemsPanel", topPanel.transform, new Color(0f, 0f, 0f, 0.3f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(gemsPanel, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-60f, 0f), new Vector2(280f, 90f));

        GameObject gemsIcon = UIBuildUtils.CreateImage("GemsIcon", gemsPanel.transform, new Color(0.4f, 0.85f, 0.9f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemsIcon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(45f, 0f), new Vector2(60f, 60f));

        GameObject gemsText = UIBuildUtils.CreateText("GemsText", gemsPanel.transform, "0", 48, UIBuildUtils.WHITE, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(gemsText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(100f, 0f), new Vector2(-20f, 0f));

        GameObject titleText = UIBuildUtils.CreateText("Title", topPanel.transform, "SELECT LEVEL", 56, Color.white);
        UIBuildUtils.SetRect(titleText, new Vector2(0.25f, 0f), new Vector2(0.75f, 1f), Vector2.zero, Vector2.zero);
        titleText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        MenuHUD menuHud = canvasGo.AddComponent<MenuHUD>();
        SerializedObject menuHudSo = new SerializedObject(menuHud);
        menuHudSo.FindProperty("coinsText").objectReferenceValue = coinsText.GetComponent<TextMeshProUGUI>();
        menuHudSo.FindProperty("gemsText").objectReferenceValue = gemsText.GetComponent<TextMeshProUGUI>();
        menuHudSo.ApplyModifiedProperties();

        GameObject backBtn = UIBuildUtils.CreateButton("BackButton", safeRoot.transform, "BACK", new Color(0.4f, 0.4f, 0.45f), Color.white, 44);
        UIBuildUtils.SetSize(backBtn, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(120f, 100f), new Vector2(200f, 130f));

        GameObject scrollArea = new GameObject("ScrollArea");
        RectTransform scrollRt = scrollArea.AddComponent<RectTransform>();
        scrollRt.SetParent(safeRoot.transform, false);
        scrollRt.anchorMin = new Vector2(0f, 0f);
        scrollRt.anchorMax = new Vector2(1f, 1f);
        scrollRt.offsetMin = new Vector2(40f, 250f);
        scrollRt.offsetMax = new Vector2(-40f, -200f);
        Image scrollAreaImg = scrollArea.AddComponent<Image>();
        scrollAreaImg.color = new Color(0f, 0f, 0f, 0.15f);

        ScrollRect scrollRect = scrollArea.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.elasticity = 0.1f;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.inertia = true;

        GameObject viewport = new GameObject("Viewport");
        RectTransform vrt = viewport.AddComponent<RectTransform>();
        vrt.SetParent(scrollArea.transform, false);
        vrt.anchorMin = Vector2.zero;
        vrt.anchorMax = Vector2.one;
        vrt.offsetMin = new Vector2(10f, 10f);
        vrt.offsetMax = new Vector2(-10f, -10f);
        Image viewImg = viewport.AddComponent<Image>();
        viewImg.color = new Color(1f, 1f, 1f, 0.01f);
        viewport.AddComponent<Mask>().showMaskGraphic = false;

        GameObject pathContent = new GameObject("PathContent");
        RectTransform prt = pathContent.AddComponent<RectTransform>();
        prt.SetParent(viewport.transform, false);
        prt.anchorMin = new Vector2(0f, 0f);
        prt.anchorMax = new Vector2(1f, 0f);
        prt.pivot = new Vector2(0.5f, 0f);
        prt.anchoredPosition = Vector2.zero;
        prt.sizeDelta = new Vector2(0f, 3500f);

        scrollRect.viewport = vrt;
        scrollRect.content = prt;

        GameObject template = BuildLevelNodeTemplate(canvasGo.transform);

        LevelSelectController controller = canvasGo.AddComponent<LevelSelectController>();
        SerializedObject cso = new SerializedObject(controller);
        cso.FindProperty("pathContainer").objectReferenceValue = prt;
        cso.FindProperty("nodeTemplate").objectReferenceValue = template.GetComponent<LevelSelectNodeView>();
        cso.FindProperty("backButton").objectReferenceValue = backBtn.GetComponent<Button>();
        cso.FindProperty("scrollRect").objectReferenceValue = scrollRect;
        cso.FindProperty("nodeSpacing").floatValue = 220f;
        cso.FindProperty("horizontalAmplitude").floatValue = 180f;
        cso.ApplyModifiedProperties();
    }

    private static GameObject BuildLevelNodeTemplate(Transform parent)
    {
        GameObject root = new GameObject("LevelNodeTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.sizeDelta = new Vector2(200f, 200f);

        GameObject circle = UIBuildUtils.CreateImage("Circle", root.transform, new Color(0.55f, 0.565f, 0.886f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(circle, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        Button btn = circle.AddComponent<Button>();
        ColorBlock cb = btn.colors; cb.disabledColor = new Color(1f, 1f, 1f, 0.5f); btn.colors = cb;
        circle.AddComponent<ButtonAnimator>();

        GameObject number = UIBuildUtils.CreateText("Number", circle.transform, "1", 72, Color.white);
        UIBuildUtils.SetRect(number, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        number.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        number.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        GameObject lockGo = UIBuildUtils.CreateImage("LockIcon", circle.transform, Color.white, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(lockGo, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 80f));
        lockGo.GetComponent<Image>().raycastTarget = false;
        GameObject lockSymbol = UIBuildUtils.CreateText("LockSymbol", lockGo.transform, "🔒", 56, Color.white);
        UIBuildUtils.SetRect(lockSymbol, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        lockSymbol.GetComponent<TextMeshProUGUI>().raycastTarget = false;
        lockGo.SetActive(false);

        GameObject starsBar = new GameObject("StarsBar");
        RectTransform sbrt = starsBar.AddComponent<RectTransform>();
        sbrt.SetParent(root.transform, false);
        sbrt.anchorMin = new Vector2(0.5f, 0f);
        sbrt.anchorMax = new Vector2(0.5f, 0f);
        sbrt.pivot = new Vector2(0.5f, 1f);
        sbrt.anchoredPosition = new Vector2(0f, -10f);
        sbrt.sizeDelta = new Vector2(180f, 50f);

        Image star1 = BuildSmallStarIcon(starsBar.transform, "Star1", new Vector2(-55f, 0f));
        Image star2 = BuildSmallStarIcon(starsBar.transform, "Star2", new Vector2(0f, 0f));
        Image star3 = BuildSmallStarIcon(starsBar.transform, "Star3", new Vector2(55f, 0f));

        GameObject marker = UIBuildUtils.CreateImage("CurrentMarker", root.transform, new Color(0.95f, 0.78f, 0.25f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(marker, new Vector2(0.5f, 1f), new Vector2(0.5f, 0f), new Vector2(0f, 35f), new Vector2(50f, 50f));
        marker.GetComponent<Image>().raycastTarget = false;
        marker.SetActive(false);

        LevelSelectNodeView view = root.AddComponent<LevelSelectNodeView>();
        view.button = btn;
        view.circleBg = circle.GetComponent<Image>();
        view.lockIcon = lockGo.GetComponent<Image>();
        view.numberText = number.GetComponent<TextMeshProUGUI>();
        view.star1Icon = star1;
        view.star2Icon = star2;
        view.star3Icon = star3;
        view.currentMarker = marker.GetComponent<Image>();

        root.SetActive(false);
        return root;
    }

    private static Image BuildSmallStarIcon(Transform parent, string name, Vector2 pos)
    {
        GameObject go = new GameObject(name);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(46f, 46f);

        Image img = go.AddComponent<Image>();
        img.sprite = UIBuildUtils.GetKnobSprite();
        img.color = new Color(0f, 0f, 0f, 0.5f);
        img.raycastTarget = false;

        GameObject sym = UIBuildUtils.CreateText("StarSymbol", go.transform, "★", 36, new Color(1f, 1f, 1f, 0.9f));
        UIBuildUtils.SetRect(sym, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        sym.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        sym.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        return img;
    }
}
#endif

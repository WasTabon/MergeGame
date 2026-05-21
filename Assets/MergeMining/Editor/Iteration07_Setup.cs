#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration07_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string CONFIG_DIR = "Assets/MergeMining/Resources";
    private const string BOOSTER_CONFIG_PATH = "Assets/MergeMining/Resources/BoosterConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 7) Update Game Scene")]
    public static void UpdateGameScene()
    {
        CreateBoosterConfig();

        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        AttachBoosterManager();
        GameObject boosterHud = BuildBoosterHud();
        WireBoosterHud(boosterHud);
        AddBoosterEdgeGlow();

        ShiftPickaxeGridUp();
        ShiftShopButtonUp();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 7: Game scene updated.");
    }

    private static void ShiftPickaxeGridUp()
    {
        GameObject grid = GameObject.Find("PickaxeGrid");
        if (grid == null) return;
        RectTransform rt = grid.transform as RectTransform;
        Vector2 pos = rt.anchoredPosition;
        pos.y = 420f;
        rt.anchoredPosition = pos;
    }

    private static void ShiftShopButtonUp()
    {
        GameObject shop = GameObject.Find("ShopButton");
        if (shop == null) return;
        RectTransform rt = shop.transform as RectTransform;
        Vector2 pos = rt.anchoredPosition;
        pos.y = 250f;
        rt.anchoredPosition = pos;
    }

    private static void AttachBoosterManager()
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        BoosterManager mgr = Object.FindObjectOfType<BoosterManager>();
        if (mgr == null) canvasGo.AddComponent<BoosterManager>();
    }

    private static GameObject BuildBoosterHud()
    {
        GameObject existing = GameObject.Find("BoosterHud");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject canvasGo = GameObject.Find("Canvas");
        Transform safeAreaT = canvasGo.transform.Find("SafeArea");

        GameObject root = new GameObject("BoosterHud");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(safeAreaT != null ? safeAreaT : canvasGo.transform, false);
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 80f);
        rt.sizeDelta = new Vector2(-40f, 160f);

        HorizontalLayoutGroup layout = root.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.childControlWidth = true;
        layout.childControlHeight = true;

        var btns = new List<BoosterButtonView>();
        btns.Add(BuildBoosterButton(root.transform, "SpeedBoosterButton", BoosterType.SpeedX2, "x2 SPEED", new Color(0.4f, 0.85f, 0.95f)));
        btns.Add(BuildBoosterButton(root.transform, "RewardsBoosterButton", BoosterType.RewardsX2, "x2 GOLD", new Color(1f, 0.85f, 0.4f)));
        btns.Add(BuildBoosterButton(root.transform, "AutoMergeBoosterButton", BoosterType.AutoMerge, "AUTO MERGE", new Color(0.85f, 0.55f, 0.95f)));
        btns.Add(BuildBoosterButton(root.transform, "InstantDestroyBoosterButton", BoosterType.InstantDestroy, "INSTANT", new Color(0.95f, 0.4f, 0.4f)));

        BoosterHud hud = root.AddComponent<BoosterHud>();
        SerializedObject so = new SerializedObject(hud);
        SerializedProperty btnsProp = so.FindProperty("buttons");
        btnsProp.arraySize = btns.Count;
        for (int i = 0; i < btns.Count; i++)
        {
            btnsProp.GetArrayElementAtIndex(i).objectReferenceValue = btns[i];
        }
        so.ApplyModifiedProperties();

        return root;
    }

    private static BoosterButtonView BuildBoosterButton(Transform parent, string name, BoosterType type, string label, Color color)
    {
        GameObject root = UIBuildUtils.CreateImage(name, parent, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        Button btn = root.AddComponent<Button>();
        ColorBlock cb = btn.colors; cb.normalColor = Color.white; cb.disabledColor = new Color(1f, 1f, 1f, 0.45f); btn.colors = cb;
        root.AddComponent<ButtonAnimator>();

        GameObject iconBg = UIBuildUtils.CreateImage("IconBg", root.transform, color, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(iconBg, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(70f, 70f));
        iconBg.GetComponent<Image>().raycastTarget = false;

        GameObject nameText = UIBuildUtils.CreateText("Name", root.transform, label, 22, Color.white);
        UIBuildUtils.SetSize(nameText, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -25f), new Vector2(180f, 30f));
        nameText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject costRow = new GameObject("CostRow");
        RectTransform crt = costRow.AddComponent<RectTransform>();
        crt.SetParent(root.transform, false);
        crt.anchorMin = new Vector2(0.5f, 0f);
        crt.anchorMax = new Vector2(0.5f, 0f);
        crt.pivot = new Vector2(0.5f, 0f);
        crt.anchoredPosition = new Vector2(0f, 12f);
        crt.sizeDelta = new Vector2(120f, 32f);

        GameObject gemIcon = UIBuildUtils.CreateImage("GemIcon", costRow.transform, UIColors.GEM, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemIcon, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(20f, 0f), new Vector2(24f, 24f));
        gemIcon.GetComponent<Image>().raycastTarget = false;

        GameObject costText = UIBuildUtils.CreateText("Cost", costRow.transform, "10", 24, Color.white, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(costText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(40f, 0f), new Vector2(-10f, 0f));
        costText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject cooldown = UIBuildUtils.CreateImage("CooldownOverlay", root.transform, new Color(0.4f, 0.85f, 0.95f, 0.35f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(cooldown, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image cdImg = cooldown.GetComponent<Image>();
        cdImg.type = Image.Type.Filled;
        cdImg.fillMethod = Image.FillMethod.Vertical;
        cdImg.fillOrigin = (int)Image.OriginVertical.Bottom;
        cdImg.fillAmount = 1f;
        cdImg.raycastTarget = false;
        cooldown.SetActive(false);

        GameObject remaining = UIBuildUtils.CreateText("Remaining", root.transform, "30s", 28, Color.white);
        UIBuildUtils.SetSize(remaining, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 14f), new Vector2(120f, 32f));
        remaining.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        remaining.SetActive(false);

        BoosterButtonView view = root.AddComponent<BoosterButtonView>();
        view.type = type;
        view.button = btn;
        view.iconImage = iconBg.GetComponent<Image>();
        view.nameText = nameText.GetComponent<TextMeshProUGUI>();
        view.costText = costText.GetComponent<TextMeshProUGUI>();
        view.costRow = crt;
        view.cooldownOverlay = cdImg;
        view.remainingText = remaining.GetComponent<TextMeshProUGUI>();

        return view;
    }

    private static void WireBoosterHud(GameObject boosterHud)
    {
    }

    private static void AddBoosterEdgeGlow()
    {
        GameObject existing = GameObject.Find("BoosterEdgeGlow");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject canvasGo = GameObject.Find("Canvas");

        GameObject root = UIBuildUtils.CreateImage("BoosterEdgeGlow", canvasGo.transform, new Color(0.4f, 0.85f, 0.95f, 0f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(root, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image img = root.GetComponent<Image>();
        img.raycastTarget = false;
        img.type = Image.Type.Sliced;

        root.transform.SetAsLastSibling();

        BoosterVisualFx fx = root.AddComponent<BoosterVisualFx>();
        SerializedObject so = new SerializedObject(fx);
        so.FindProperty("edgeGlowImage").objectReferenceValue = img;
        so.ApplyModifiedProperties();
    }

    private static void CreateBoosterConfig()
    {
        if (!Directory.Exists(CONFIG_DIR)) Directory.CreateDirectory(CONFIG_DIR);
        BoosterData existing = AssetDatabase.LoadAssetAtPath<BoosterData>(BOOSTER_CONFIG_PATH);
        if (existing != null) return;

        BoosterData data = ScriptableObject.CreateInstance<BoosterData>();
        data.boosters = new List<BoosterInfo>
        {
            new BoosterInfo { type = BoosterType.SpeedX2, displayName = "Speed x2", color = new Color(0.4f, 0.85f, 0.95f), duration = 30f, gemsCost = 10 },
            new BoosterInfo { type = BoosterType.RewardsX2, displayName = "Gold x2", color = new Color(1f, 0.85f, 0.4f), duration = 30f, gemsCost = 10 },
            new BoosterInfo { type = BoosterType.AutoMerge, displayName = "Auto Merge", color = new Color(0.85f, 0.55f, 0.95f), duration = 30f, gemsCost = 15 },
            new BoosterInfo { type = BoosterType.InstantDestroy, displayName = "Instant Break", color = new Color(0.95f, 0.4f, 0.4f), duration = 0f, gemsCost = 5 },
        };

        AssetDatabase.CreateAsset(data, BOOSTER_CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("BoosterConfig.asset created");
    }
}
#endif

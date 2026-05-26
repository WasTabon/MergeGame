#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration16_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string LEVEL_CONFIG_PATH = "Assets/MergeMining/Resources/LevelConfig.asset";
    private const string PICKAXE_CONFIG_PATH = "Assets/MergeMining/Resources/PickaxeConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 16) Update ALL")]
    public static void UpdateAll()
    {
        UpdatePickaxeConfigDurability();
        UpdateLevelConfigWithTimers();
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        BuildDefeatPopup();
        AddTimerToLevelHud();
        AddDurabilityBarToPickaxePrefab();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 16 ALL done.");
    }

    private static void UpdatePickaxeConfigDurability()
    {
        PickaxeData data = AssetDatabase.LoadAssetAtPath<PickaxeData>(PICKAXE_CONFIG_PATH);
        if (data == null) { Debug.LogError("PickaxeConfig.asset not found"); return; }

        for (int i = 0; i < data.levels.Count; i++)
        {
            int baseD;
            if (i == 0) baseD = 10;
            else if (i == 1) baseD = 15;
            else if (i == 2) baseD = 25;
            else baseD = Mathf.RoundToInt(25 * Mathf.Pow(1.5f, i - 2));
            data.levels[i].durability = baseD;
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("PickaxeConfig durability updated.");
    }

    private static void UpdateLevelConfigWithTimers()
    {
        LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(LEVEL_CONFIG_PATH);
        if (data == null) { Debug.LogError("LevelConfig.asset not found"); return; }

        float[] timers = { 60, 65, 70, 75, 85, 95, 105, 115, 125, 135, 145, 155, 165, 175, 180 };
        float[] descend = { 0f, 0f, 8f, 10f, 14f, 18f, 22f, 25f, 28f, 30f, 32f, 33f, 34f, 35f, 35f };

        for (int i = 0; i < data.levels.Count && i < 15; i++)
        {
            data.levels[i].timeLimitSeconds = timers[i];
            data.levels[i].blockDescendSpeed = descend[i];
            data.levels[i].blockRegenDelay = 1.5f;
            data.levels[i].blockRegenPerSec = 0.05f;
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("LevelConfig updated with timers & descend speed.");
    }

    private static void BuildDefeatPopup()
    {
        GameObject old = GameObject.Find("LevelDefeatPopup");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("LevelDefeatPopupCanvas", 228);

        GameObject popupGo = new GameObject("LevelDefeatPopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.18f, 0.13f, 0.13f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 900f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "DEFEAT", 110, new Color(0.95f, 0.3f, 0.3f));
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(800f, 130f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject reasonText = UIBuildUtils.CreateText("ReasonText", content.transform, "RAN OUT OF TIME!", 48, Color.white);
        UIBuildUtils.SetSize(reasonText, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 100f), new Vector2(800f, 100f));

        GameObject retryBtn = UIBuildUtils.CreateButton("RetryButton", content.transform, "RETRY", UIColors.GREEN, Color.white, 56);
        UIBuildUtils.SetSize(retryBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 260f), new Vector2(700f, 160f));

        GameObject menuBtn = UIBuildUtils.CreateButton("MenuButton", content.transform, "MENU", new Color(0.4f, 0.4f, 0.45f), Color.white, 50);
        UIBuildUtils.SetSize(menuBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 110f), new Vector2(700f, 140f));

        LevelDefeatPopup popup = popupGo.AddComponent<LevelDefeatPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("reasonText").objectReferenceValue = reasonText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("retryButton").objectReferenceValue = retryBtn.GetComponent<Button>();
        so.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        LevelManager mgr = Object.FindObjectOfType<LevelManager>();
        if (mgr != null)
        {
            SerializedObject mso = new SerializedObject(mgr);
            mso.FindProperty("defeatPopup").objectReferenceValue = popup;
            mso.ApplyModifiedProperties();
        }
    }

    private static void AddTimerToLevelHud()
    {
        LevelHud hud = Object.FindObjectOfType<LevelHud>(true);
        if (hud == null) { Debug.LogWarning("LevelHud not found"); return; }

        Transform existing = hud.transform.Find("TimerPanel");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        GameObject timerPanel = UIBuildUtils.CreateImage("TimerPanel", hud.transform, new Color(0f, 0f, 0f, 0.45f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(timerPanel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(100f, 60f), new Vector2(200f, 90f));
        timerPanel.GetComponent<Image>().raycastTarget = false;

        GameObject timerFill = UIBuildUtils.CreateImage("TimerFill", timerPanel.transform, new Color(0.4f, 0.78f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(timerFill, Vector2.zero, Vector2.one, new Vector2(4f, 4f), new Vector2(-4f, -4f));
        Image fillImg = timerFill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg.fillAmount = 1f;
        fillImg.raycastTarget = false;

        GameObject timerText = UIBuildUtils.CreateText("TimerText", timerPanel.transform, "01:00", 44, Color.white);
        UIBuildUtils.SetRect(timerText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        timerText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        timerText.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        SerializedObject so = new SerializedObject(hud);
        so.FindProperty("timerText").objectReferenceValue = timerText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("timerFill").objectReferenceValue = fillImg;
        so.ApplyModifiedProperties();
        Debug.Log("Timer added to LevelHud.");
    }

    private static void AddDurabilityBarToPickaxePrefab()
    {
        PickaxeGridManager grid = Object.FindObjectOfType<PickaxeGridManager>();
        if (grid == null) { Debug.LogWarning("PickaxeGridManager not found"); return; }

        SerializedObject so = new SerializedObject(grid);
        SerializedProperty prefabProp = so.FindProperty("pickaxePrefabRoot");
        GameObject prefab = prefabProp.objectReferenceValue as GameObject;
        if (prefab == null) { Debug.LogWarning("pickaxePrefabRoot is null"); return; }

        Transform existing = prefab.transform.Find("DurabilityBar");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        GameObject barBg = UIBuildUtils.CreateImage("DurabilityBar", prefab.transform, new Color(0f, 0f, 0f, 0.5f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(barBg, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 15f), new Vector2(120f, 18f));
        barBg.GetComponent<Image>().raycastTarget = false;

        GameObject fill = UIBuildUtils.CreateImage("DurabilityFill", barBg.transform, new Color(0.4f, 0.78f, 0.4f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(fill, Vector2.zero, Vector2.one, new Vector2(2f, 2f), new Vector2(-2f, -2f));
        Image fillImg = fill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg.fillAmount = 1f;
        fillImg.raycastTarget = false;

        Pickaxe pickaxeComp = prefab.GetComponent<Pickaxe>();
        if (pickaxeComp != null)
        {
            SerializedObject pso = new SerializedObject(pickaxeComp);
            pso.FindProperty("durabilityFill").objectReferenceValue = fillImg;
            pso.ApplyModifiedProperties();
        }
        Debug.Log("Durability bar added to pickaxe prefab.");
    }
}
#endif

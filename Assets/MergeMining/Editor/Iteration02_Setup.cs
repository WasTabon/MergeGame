#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration02_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string CONFIG_DIR = "Assets/MergeMining/Resources";
    private const string CONFIG_PATH = "Assets/MergeMining/Resources/PickaxeConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 2) Update Game Scene")]
    public static void UpdateGameScene()
    {
        CreatePickaxeConfigIfMissing();

        if (EditorSceneManager.GetActiveScene().path != GAME_SCENE_PATH)
        {
            EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        }

        GameObject canvasGo = GameObject.Find("Canvas");
        Debug.Assert(canvasGo != null, "Canvas not found! Run Iteration 1 setup first.");

        GameObject pickaxeGridGo = FindByName(canvasGo.transform, "PickaxeGrid");
        Debug.Assert(pickaxeGridGo != null, "PickaxeGrid not found!");

        AttachSlotComponents(pickaxeGridGo);

        GameObject dragLayer = EnsureDragLayer(canvasGo.transform);
        GameObject prefabRoot = EnsurePickaxePrefabRoot();
        GameObject mergeFxRoot = EnsureMergeEffectPrefabRoot();

        PickaxeGridManager gridManager = pickaxeGridGo.GetComponent<PickaxeGridManager>();
        if (gridManager == null) gridManager = pickaxeGridGo.AddComponent<PickaxeGridManager>();

        List<PickaxeSlot> slotList = new List<PickaxeSlot>();
        foreach (Transform child in pickaxeGridGo.transform)
        {
            PickaxeSlot s = child.GetComponent<PickaxeSlot>();
            if (s != null) slotList.Add(s);
        }

        MergeEffect mergeFx = mergeFxRoot.GetComponent<MergeEffect>();

        SerializedObject gso = new SerializedObject(gridManager);
        SerializedProperty slotsProp = gso.FindProperty("slots");
        slotsProp.arraySize = slotList.Count;
        for (int i = 0; i < slotList.Count; i++)
        {
            slotsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotList[i];
        }
        gso.FindProperty("dragLayer").objectReferenceValue = dragLayer.GetComponent<RectTransform>();
        gso.FindProperty("pickaxeContainer").objectReferenceValue = pickaxeGridGo.GetComponent<RectTransform>();
        gso.FindProperty("pickaxePrefabRoot").objectReferenceValue = prefabRoot;
        gso.FindProperty("mergeEffectPrefab").objectReferenceValue = mergeFx;
        gso.FindProperty("slotSize").floatValue = 170f;
        gso.ApplyModifiedProperties();

        AttachShopController(canvasGo);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        Debug.Log("Iteration 2 setup complete. Game scene updated.");
    }

    private static GameObject FindByName(Transform parent, string name)
    {
        if (parent.name == name) return parent.gameObject;
        foreach (Transform t in parent)
        {
            GameObject result = FindByName(t, name);
            if (result != null) return result;
        }
        return null;
    }

    private static void AttachSlotComponents(GameObject grid)
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                string slotName = "Slot_" + r + "_" + c;
                Transform t = grid.transform.Find(slotName);
                if (t == null)
                {
                    Debug.LogWarning("Slot not found: " + slotName);
                    continue;
                }
                PickaxeSlot slot = t.GetComponent<PickaxeSlot>();
                if (slot == null) slot = t.gameObject.AddComponent<PickaxeSlot>();
                slot.Init(r, c);
            }
        }
    }

    private static GameObject EnsureDragLayer(Transform canvasParent)
    {
        GameObject existing = GameObject.Find("DragLayer");
        if (existing != null) return existing;

        GameObject canvasGo = new GameObject("DragLayer");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();
        return canvasGo;
    }

    private static GameObject EnsurePickaxePrefabRoot()
    {
        GameObject existing = GameObject.Find("PickaxePrefabTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("PickaxePrefabTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(150f, 150f);
        root.AddComponent<CanvasGroup>();

        GameObject glow = UIBuildUtils.CreateImage("Glow", root.transform, new Color(1f, 1f, 1f, 0.3f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(glow, Vector2.zero, Vector2.one, new Vector2(-30f, -30f), new Vector2(30f, 30f));
        glow.GetComponent<Image>().raycastTarget = false;

        GameObject body = UIBuildUtils.CreateImage("Body", root.transform, Color.white, UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(body, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        GameObject badge = UIBuildUtils.CreateImage("LevelBadge", root.transform, new Color(0f, 0f, 0f, 0.55f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(badge, new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(-15f, -15f), new Vector2(60f, 60f));
        badge.GetComponent<Image>().raycastTarget = false;

        GameObject levelText = UIBuildUtils.CreateText("LevelText", badge.transform, "1", 40, Color.white);
        UIBuildUtils.SetRect(levelText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        TextMeshProUGUI tmp = levelText.GetComponent<TextMeshProUGUI>();
        tmp.fontStyle = FontStyles.Bold;

        Pickaxe p = root.AddComponent<Pickaxe>();
        SerializedObject so = new SerializedObject(p);
        so.FindProperty("bodyImage").objectReferenceValue = body.GetComponent<Image>();
        so.FindProperty("levelBadge").objectReferenceValue = badge.GetComponent<Image>();
        so.FindProperty("levelText").objectReferenceValue = tmp;
        so.FindProperty("glowImage").objectReferenceValue = glow.GetComponent<Image>();
        so.ApplyModifiedProperties();

        root.AddComponent<PickaxeDragHandler>();

        root.SetActive(false);
        return root;
    }

    private static GameObject EnsureMergeEffectPrefabRoot()
    {
        GameObject existing = GameObject.Find("MergeEffectTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("MergeEffectTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200f, 200f);

        GameObject flash = UIBuildUtils.CreateImage("Flash", root.transform, new Color(1f, 1f, 1f, 0.9f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(flash, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        flash.GetComponent<Image>().raycastTarget = false;

        List<RectTransform> particleRts = new List<RectTransform>();
        List<Image> particleImgs = new List<Image>();
        int particleCount = 8;
        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = UIBuildUtils.CreateImage("Particle_" + i, root.transform, Color.white, UIBuildUtils.GetKnobSprite());
            UIBuildUtils.SetSize(p, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(30f, 30f));
            p.GetComponent<Image>().raycastTarget = false;
            particleRts.Add(p.GetComponent<RectTransform>());
            particleImgs.Add(p.GetComponent<Image>());
        }

        MergeEffect fx = root.AddComponent<MergeEffect>();
        SerializedObject so = new SerializedObject(fx);
        so.FindProperty("flashImage").objectReferenceValue = flash.GetComponent<Image>();

        SerializedProperty pRtsProp = so.FindProperty("particles");
        pRtsProp.arraySize = particleRts.Count;
        for (int i = 0; i < particleRts.Count; i++)
        {
            pRtsProp.GetArrayElementAtIndex(i).objectReferenceValue = particleRts[i];
        }
        SerializedProperty pImgsProp = so.FindProperty("particleImages");
        pImgsProp.arraySize = particleImgs.Count;
        for (int i = 0; i < particleImgs.Count; i++)
        {
            pImgsProp.GetArrayElementAtIndex(i).objectReferenceValue = particleImgs[i];
        }
        so.ApplyModifiedProperties();

        root.SetActive(false);
        return root;
    }

    private static void AttachShopController(GameObject canvasGo)
    {
        GameObject shopBtnGo = FindByName(canvasGo.transform, "ShopButton");
        Debug.Assert(shopBtnGo != null, "ShopButton not found!");

        Transform labelT = shopBtnGo.transform.Find("Label");
        if (labelT != null)
        {
            TextMeshProUGUI labelTmp = labelT.GetComponent<TextMeshProUGUI>();
            labelTmp.text = "BUY";
            labelTmp.fontSize = 48;
            RectTransform lrt = labelT as RectTransform;
            lrt.anchorMin = new Vector2(0f, 0f);
            lrt.anchorMax = new Vector2(0.45f, 1f);
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
        }

        Image shopImg = shopBtnGo.GetComponent<Image>();
        if (shopImg != null) shopImg.color = UIBuildUtils.ACCENT;

        Transform priceContainerT = shopBtnGo.transform.Find("PriceContainer");
        GameObject priceContainerGo;
        if (priceContainerT == null)
        {
            priceContainerGo = new GameObject("PriceContainer");
            RectTransform pcRt = priceContainerGo.AddComponent<RectTransform>();
            pcRt.SetParent(shopBtnGo.transform, false);
            pcRt.anchorMin = new Vector2(0.5f, 0f);
            pcRt.anchorMax = new Vector2(1f, 1f);
            pcRt.offsetMin = new Vector2(0f, 0f);
            pcRt.offsetMax = new Vector2(-30f, 0f);
        }
        else
        {
            priceContainerGo = priceContainerT.gameObject;
        }

        Transform priceIconT = priceContainerGo.transform.Find("PriceIcon");
        GameObject priceIcon;
        if (priceIconT == null)
        {
            priceIcon = UIBuildUtils.CreateImage("PriceIcon", priceContainerGo.transform, UIBuildUtils.ACCENT, UIBuildUtils.GetKnobSprite());
            UIBuildUtils.SetSize(priceIcon, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20f, 0f), new Vector2(60f, 60f));
            priceIcon.GetComponent<Image>().raycastTarget = false;
            priceIcon.GetComponent<Image>().color = new Color(1f, 0.85f, 0.4f, 1f);
        }
        else priceIcon = priceIconT.gameObject;

        Transform priceTextT = priceContainerGo.transform.Find("PriceText");
        GameObject priceTextGo;
        if (priceTextT == null)
        {
            priceTextGo = UIBuildUtils.CreateText("PriceText", priceContainerGo.transform, "10", 52, Color.white, TextAlignmentOptions.MidlineLeft);
            UIBuildUtils.SetRect(priceTextGo, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(90f, 0f), new Vector2(-10f, 0f));
            priceTextGo.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        }
        else priceTextGo = priceTextT.gameObject;

        Button shopBtn = shopBtnGo.GetComponent<Button>();
        shopBtn.interactable = true;

        ShopController controller = canvasGo.GetComponent<ShopController>();
        if (controller == null) controller = canvasGo.AddComponent<ShopController>();
        SerializedObject so = new SerializedObject(controller);
        so.FindProperty("shopButton").objectReferenceValue = shopBtn;
        so.FindProperty("priceText").objectReferenceValue = priceTextGo.GetComponent<TextMeshProUGUI>();
        so.FindProperty("priceIcon").objectReferenceValue = priceIcon.GetComponent<RectTransform>();
        so.FindProperty("basePrice").intValue = 10;
        so.FindProperty("priceMultiplier").floatValue = 1.15f;
        so.ApplyModifiedProperties();
    }

    private static void CreatePickaxeConfigIfMissing()
    {
        if (!Directory.Exists(CONFIG_DIR))
        {
            Directory.CreateDirectory(CONFIG_DIR);
            AssetDatabase.Refresh();
        }

        PickaxeData existing = AssetDatabase.LoadAssetAtPath<PickaxeData>(CONFIG_PATH);
        if (existing != null) return;

        PickaxeData data = ScriptableObject.CreateInstance<PickaxeData>();
        data.levels = new List<PickaxeLevelData>();

        Color[] palette = new Color[]
        {
            new Color(0.6f, 0.6f, 0.6f),
            new Color(0.45f, 0.45f, 0.5f),
            new Color(0.72f, 0.5f, 0.32f),
            new Color(0.78f, 0.78f, 0.82f),
            new Color(0.95f, 0.78f, 0.25f),
            new Color(0.65f, 0.18f, 0.18f),
            new Color(0.88f, 0.27f, 0.27f),
            new Color(0.95f, 0.46f, 0.18f),
            new Color(1.0f, 0.62f, 0.15f),
            new Color(1.0f, 0.85f, 0.3f),
            new Color(0.85f, 0.3f, 0.85f),
            new Color(0.55f, 0.3f, 0.92f),
            new Color(0.32f, 0.45f, 0.95f),
            new Color(0.25f, 0.85f, 0.85f),
            new Color(0.95f, 0.98f, 1.0f),
        };

        for (int i = 0; i < 15; i++)
        {
            PickaxeLevelData lvl = new PickaxeLevelData();
            lvl.level = i + 1;
            lvl.color = palette[i];
            lvl.damage = Mathf.Pow(1.45f, i);
            lvl.miningSpeed = 1f + i * 0.05f;
            lvl.displayName = "Pickaxe Lv" + (i + 1);
            data.levels.Add(lvl);
        }

        AssetDatabase.CreateAsset(data, CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("PickaxeConfig.asset created at " + CONFIG_PATH);
    }
}
#endif

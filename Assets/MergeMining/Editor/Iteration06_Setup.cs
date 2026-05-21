#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration06_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string CONFIG_DIR = "Assets/MergeMining/Resources";
    private const string CHEST_CONFIG_PATH = "Assets/MergeMining/Resources/ChestConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 6) Update Game Scene")]
    public static void UpdateGameScene()
    {
        CreateChestConfig();

        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        ChestOpeningPopup popup = BuildChestOpeningPopup();
        AttachChestManager(popup);
        GameObject chestHud = BuildChestHud();
        WireChestHud(chestHud);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 6: Game scene updated.");
    }

    private static ChestOpeningPopup BuildChestOpeningPopup()
    {
        GameObject oldRoot = GameObject.Find("ChestOpeningPopup");
        if (oldRoot != null) Object.DestroyImmediate(oldRoot.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("ChestOpeningPopupCanvas", 215);

        GameObject popupGo = new GameObject("ChestOpeningPopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 1400f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "FREE CHEST", 80, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -120f), new Vector2(800f, 100f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject chestVisual = new GameObject("ChestVisual");
        RectTransform cvRt = chestVisual.AddComponent<RectTransform>();
        cvRt.SetParent(content.transform, false);
        cvRt.anchorMin = new Vector2(0.5f, 1f);
        cvRt.anchorMax = new Vector2(0.5f, 1f);
        cvRt.pivot = new Vector2(0.5f, 1f);
        cvRt.anchoredPosition = new Vector2(0f, -260f);
        cvRt.sizeDelta = new Vector2(400f, 360f);

        GameObject chestBody = UIBuildUtils.CreateImage("ChestBody", chestVisual.transform, new Color(0.5f, 0.35f, 0.2f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(chestBody, new Vector2(0f, 0f), new Vector2(1f, 0.65f), Vector2.zero, Vector2.zero);
        chestBody.GetComponent<Image>().raycastTarget = false;

        GameObject chestLock = UIBuildUtils.CreateImage("Lock", chestBody.transform, new Color(1f, 0.85f, 0.4f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(chestLock, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(60f, 60f));
        chestLock.GetComponent<Image>().raycastTarget = false;

        GameObject chestLid = UIBuildUtils.CreateImage("ChestLid", chestVisual.transform, new Color(0.95f, 0.78f, 0.25f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(chestLid, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -10f), new Vector2(380f, 130f));
        chestLid.GetComponent<Image>().raycastTarget = false;

        GameObject flash = UIBuildUtils.CreateImage("Flash", chestVisual.transform, new Color(1f, 1f, 1f, 0f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetRect(flash, new Vector2(-0.5f, -0.5f), new Vector2(1.5f, 1.5f), Vector2.zero, Vector2.zero);
        flash.GetComponent<Image>().raycastTarget = false;

        GameObject rewardsContainer = new GameObject("RewardsContainer");
        RectTransform rcRt = rewardsContainer.AddComponent<RectTransform>();
        rcRt.SetParent(content.transform, false);
        rcRt.anchorMin = new Vector2(0.5f, 0f);
        rcRt.anchorMax = new Vector2(0.5f, 0f);
        rcRt.pivot = new Vector2(0.5f, 0f);
        rcRt.anchoredPosition = new Vector2(0f, 300f);
        rcRt.sizeDelta = new Vector2(800f, 320f);
        GridLayoutGroup grid = rewardsContainer.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(180f, 140f);
        grid.spacing = new Vector2(10f, 10f);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 4;

        GameObject rewardTemplate = BuildRewardItemTemplate(content.transform);

        GameObject continueBtn = UIBuildUtils.CreateButton("ContinueButton", content.transform, "CONTINUE", UIColors.GREEN, Color.white, 60);
        UIBuildUtils.SetSize(continueBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 100f), new Vector2(640f, 160f));

        ChestOpeningPopup popup = popupGo.AddComponent<ChestOpeningPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("chestVisual").objectReferenceValue = cvRt;
        so.FindProperty("chestBodyImage").objectReferenceValue = chestBody.GetComponent<Image>();
        so.FindProperty("chestLidImage").objectReferenceValue = chestLid.GetComponent<Image>();
        so.FindProperty("flashImage").objectReferenceValue = flash.GetComponent<Image>();
        so.FindProperty("rewardsContainer").objectReferenceValue = rcRt;
        so.FindProperty("rewardItemTemplate").objectReferenceValue = rewardTemplate;
        so.FindProperty("continueButton").objectReferenceValue = continueBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static GameObject BuildRewardItemTemplate(Transform parent)
    {
        GameObject existing = GameObject.Find("ChestRewardItemTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("ChestRewardItemTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.sizeDelta = new Vector2(180f, 140f);

        GameObject bg = UIBuildUtils.CreateImage("Bg", root.transform, new Color(0f, 0f, 0f, 0.35f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(bg, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        bg.GetComponent<Image>().raycastTarget = false;

        GameObject icon = UIBuildUtils.CreateImage("Icon", root.transform, Color.white, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(icon, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(80f, 80f));
        icon.GetComponent<Image>().raycastTarget = false;

        GameObject amount = UIBuildUtils.CreateText("Amount", root.transform, "+0", 40, Color.white);
        UIBuildUtils.SetSize(amount, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 25f), new Vector2(170f, 40f));
        amount.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        root.SetActive(false);
        return root;
    }

    private static void AttachChestManager(ChestOpeningPopup popup)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        ChestManager mgr = Object.FindObjectOfType<ChestManager>();
        if (mgr == null) mgr = canvasGo.AddComponent<ChestManager>();

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("chestOpeningPopup").objectReferenceValue = popup;
        so.ApplyModifiedProperties();
    }

    private static GameObject BuildChestHud()
    {
        GameObject existing = GameObject.Find("ChestHud");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject canvasGo = GameObject.Find("Canvas");
        Transform safeAreaT = canvasGo.transform.Find("SafeArea");

        GameObject root = new GameObject("ChestHud");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(safeAreaT != null ? safeAreaT : canvasGo.transform, false);
        rt.anchorMin = new Vector2(0f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 1090f);
        rt.sizeDelta = new Vector2(-40f, 170f);

        HorizontalLayoutGroup layout = root.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.childControlWidth = true;
        layout.childControlHeight = true;

        GameObject freeChest = BuildChestButton(root.transform, "FreeChestButton", new Color(0.4f, 0.7f, 0.4f), new Color(0.6f, 0.95f, 0.6f), "FREE", "00:00");
        GameObject blockChest = BuildChestButton(root.transform, "BlockChestButton", new Color(0.5f, 0.35f, 0.2f), new Color(0.95f, 0.78f, 0.25f), "BLOCK", "0/20");
        GameObject premiumChest = BuildPremiumChestButton(root.transform);

        return root;
    }

    private static GameObject BuildChestButton(Transform parent, string name, Color body, Color lid, string label, string subtext)
    {
        GameObject root = UIBuildUtils.CreateImage(name, parent, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        Button btn = root.AddComponent<Button>();
        btn.transition = Selectable.Transition.ColorTint;
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.disabledColor = new Color(1f, 1f, 1f, 0.5f);
        btn.colors = cb;
        root.AddComponent<ButtonAnimator>();

        GameObject chestBody = UIBuildUtils.CreateImage("ChestBody", root.transform, body, UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(chestBody, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(90f, 70f));
        chestBody.GetComponent<Image>().raycastTarget = false;

        GameObject chestLid = UIBuildUtils.CreateImage("ChestLid", root.transform, lid, UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(chestLid, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -15f), new Vector2(90f, 30f));
        chestLid.GetComponent<Image>().raycastTarget = false;

        GameObject labelText = UIBuildUtils.CreateText("Label", root.transform, label, 24, Color.white);
        UIBuildUtils.SetSize(labelText, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -25f), new Vector2(150f, 30f));
        labelText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject sub = UIBuildUtils.CreateText("Subtext", root.transform, subtext, 22, new Color(1f, 1f, 1f, 0.8f));
        UIBuildUtils.SetSize(sub, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 15f), new Vector2(150f, 30f));

        GameObject badge = UIBuildUtils.CreateImage("ReadyBadge", root.transform, new Color(0.95f, 0.4f, 0.4f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(badge, new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(-15f, -15f), new Vector2(28f, 28f));
        badge.GetComponent<Image>().raycastTarget = false;
        badge.SetActive(false);

        return root;
    }

    private static GameObject BuildPremiumChestButton(Transform parent)
    {
        GameObject root = UIBuildUtils.CreateImage("PremiumChestButton", parent, new Color(0f, 0f, 0f, 0.4f), UIBuildUtils.GetUISprite());
        Button btn = root.AddComponent<Button>();
        btn.transition = Selectable.Transition.ColorTint;
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white; cb.disabledColor = new Color(1f, 1f, 1f, 0.5f);
        btn.colors = cb;
        root.AddComponent<ButtonAnimator>();

        GameObject chestBody = UIBuildUtils.CreateImage("ChestBody", root.transform, new Color(0.4f, 0.2f, 0.55f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(chestBody, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(90f, 70f));
        chestBody.GetComponent<Image>().raycastTarget = false;

        GameObject chestLid = UIBuildUtils.CreateImage("ChestLid", root.transform, new Color(0.85f, 0.55f, 0.95f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(chestLid, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -15f), new Vector2(90f, 30f));
        chestLid.GetComponent<Image>().raycastTarget = false;

        GameObject labelText = UIBuildUtils.CreateText("Label", root.transform, "PREMIUM", 22, Color.white);
        UIBuildUtils.SetSize(labelText, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -25f), new Vector2(150f, 30f));
        labelText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject costRow = new GameObject("CostRow");
        RectTransform crt = costRow.AddComponent<RectTransform>();
        crt.SetParent(root.transform, false);
        crt.anchorMin = new Vector2(0.5f, 0f);
        crt.anchorMax = new Vector2(0.5f, 0f);
        crt.pivot = new Vector2(0.5f, 0f);
        crt.anchoredPosition = new Vector2(0f, 15f);
        crt.sizeDelta = new Vector2(150f, 36f);

        GameObject gemIcon = UIBuildUtils.CreateImage("GemIcon", costRow.transform, UIColors.GEM, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemIcon, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(30f, 0f), new Vector2(28f, 28f));
        gemIcon.GetComponent<Image>().raycastTarget = false;

        GameObject costText = UIBuildUtils.CreateText("Cost", costRow.transform, "50", 26, Color.white, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(costText, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(50f, 0f), new Vector2(-10f, 0f));
        costText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        return root;
    }

    private static void WireChestHud(GameObject chestHud)
    {
        ChestHud hud = chestHud.GetComponent<ChestHud>();
        if (hud == null) hud = chestHud.AddComponent<ChestHud>();

        Transform freeT = chestHud.transform.Find("FreeChestButton");
        Transform blockT = chestHud.transform.Find("BlockChestButton");
        Transform premT = chestHud.transform.Find("PremiumChestButton");

        SerializedObject so = new SerializedObject(hud);
        so.FindProperty("freeChestButton").objectReferenceValue = freeT.GetComponent<Button>();
        so.FindProperty("freeChestIcon").objectReferenceValue = freeT.Find("ChestBody").GetComponent<Image>();
        so.FindProperty("freeChestTimer").objectReferenceValue = freeT.Find("Subtext").GetComponent<TextMeshProUGUI>();
        so.FindProperty("freeChestReadyBadge").objectReferenceValue = freeT.Find("ReadyBadge").gameObject;

        so.FindProperty("blockChestButton").objectReferenceValue = blockT.GetComponent<Button>();
        so.FindProperty("blockChestIcon").objectReferenceValue = blockT.Find("ChestBody").GetComponent<Image>();
        so.FindProperty("blockChestProgress").objectReferenceValue = blockT.Find("Subtext").GetComponent<TextMeshProUGUI>();
        so.FindProperty("blockChestReadyBadge").objectReferenceValue = blockT.Find("ReadyBadge").gameObject;

        so.FindProperty("premiumChestButton").objectReferenceValue = premT.GetComponent<Button>();
        so.FindProperty("premiumChestCost").objectReferenceValue = premT.Find("CostRow/Cost").GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();
    }

    private static void CreateChestConfig()
    {
        if (!Directory.Exists(CONFIG_DIR)) Directory.CreateDirectory(CONFIG_DIR);
        ChestData existing = AssetDatabase.LoadAssetAtPath<ChestData>(CHEST_CONFIG_PATH);
        if (existing != null) return;

        ChestData data = ScriptableObject.CreateInstance<ChestData>();
        data.freeChestCooldownSeconds = 300f;
        data.blocksPerChest = 20;
        data.premiumChestGemsCost = 50;

        ChestTypeData free = new ChestTypeData
        {
            type = ChestType.Free,
            displayName = "Free Chest",
            chestColor = new Color(0.4f, 0.7f, 0.4f),
            accentColor = new Color(0.6f, 0.95f, 0.6f),
            rewardsCount = 4,
            possibleRewards = new List<ChestReward>
            {
                new ChestReward { id = "coins", color = UIColors.ACCENT, minAmount = 30, maxAmount = 100, weight = 5f },
                new ChestReward { id = "gems", color = UIColors.GEM, minAmount = 1, maxAmount = 2, weight = 1f },
                new ChestReward { id = "pickaxe_lvl1", color = new Color(0.6f, 0.6f, 0.6f), minAmount = 1, maxAmount = 1, weight = 3f },
                new ChestReward { id = "pickaxe_lvl2", color = new Color(0.45f, 0.45f, 0.5f), minAmount = 1, maxAmount = 1, weight = 1f },
            }
        };

        ChestTypeData block = new ChestTypeData
        {
            type = ChestType.Block,
            displayName = "Block Chest",
            chestColor = new Color(0.5f, 0.35f, 0.2f),
            accentColor = new Color(0.95f, 0.78f, 0.25f),
            rewardsCount = 5,
            possibleRewards = new List<ChestReward>
            {
                new ChestReward { id = "coins", color = UIColors.ACCENT, minAmount = 80, maxAmount = 300, weight = 4f },
                new ChestReward { id = "gems", color = UIColors.GEM, minAmount = 2, maxAmount = 5, weight = 1.5f },
                new ChestReward { id = "pickaxe_lvl2", color = new Color(0.45f, 0.45f, 0.5f), minAmount = 1, maxAmount = 1, weight = 2.5f },
                new ChestReward { id = "pickaxe_lvl3", color = new Color(0.72f, 0.5f, 0.32f), minAmount = 1, maxAmount = 1, weight = 1.5f },
                new ChestReward { id = "pickaxe_lvl4", color = new Color(0.78f, 0.78f, 0.82f), minAmount = 1, maxAmount = 1, weight = 0.7f },
            }
        };

        ChestTypeData premium = new ChestTypeData
        {
            type = ChestType.Premium,
            displayName = "Premium Chest",
            chestColor = new Color(0.4f, 0.2f, 0.55f),
            accentColor = new Color(0.85f, 0.55f, 0.95f),
            rewardsCount = 6,
            possibleRewards = new List<ChestReward>
            {
                new ChestReward { id = "coins", color = UIColors.ACCENT, minAmount = 300, maxAmount = 800, weight = 3f },
                new ChestReward { id = "gems", color = UIColors.GEM, minAmount = 5, maxAmount = 15, weight = 2f },
                new ChestReward { id = "pickaxe_lvl4", color = new Color(0.78f, 0.78f, 0.82f), minAmount = 1, maxAmount = 1, weight = 2f },
                new ChestReward { id = "pickaxe_lvl5", color = new Color(0.95f, 0.78f, 0.25f), minAmount = 1, maxAmount = 1, weight = 1.5f },
                new ChestReward { id = "pickaxe_lvl6", color = new Color(0.65f, 0.18f, 0.18f), minAmount = 1, maxAmount = 1, weight = 1f },
            }
        };

        data.chests = new List<ChestTypeData> { free, block, premium };
        AssetDatabase.CreateAsset(data, CHEST_CONFIG_PATH);
        AssetDatabase.SaveAssets();
        Debug.Log("ChestConfig.asset created");
    }
}
#endif

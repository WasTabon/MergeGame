#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration12_Setup
{
    private const string MENU_SCENE_PATH = "Assets/MergeMining/Scenes/MainMenu.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 12) Update MainMenu Scene")]
    public static void UpdateMainMenuScene()
    {
        EditorSceneManager.OpenScene(MENU_SCENE_PATH);

        AchievementsListPopup popup = BuildAchievementsListPopup();
        AddAchievementsButton(popup);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 12: MainMenu updated with achievements panel.");
    }

    private static AchievementsListPopup BuildAchievementsListPopup()
    {
        GameObject existing = GameObject.Find("AchievementsListPopup");
        if (existing != null) Object.DestroyImmediate(existing.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("AchievementsListPopupCanvas", 220);

        GameObject popupGo = new GameObject("AchievementsListPopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(980f, 1700f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "ACHIEVEMENTS", 80, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(800f, 100f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject summary = UIBuildUtils.CreateText("Summary", content.transform, "0/10 UNLOCKED", 36, new Color(1f, 1f, 1f, 0.7f));
        UIBuildUtils.SetSize(summary, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -190f), new Vector2(700f, 50f));

        GameObject scrollArea = UIBuildUtils.CreateImage("ScrollArea", content.transform, new Color(0f, 0f, 0f, 0.2f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(scrollArea, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(900f, 1200f));

        ScrollRect scrollRect = scrollArea.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.1f;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
        scrollRect.scrollSensitivity = 30f;

        GameObject viewportGo = new GameObject("Viewport");
        RectTransform viewRt = viewportGo.AddComponent<RectTransform>();
        viewRt.SetParent(scrollArea.transform, false);
        viewRt.anchorMin = Vector2.zero; viewRt.anchorMax = Vector2.one;
        viewRt.offsetMin = new Vector2(10f, 10f); viewRt.offsetMax = new Vector2(-10f, -10f);
        Image viewImg = viewportGo.AddComponent<Image>();
        viewImg.color = new Color(1f, 1f, 1f, 0.01f);
        viewportGo.AddComponent<Mask>().showMaskGraphic = false;

        GameObject contentRowsGo = new GameObject("Content");
        RectTransform contentRt = contentRowsGo.AddComponent<RectTransform>();
        contentRt.SetParent(viewportGo.transform, false);
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.anchoredPosition = Vector2.zero;
        contentRt.sizeDelta = new Vector2(0f, 0f);

        VerticalLayoutGroup vlg = contentRowsGo.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 14f;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;

        ContentSizeFitter csf = contentRowsGo.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewRt;
        scrollRect.content = contentRt;

        GameObject itemTemplate = BuildItemTemplate(popupGo.transform);

        GameObject closeBtn = UIBuildUtils.CreateButton("CloseButton", content.transform, "CLOSE", UIColors.PRIMARY, Color.white, 56);
        UIBuildUtils.SetSize(closeBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 100f), new Vector2(620f, 150f));

        AchievementsListPopup popup = popupGo.AddComponent<AchievementsListPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = true;
        so.FindProperty("pausesGame").boolValue = false;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("summaryText").objectReferenceValue = summary.GetComponent<TextMeshProUGUI>();
        so.FindProperty("contentContainer").objectReferenceValue = contentRt;
        so.FindProperty("itemTemplate").objectReferenceValue = itemTemplate.GetComponent<AchievementListItem>();
        so.FindProperty("closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static GameObject BuildItemTemplate(Transform parent)
    {
        GameObject existing = GameObject.Find("AchievementListItemTemplate");
        if (existing != null) return existing;

        GameObject root = new GameObject("AchievementListItemTemplate");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.sizeDelta = new Vector2(0f, 180f);

        LayoutElement le = root.AddComponent<LayoutElement>();
        le.preferredHeight = 180f;
        le.minHeight = 180f;

        Image bg = root.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);
        bg.sprite = UIBuildUtils.GetUISprite();
        bg.type = Image.Type.Sliced;

        GameObject iconBg = UIBuildUtils.CreateImage("IconBg", root.transform, new Color(0.4f, 0.4f, 0.45f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(iconBg, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(80f, 0f), new Vector2(120f, 120f));
        iconBg.GetComponent<Image>().raycastTarget = false;

        GameObject trophy = UIBuildUtils.CreateText("Trophy", iconBg.transform, "★", 80, new Color(0.7f, 0.7f, 0.75f));
        UIBuildUtils.SetRect(trophy, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        trophy.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject checkmark = UIBuildUtils.CreateText("Checkmark", iconBg.transform, "✓", 56, new Color(0.4f, 0.95f, 0.4f));
        UIBuildUtils.SetSize(checkmark, new Vector2(1f, 1f), new Vector2(0.5f, 0.5f), new Vector2(-5f, -5f), new Vector2(50f, 50f));
        checkmark.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        checkmark.SetActive(false);

        GameObject title = UIBuildUtils.CreateText("Title", root.transform, "Achievement Title", 38, Color.white, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(title, new Vector2(0f, 0.6f), new Vector2(1f, 1f), new Vector2(160f, -15f), new Vector2(-180f, -10f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject desc = UIBuildUtils.CreateText("Description", root.transform, "Description goes here", 26, new Color(1f, 1f, 1f, 0.7f), TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(desc, new Vector2(0f, 0.35f), new Vector2(1f, 0.6f), new Vector2(160f, 0f), new Vector2(-180f, 0f));

        GameObject progressBg = UIBuildUtils.CreateImage("ProgressBg", root.transform, new Color(0f, 0f, 0f, 0.45f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(progressBg, new Vector2(0f, 0.1f), new Vector2(1f, 0.3f), new Vector2(160f, 0f), new Vector2(-180f, 0f));
        progressBg.GetComponent<Image>().raycastTarget = false;

        GameObject progressFill = UIBuildUtils.CreateImage("ProgressFill", progressBg.transform, new Color(0.55f, 0.565f, 0.886f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(progressFill, Vector2.zero, Vector2.one, new Vector2(4f, 4f), new Vector2(-4f, -4f));
        Image fillImg = progressFill.GetComponent<Image>();
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        fillImg.fillAmount = 0.3f;
        fillImg.raycastTarget = false;

        GameObject progressText = UIBuildUtils.CreateText("ProgressText", progressBg.transform, "0/10", 22, Color.white);
        UIBuildUtils.SetRect(progressText, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        progressText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        progressText.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        GameObject rewardRow = new GameObject("RewardRow");
        RectTransform rrt = rewardRow.AddComponent<RectTransform>();
        rrt.SetParent(root.transform, false);
        rrt.anchorMin = new Vector2(1f, 0.5f);
        rrt.anchorMax = new Vector2(1f, 0.5f);
        rrt.pivot = new Vector2(1f, 0.5f);
        rrt.anchoredPosition = new Vector2(-30f, 0f);
        rrt.sizeDelta = new Vector2(140f, 70f);

        GameObject gemIcon = UIBuildUtils.CreateImage("GemIcon", rewardRow.transform, UIColors.GEM, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemIcon, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(25f, 0f), new Vector2(46f, 46f));
        gemIcon.GetComponent<Image>().raycastTarget = false;

        GameObject reward = UIBuildUtils.CreateText("RewardText", rewardRow.transform, "+1", 36, Color.white, TextAlignmentOptions.MidlineLeft);
        UIBuildUtils.SetRect(reward, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(60f, 0f), new Vector2(-5f, 0f));
        reward.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        AchievementListItem item = root.AddComponent<AchievementListItem>();
        item.iconBg = iconBg.GetComponent<Image>();
        item.trophyIcon = null;
        item.titleText = title.GetComponent<TextMeshProUGUI>();
        item.descriptionText = desc.GetComponent<TextMeshProUGUI>();
        item.progressText = progressText.GetComponent<TextMeshProUGUI>();
        item.progressFill = fillImg;
        item.gemIcon = gemIcon.GetComponent<Image>();
        item.rewardText = reward.GetComponent<TextMeshProUGUI>();
        item.checkmark = checkmark;

        root.SetActive(false);
        return root;
    }

    private static void AddAchievementsButton(AchievementsListPopup popup)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        Debug.Assert(canvasGo != null);

        Transform safeAreaT = canvasGo.transform.Find("SafeArea");
        if (safeAreaT == null) return;

        Transform existing = safeAreaT.Find("AchievementsButton");
        if (existing != null) Object.DestroyImmediate(existing.gameObject);

        Transform playBtn = safeAreaT.Find("PlayButton");
        if (playBtn != null)
        {
            RectTransform playRt = playBtn as RectTransform;
            playRt.anchoredPosition = new Vector2(0f, -10f);
        }

        GameObject achBtn = UIBuildUtils.CreateButton("AchievementsButton", safeAreaT, "ACHIEVEMENTS", new Color(0.55f, 0.35f, 0.8f, 1f), Color.white, 56);
        UIBuildUtils.SetSize(achBtn, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -210f), new Vector2(560f, 150f));

        AchievementsButtonHandler handler = canvasGo.GetComponent<AchievementsButtonHandler>();
        if (handler == null) handler = canvasGo.AddComponent<AchievementsButtonHandler>();
        SerializedObject so = new SerializedObject(handler);
        so.FindProperty("button").objectReferenceValue = achBtn.GetComponent<Button>();
        so.FindProperty("popup").objectReferenceValue = popup;
        so.ApplyModifiedProperties();

        Transform settingsBtn = safeAreaT.Find("SettingsButton");
        if (settingsBtn != null)
        {
            RectTransform srt = settingsBtn as RectTransform;
            srt.anchoredPosition = new Vector2(0f, -400f);
        }
    }
}
#endif

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration08_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 8) Update Game Scene")]
    public static void UpdateGameScene()
    {
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        OfflineRewardsPopup offlinePopup = BuildOfflineRewardsPopup();
        TutorialOverlay tutorialOverlay = BuildTutorialOverlay();

        AttachOfflineRewardsManager(offlinePopup);
        AttachTutorialManager(tutorialOverlay);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 8: Game scene updated.");
    }

    private static OfflineRewardsPopup BuildOfflineRewardsPopup()
    {
        GameObject old = GameObject.Find("OfflineRewardsPopup");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("OfflineRewardsPopupCanvas", 240);

        GameObject popupGo = new GameObject("OfflineRewardsPopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 1200f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "WELCOME BACK!", 84, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -130f), new Vector2(800f, 110f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject timeAway = UIBuildUtils.CreateText("TimeAway", content.transform, "AWAY FOR 2h 14m", 44, new Color(1f, 1f, 1f, 0.85f));
        UIBuildUtils.SetSize(timeAway, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -260f), new Vector2(700f, 70f));

        GameObject coinsIcon = UIBuildUtils.CreateImage("CoinsIcon", content.transform, UIColors.ACCENT, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(coinsIcon, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 130f), new Vector2(220f, 220f));
        coinsIcon.GetComponent<Image>().raycastTarget = false;

        GameObject coinsAmount = UIBuildUtils.CreateText("CoinsAmount", content.transform, "+0", 110, Color.white);
        UIBuildUtils.SetSize(coinsAmount, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -90f), new Vector2(700f, 130f));
        coinsAmount.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject claimBtn = UIBuildUtils.CreateButton("ClaimButton", content.transform, "CLAIM", UIColors.GREEN, Color.white, 56);
        UIBuildUtils.SetSize(claimBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180f, 120f), new Vector2(320f, 150f));

        GameObject claimDoubleBtn = UIBuildUtils.CreateButton("ClaimDoubleButton", content.transform, "x2 CLAIM", UIColors.PRIMARY, Color.white, 50);
        UIBuildUtils.SetSize(claimDoubleBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(180f, 120f), new Vector2(320f, 150f));

        OfflineRewardsPopup popup = popupGo.AddComponent<OfflineRewardsPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("timeAwayText").objectReferenceValue = timeAway.GetComponent<TextMeshProUGUI>();
        so.FindProperty("coinsAmountText").objectReferenceValue = coinsAmount.GetComponent<TextMeshProUGUI>();
        so.FindProperty("coinsIcon").objectReferenceValue = coinsIcon.GetComponent<RectTransform>();
        so.FindProperty("claimButton").objectReferenceValue = claimBtn.GetComponent<Button>();
        so.FindProperty("claimDoubleButton").objectReferenceValue = claimDoubleBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static TutorialOverlay BuildTutorialOverlay()
    {
        GameObject old = GameObject.Find("TutorialOverlay");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("TutorialOverlayCanvas", 245);

        GameObject root = new GameObject("TutorialOverlay");
        RectTransform rt = root.AddComponent<RectTransform>();
        rt.SetParent(canvasGo.transform, false);
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        CanvasGroup cg = root.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;

        GameObject dimmer = UIBuildUtils.CreateImage("Dimmer", root.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(dimmer, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        dimmer.GetComponent<Image>().raycastTarget = false;

        GameObject ring = UIBuildUtils.CreateImage("HighlightRing", root.transform, new Color(1f, 1f, 1f, 0.7f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(ring, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300f, 300f));
        ring.GetComponent<Image>().raycastTarget = false;

        GameObject hand = UIBuildUtils.CreateImage("HandPointer", root.transform, new Color(1f, 1f, 1f, 0.95f), UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(hand, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(80f, 80f));
        hand.GetComponent<Image>().raycastTarget = false;

        GameObject instruction = UIBuildUtils.CreateText("Instruction", root.transform, "TAP HERE", 64, Color.white);
        UIBuildUtils.SetSize(instruction, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -380f), new Vector2(900f, 100f));
        instruction.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject skipBtn = UIBuildUtils.CreateButton("SkipButton", root.transform, "SKIP", new Color(0f, 0f, 0f, 0.5f), Color.white, 36);
        UIBuildUtils.SetSize(skipBtn, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-110f, 50f), new Vector2(180f, 70f));

        TutorialOverlay overlay = root.AddComponent<TutorialOverlay>();
        SerializedObject so = new SerializedObject(overlay);
        so.FindProperty("canvasGroup").objectReferenceValue = cg;
        so.FindProperty("dimmer").objectReferenceValue = dimmer.GetComponent<Image>();
        so.FindProperty("highlightRing").objectReferenceValue = ring.GetComponent<RectTransform>();
        so.FindProperty("handPointer").objectReferenceValue = hand.GetComponent<RectTransform>();
        so.FindProperty("instructionText").objectReferenceValue = instruction.GetComponent<TextMeshProUGUI>();
        so.FindProperty("skipButton").objectReferenceValue = skipBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return overlay;
    }

    private static void AttachOfflineRewardsManager(OfflineRewardsPopup popup)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        OfflineRewardsManager mgr = Object.FindObjectOfType<OfflineRewardsManager>();
        if (mgr == null) mgr = canvasGo.AddComponent<OfflineRewardsManager>();

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("popup").objectReferenceValue = popup;
        so.FindProperty("maxOfflineSeconds").floatValue = 4f * 3600f;
        so.FindProperty("minOfflineSecondsToShow").floatValue = 60f;
        so.FindProperty("coinsPerSecondPerLevel").floatValue = 0.5f;
        so.ApplyModifiedProperties();
    }

    private static void AttachTutorialManager(TutorialOverlay overlay)
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        TutorialManager mgr = Object.FindObjectOfType<TutorialManager>();
        if (mgr == null) mgr = canvasGo.AddComponent<TutorialManager>();

        RectTransform shopBtnRt = null;
        GameObject shopGo = GameObject.Find("ShopButton");
        if (shopGo != null) shopBtnRt = shopGo.GetComponent<RectTransform>();

        RectTransform pickaxeGridRt = null;
        GameObject gridGo = GameObject.Find("PickaxeGrid");
        if (gridGo != null) pickaxeGridRt = gridGo.GetComponent<RectTransform>();

        RectTransform blocksRowRt = null;
        GameObject blocksGo = GameObject.Find("BlocksRow");
        if (blocksGo != null) blocksRowRt = blocksGo.GetComponent<RectTransform>();

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("overlay").objectReferenceValue = overlay;
        so.FindProperty("shopButtonRef").objectReferenceValue = shopBtnRt;
        so.FindProperty("pickaxeGridRef").objectReferenceValue = pickaxeGridRt;
        so.FindProperty("blocksRowRef").objectReferenceValue = blocksRowRt;
        so.ApplyModifiedProperties();
    }
}
#endif

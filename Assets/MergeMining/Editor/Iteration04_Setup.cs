#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration04_Setup
{
    private const string MENU_SCENE_PATH = "Assets/MergeMining/Scenes/MainMenu.unity";
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 4) Update ALL Scenes")]
    public static void UpdateAll()
    {
        UpdateGameScene();
        UpdateMainMenuScene();
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        Debug.Log("Iteration 4: ALL scenes updated.");
    }

    [MenuItem("Tools/Merge Mining/(Iteration 4) Update Game Scene")]
    public static void UpdateGameScene()
    {
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        ConfirmPopup confirmPopup = BuildConfirmPopup();
        GameSettingsPopup gameSettingsPopup = BuildGameSettingsPopup();
        PausePopup pausePopup = BuildPausePopup(gameSettingsPopup, confirmPopup);
        ZoneCompletePopup zonePopup = BuildZoneCompletePopup();

        WirePauseButton(pausePopup);
        WireZoneCompleteIntoBlocksRow(zonePopup);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 4: Game scene updated.");
    }

    [MenuItem("Tools/Merge Mining/(Iteration 4) Update MainMenu Scene")]
    public static void UpdateMainMenuScene()
    {
        EditorSceneManager.OpenScene(MENU_SCENE_PATH);

        ConfirmPopup confirmPopup = BuildConfirmPopup();
        WireConfirmIntoMenuSettings(confirmPopup);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 4: MainMenu scene updated.");
    }

    private static (GameObject canvas, GameObject popupRoot, GameObject backdrop, Button backdropBtn, GameObject content) BuildPopupCanvas(string popupName, int sortingOrder, Vector2 contentSize)
    {
        GameObject canvasGo = UIBuildUtils.CreateCanvas(popupName + "Canvas", sortingOrder);

        GameObject popupGo = new GameObject(popupName);
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero;
        prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;
        ColorBlock cb = backdropBtn.colors; cb.normalColor = Color.white; cb.highlightedColor = Color.white; backdropBtn.colors = cb;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, contentSize);

        return (canvasGo, popupGo, backdrop, backdropBtn, content);
    }

    private static PausePopup BuildPausePopup(GameSettingsPopup gameSettings, ConfirmPopup confirm)
    {
        GameObject existing = GameObject.Find("PausePopup");
        if (existing != null) Object.DestroyImmediate(existing.transform.parent.gameObject);

        var built = BuildPopupCanvas("PausePopup", 200, new Vector2(800f, 1100f));

        GameObject title = UIBuildUtils.CreateText("Title", built.content.transform, "PAUSED", 90, Color.white);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -130f), new Vector2(600f, 120f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject resume = UIBuildUtils.CreateButton("ResumeButton", built.content.transform, "RESUME", UIColors.GREEN, Color.white, 60);
        UIBuildUtils.SetSize(resume, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 160f), new Vector2(600f, 160f));

        GameObject settings = UIBuildUtils.CreateButton("SettingsButton", built.content.transform, "SETTINGS", UIColors.PRIMARY, Color.white, 60);
        UIBuildUtils.SetSize(settings, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -40f), new Vector2(600f, 160f));

        GameObject mainMenu = UIBuildUtils.CreateButton("MainMenuButton", built.content.transform, "MAIN MENU", new Color(0.4f, 0.4f, 0.45f), Color.white, 60);
        UIBuildUtils.SetSize(mainMenu, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -240f), new Vector2(600f, 160f));

        PausePopup popup = built.popupRoot.AddComponent<PausePopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = built.content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = built.backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = built.backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("resumeButton").objectReferenceValue = resume.GetComponent<Button>();
        so.FindProperty("settingsButton").objectReferenceValue = settings.GetComponent<Button>();
        so.FindProperty("mainMenuButton").objectReferenceValue = mainMenu.GetComponent<Button>();
        so.FindProperty("gameSettingsPopup").objectReferenceValue = gameSettings;
        so.FindProperty("confirmPopup").objectReferenceValue = confirm;
        so.ApplyModifiedProperties();

        return popup;
    }

    private static GameSettingsPopup BuildGameSettingsPopup()
    {
        GameObject existing = GameObject.Find("GameSettingsPopup");
        if (existing != null) Object.DestroyImmediate(existing.transform.parent.gameObject);

        var built = BuildPopupCanvas("GameSettingsPopup", 210, new Vector2(900f, 1000f));

        GameObject title = UIBuildUtils.CreateText("Title", built.content.transform, "SETTINGS", 80, Color.white);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -100f), new Vector2(700f, 100f));

        GameObject sfxToggle = UIBuildUtils.CreateToggle("SfxToggle", built.content.transform, "SFX", true);
        UIBuildUtils.SetSize(sfxToggle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -240f), new Vector2(700f, 100f));

        GameObject sfxSlider = UIBuildUtils.CreateSlider("SfxSlider", built.content.transform, 1f);
        UIBuildUtils.SetSize(sfxSlider, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -340f), new Vector2(700f, 60f));

        GameObject musicToggle = UIBuildUtils.CreateToggle("MusicToggle", built.content.transform, "MUSIC", true);
        UIBuildUtils.SetSize(musicToggle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -460f), new Vector2(700f, 100f));

        GameObject musicSlider = UIBuildUtils.CreateSlider("MusicSlider", built.content.transform, 0.6f);
        UIBuildUtils.SetSize(musicSlider, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -560f), new Vector2(700f, 60f));

        GameObject hapticToggle = UIBuildUtils.CreateToggle("HapticToggle", built.content.transform, "VIBRATION", true);
        UIBuildUtils.SetSize(hapticToggle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -680f), new Vector2(700f, 100f));

        GameObject closeBtn = UIBuildUtils.CreateButton("CloseButton", built.content.transform, "CLOSE", UIColors.PRIMARY, Color.white, 56);
        UIBuildUtils.SetSize(closeBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 110f), new Vector2(600f, 140f));

        GameSettingsPopup popup = built.popupRoot.AddComponent<GameSettingsPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = built.content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = built.backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = built.backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = false;
        so.FindProperty("sfxToggle").objectReferenceValue = sfxToggle.GetComponent<Toggle>();
        so.FindProperty("musicToggle").objectReferenceValue = musicToggle.GetComponent<Toggle>();
        so.FindProperty("hapticToggle").objectReferenceValue = hapticToggle.GetComponent<Toggle>();
        so.FindProperty("sfxSlider").objectReferenceValue = sfxSlider.GetComponent<Slider>();
        so.FindProperty("musicSlider").objectReferenceValue = musicSlider.GetComponent<Slider>();
        so.FindProperty("closeButton").objectReferenceValue = closeBtn.GetComponent<Button>();
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static ConfirmPopup BuildConfirmPopup()
    {
        GameObject existing = GameObject.Find("ConfirmPopup");
        if (existing != null) Object.DestroyImmediate(existing.transform.parent.gameObject);

        var built = BuildPopupCanvas("ConfirmPopup", 230, new Vector2(820f, 600f));

        GameObject title = UIBuildUtils.CreateText("Title", built.content.transform, "TITLE", 64, Color.white);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -90f), new Vector2(700f, 90f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject message = UIBuildUtils.CreateText("Message", built.content.transform, "Message text", 36, new Color(1f, 1f, 1f, 0.85f));
        UIBuildUtils.SetSize(message, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(700f, 200f));
        message.GetComponent<TextMeshProUGUI>().enableWordWrapping = true;

        GameObject cancelBtn = UIBuildUtils.CreateButton("CancelButton", built.content.transform, "CANCEL", new Color(0.4f, 0.4f, 0.45f), Color.white, 52);
        UIBuildUtils.SetSize(cancelBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180f, 110f), new Vector2(320f, 140f));

        GameObject confirmBtn = UIBuildUtils.CreateButton("ConfirmButton", built.content.transform, "CONFIRM", UIColors.RED, Color.white, 52);
        UIBuildUtils.SetSize(confirmBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(180f, 110f), new Vector2(320f, 140f));

        TextMeshProUGUI cancelLabel = cancelBtn.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI confirmLabel = confirmBtn.transform.Find("Label").GetComponent<TextMeshProUGUI>();

        ConfirmPopup popup = built.popupRoot.AddComponent<ConfirmPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = built.content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = built.backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = built.backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = false;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("messageText").objectReferenceValue = message.GetComponent<TextMeshProUGUI>();
        so.FindProperty("cancelButton").objectReferenceValue = cancelBtn.GetComponent<Button>();
        so.FindProperty("confirmButton").objectReferenceValue = confirmBtn.GetComponent<Button>();
        so.FindProperty("cancelLabel").objectReferenceValue = cancelLabel;
        so.FindProperty("confirmLabel").objectReferenceValue = confirmLabel;
        so.FindProperty("confirmButtonImage").objectReferenceValue = confirmBtn.GetComponent<Image>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static ZoneCompletePopup BuildZoneCompletePopup()
    {
        GameObject existing = GameObject.Find("ZoneCompletePopup");
        if (existing != null) Object.DestroyImmediate(existing.transform.parent.gameObject);

        var built = BuildPopupCanvas("ZoneCompletePopup", 220, new Vector2(900f, 1100f));

        GameObject title = UIBuildUtils.CreateText("Title", built.content.transform, "ZONE\nCOMPLETE!", 100, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -200f), new Vector2(800f, 280f));
        TextMeshProUGUI titleTmp = title.GetComponent<TextMeshProUGUI>();
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.outlineWidth = 0.35f;

        GameObject gemIcon = UIBuildUtils.CreateImage("GemIcon", built.content.transform, UIColors.GEM, UIBuildUtils.GetKnobSprite());
        UIBuildUtils.SetSize(gemIcon, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 90f), new Vector2(200f, 200f));
        gemIcon.GetComponent<Image>().raycastTarget = false;

        GameObject rewardText = UIBuildUtils.CreateText("RewardText", built.content.transform, "+1", 96, Color.white);
        UIBuildUtils.SetSize(rewardText, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -110f), new Vector2(400f, 120f));
        rewardText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject continueBtn = UIBuildUtils.CreateButton("ContinueButton", built.content.transform, "CONTINUE", UIColors.GREEN, Color.white, 60);
        UIBuildUtils.SetSize(continueBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 140f), new Vector2(640f, 170f));

        ZoneCompletePopup popup = built.popupRoot.AddComponent<ZoneCompletePopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = built.content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = built.backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = built.backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("rewardText").objectReferenceValue = rewardText.GetComponent<TextMeshProUGUI>();
        so.FindProperty("gemIcon").objectReferenceValue = gemIcon.GetComponent<RectTransform>();
        so.FindProperty("continueButton").objectReferenceValue = continueBtn.GetComponent<Button>();
        so.ApplyModifiedProperties();

        return popup;
    }

    private static void WirePauseButton(PausePopup pausePopup)
    {
        GameObject pauseBtnGo = GameObject.Find("PauseButton");
        if (pauseBtnGo == null)
        {
            Debug.LogWarning("PauseButton not found!");
            return;
        }

        GameObject canvasGo = GameObject.Find("Canvas");
        PauseButtonHandler handler = canvasGo.GetComponent<PauseButtonHandler>();
        if (handler == null) handler = canvasGo.AddComponent<PauseButtonHandler>();

        SerializedObject so = new SerializedObject(handler);
        so.FindProperty("pauseButton").objectReferenceValue = pauseBtnGo.GetComponent<Button>();
        so.FindProperty("pausePopup").objectReferenceValue = pausePopup;
        so.ApplyModifiedProperties();
    }

    private static void WireZoneCompleteIntoBlocksRow(ZoneCompletePopup zonePopup)
    {
        BlocksRowManager mgr = Object.FindObjectOfType<BlocksRowManager>();
        if (mgr == null)
        {
            Debug.LogWarning("BlocksRowManager not found!");
            return;
        }
        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("zoneCompletePopup").objectReferenceValue = zonePopup;
        so.FindProperty("blocksPerZone").intValue = 10;
        so.ApplyModifiedProperties();
    }

    private static void WireConfirmIntoMenuSettings(ConfirmPopup confirm)
    {
        SettingsPopup settings = Object.FindObjectOfType<SettingsPopup>(true);
        if (settings == null)
        {
            Debug.LogWarning("SettingsPopup not found in MainMenu!");
            return;
        }
        SerializedObject so = new SerializedObject(settings);
        so.FindProperty("confirmPopup").objectReferenceValue = confirm;
        so.ApplyModifiedProperties();
    }
}
#endif

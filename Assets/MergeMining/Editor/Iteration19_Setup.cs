#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration19_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";
    private const string LEVEL_CONFIG_PATH = "Assets/MergeMining/Resources/LevelConfig.asset";

    [MenuItem("Tools/Merge Mining/(Iteration 19) Update ALL")]
    public static void UpdateAll()
    {
        UpdateLevelConfig();
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);
        BuildModifierChoicePopup();
        AttachStarter();
        BuildSacrificeButton();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 19 done.");
    }

    private static void UpdateLevelConfig()
    {
        LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(LEVEL_CONFIG_PATH);
        if (data == null) { Debug.LogError("LevelConfig not found"); return; }

        List<int>[] sequences = new List<int>[15];
        sequences[0] = new List<int> { 0, 7, 0, 2, 5 };
        sequences[1] = new List<int> { 0, 2, 7, 3, 0, 5 };
        sequences[2] = new List<int> { 0, 7, 2, 1, 4, 5 };
        sequences[3] = new List<int> { 0, 3, 7, 1, 5, 2, 4, 6 };
        sequences[4] = new List<int> { 1, 0, 7, 4, 2, 3, 5, 6 };
        sequences[5] = new List<int> { 0, 7, 1, 3, 4, 2, 5, 6, 7 };
        sequences[6] = new List<int> { 4, 1, 6, 7, 3, 2, 5, 1, 4 };
        sequences[7] = new List<int> { 1, 3, 4, 7, 6, 2, 5, 1, 3, 7 };
        sequences[8] = new List<int> { 1, 4, 7, 6, 3, 5, 1, 4, 7, 6 };
        sequences[9] = new List<int> { 2, 1, 4, 7, 6, 5, 3, 7, 6, 2 };
        sequences[10] = new List<int> { 3, 1, 6, 7, 4, 5, 7, 6, 1, 3, 4 };
        sequences[11] = new List<int> { 1, 6, 7, 4, 5, 3, 7, 6, 4, 1, 5 };
        sequences[12] = new List<int> { 6, 7, 1, 5, 4, 7, 6, 3, 1, 5, 4, 7 };
        sequences[13] = new List<int> { 1, 7, 6, 5, 4, 7, 3, 6, 5, 1, 7, 4 };
        sequences[14] = new List<int> { 6, 7, 1, 5, 7, 4, 3, 7, 6, 1, 5, 4, 7 };

        for (int i = 0; i < 15; i++)
        {
            if (i < 2)
            {
                for (int k = 0; k < sequences[i].Count; k++)
                {
                    if (sequences[i][k] == 1) sequences[i][k] = 0;
                }
            }
        }

        for (int i = 0; i < data.levels.Count && i < 15; i++)
        {
            data.levels[i].blockSequence = sequences[i];
        }
        EditorUtility.SetDirty(data);
        AssetDatabase.SaveAssets();
        Debug.Log("LevelConfig updated: Boss block now from level 1.");
    }

    private static void BuildModifierChoicePopup()
    {
        GameObject old = GameObject.Find("ModifierChoicePopup");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("ModifierChoicePopupCanvas", 246);

        GameObject popupGo = new GameObject("ModifierChoicePopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(950f, 1500f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "CHOOSE A MODIFIER", 64, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -110f), new Vector2(900f, 100f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        List<ModifierChoiceCard> cards = new List<ModifierChoiceCard>();
        float[] yPositions = { 350f, 0f, -350f };
        for (int i = 0; i < 3; i++)
        {
            ModifierChoiceCard card = BuildCard(content.transform, "Card_" + i, yPositions[i]);
            cards.Add(card);
        }

        GameObject skipBtn = UIBuildUtils.CreateButton("SkipButton", content.transform, "SKIP", new Color(0.4f, 0.4f, 0.45f), Color.white, 40);
        UIBuildUtils.SetSize(skipBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 80f), new Vector2(400f, 100f));

        ModifierChoicePopup popup = popupGo.AddComponent<ModifierChoicePopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("skipButton").objectReferenceValue = skipBtn.GetComponent<Button>();
        SerializedProperty cardsProp = so.FindProperty("cards");
        cardsProp.arraySize = cards.Count;
        for (int i = 0; i < cards.Count; i++) cardsProp.GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
        so.ApplyModifiedProperties();
    }

    private static ModifierChoiceCard BuildCard(Transform parent, string name, float anchorY)
    {
        GameObject root = UIBuildUtils.CreateImage(name, parent, new Color(0.95f, 0.5f, 0.3f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(root, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, anchorY), new Vector2(820f, 280f));
        Image colorBg = root.GetComponent<Image>();

        Button btn = root.AddComponent<Button>();
        ColorBlock cb = btn.colors; cb.normalColor = Color.white; btn.colors = cb;
        root.AddComponent<ButtonAnimator>();

        GameObject inner = UIBuildUtils.CreateImage("Inner", root.transform, new Color(0f, 0f, 0f, 0.3f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetRect(inner, Vector2.zero, Vector2.one, new Vector2(8f, 8f), new Vector2(-8f, -8f));
        inner.GetComponent<Image>().raycastTarget = false;

        GameObject titleText = UIBuildUtils.CreateText("Title", root.transform, "TITLE", 50, Color.white);
        UIBuildUtils.SetRect(titleText, new Vector2(0f, 0.55f), new Vector2(1f, 1f), new Vector2(30f, 0f), new Vector2(-30f, -10f));
        titleText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        titleText.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        GameObject descText = UIBuildUtils.CreateText("Description", root.transform, "Desc", 32, new Color(1f, 1f, 1f, 0.9f));
        UIBuildUtils.SetRect(descText, new Vector2(0f, 0.05f), new Vector2(1f, 0.55f), new Vector2(30f, 0f), new Vector2(-30f, 0f));
        descText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        descText.GetComponent<TextMeshProUGUI>().raycastTarget = false;

        ModifierChoiceCard card = root.AddComponent<ModifierChoiceCard>();
        card.button = btn;
        card.colorBackground = colorBg;
        card.titleText = titleText.GetComponent<TextMeshProUGUI>();
        card.descriptionText = descText.GetComponent<TextMeshProUGUI>();
        return card;
    }

    private static void AttachStarter()
    {
        GameObject canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null) return;

        ModifierChoicePopup popup = Object.FindObjectOfType<ModifierChoicePopup>(true);
        if (popup == null) { Debug.LogError("ModifierChoicePopup missing"); return; }

        ModifierChoiceStarter starter = canvasGo.GetComponent<ModifierChoiceStarter>();
        if (starter == null) starter = canvasGo.AddComponent<ModifierChoiceStarter>();
        SerializedObject so = new SerializedObject(starter);
        so.FindProperty("popup").objectReferenceValue = popup;
        so.FindProperty("delaySeconds").floatValue = 0.5f;
        so.ApplyModifiedProperties();
    }

    private static void BuildSacrificeButton()
    {
        GameObject existing = GameObject.Find("SacrificeButton");
        if (existing != null) Object.DestroyImmediate(existing);

        GameObject canvasGo = GameObject.Find("Canvas");
        Transform safeArea = canvasGo.transform.Find("SafeArea");
        if (safeArea == null) return;

        GameObject btnGo = UIBuildUtils.CreateButton("SacrificeButton", safeArea, "SACRIFICE", new Color(0.9f, 0.3f, 0.45f), Color.white, 36);
        UIBuildUtils.SetSize(btnGo, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-130f, 280f), new Vector2(220f, 120f));

        Transform labelT = btnGo.transform.Find("Label");
        TextMeshProUGUI label = labelT != null ? labelT.GetComponent<TextMeshProUGUI>() : null;

        SacrificeButton sb = btnGo.AddComponent<SacrificeButton>();
        SerializedObject so = new SerializedObject(sb);
        so.FindProperty("button").objectReferenceValue = btnGo.GetComponent<Button>();
        so.FindProperty("label").objectReferenceValue = label;
        so.ApplyModifiedProperties();
    }
}
#endif

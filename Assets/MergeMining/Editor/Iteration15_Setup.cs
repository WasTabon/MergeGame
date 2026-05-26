#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class Iteration15_Setup
{
    private const string GAME_SCENE_PATH = "Assets/MergeMining/Scenes/Game.unity";

    [MenuItem("Tools/Merge Mining/(Iteration 15) Update Game Scene")]
    public static void UpdateGameScene()
    {
        EditorSceneManager.OpenScene(GAME_SCENE_PATH);

        RemoveOldTutorialOverlay();
        BuildTextTutorialPopup();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Iteration 15: Game scene updated with text tutorial.");
    }

    [MenuItem("Tools/Merge Mining/(Iteration 15) DEBUG - Reset Tutorial")]
    public static void ResetTextTutorial()
    {
        TextTutorialPopup.ResetDone();
        Debug.Log("Text tutorial reset. Will appear at next Game scene start.");
    }

    private static void RemoveOldTutorialOverlay()
    {
        GameObject existing = GameObject.Find("TutorialOverlayCanvas");
        if (existing != null) Object.DestroyImmediate(existing);
        GameObject existing2 = GameObject.Find("TutorialOverlay");
        if (existing2 != null) Object.DestroyImmediate(existing2);
    }

    private static void BuildTextTutorialPopup()
    {
        GameObject old = GameObject.Find("TextTutorialPopup");
        if (old != null) Object.DestroyImmediate(old.transform.parent.gameObject);

        GameObject canvasGo = UIBuildUtils.CreateCanvas("TextTutorialPopupCanvas", 247);

        GameObject popupGo = new GameObject("TextTutorialPopup");
        RectTransform prt = popupGo.AddComponent<RectTransform>();
        prt.SetParent(canvasGo.transform, false);
        prt.anchorMin = Vector2.zero; prt.anchorMax = Vector2.one;
        prt.offsetMin = Vector2.zero; prt.offsetMax = Vector2.zero;

        GameObject backdrop = UIBuildUtils.CreateImage("Backdrop", popupGo.transform, new Color(0f, 0f, 0f, 0f));
        UIBuildUtils.SetRect(backdrop, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Button backdropBtn = backdrop.AddComponent<Button>();
        backdropBtn.transition = Selectable.Transition.None;

        GameObject content = UIBuildUtils.CreateImage("Content", popupGo.transform, new Color(0.13f, 0.13f, 0.22f, 1f), UIBuildUtils.GetUISprite());
        UIBuildUtils.SetSize(content, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900f, 1100f));

        GameObject title = UIBuildUtils.CreateText("Title", content.transform, "HOW TO PLAY", 72, UIColors.ACCENT);
        UIBuildUtils.SetSize(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -110f), new Vector2(800f, 110f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        GameObject body = UIBuildUtils.CreateText("Body", content.transform, "Body text goes here", 38, Color.white);
        UIBuildUtils.SetSize(body, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 30f), new Vector2(760f, 700f));
        TextMeshProUGUI bodyTmp = body.GetComponent<TextMeshProUGUI>();
        bodyTmp.alignment = TextAlignmentOptions.Center;
        bodyTmp.enableWordWrapping = true;

        GameObject stepIndicator = UIBuildUtils.CreateText("StepIndicator", content.transform, "1 / 5", 32, new Color(1f, 1f, 1f, 0.65f));
        UIBuildUtils.SetSize(stepIndicator, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 290f), new Vector2(400f, 50f));

        GameObject nextBtn = UIBuildUtils.CreateButton("NextButton", content.transform, "NEXT", UIColors.GREEN, Color.white, 56);
        UIBuildUtils.SetSize(nextBtn, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 110f), new Vector2(620f, 160f));
        Transform nextLabelT = nextBtn.transform.Find("Label");
        TextMeshProUGUI nextLabel = nextLabelT != null ? nextLabelT.GetComponent<TextMeshProUGUI>() : null;

        TextTutorialPopup popup = popupGo.AddComponent<TextTutorialPopup>();
        SerializedObject so = new SerializedObject(popup);
        so.FindProperty("content").objectReferenceValue = content.GetComponent<RectTransform>();
        so.FindProperty("backdrop").objectReferenceValue = backdrop.GetComponent<Image>();
        so.FindProperty("backdropButton").objectReferenceValue = backdropBtn;
        so.FindProperty("closeOnBackdrop").boolValue = false;
        so.FindProperty("pausesGame").boolValue = true;
        so.FindProperty("titleText").objectReferenceValue = title.GetComponent<TextMeshProUGUI>();
        so.FindProperty("bodyText").objectReferenceValue = bodyTmp;
        so.FindProperty("stepIndicatorText").objectReferenceValue = stepIndicator.GetComponent<TextMeshProUGUI>();
        so.FindProperty("nextButton").objectReferenceValue = nextBtn.GetComponent<Button>();
        so.FindProperty("nextButtonLabel").objectReferenceValue = nextLabel;

        SerializedProperty stepsProp = so.FindProperty("steps");
        var stepsData = new List<(string title, string body)>
        {
            ("WELCOME!", "Welcome to Merge Mining! In each level you'll build pickaxes to break a fixed number of blocks."),
            ("BUY PICKAXES", "Tap the SHOP button to buy a pickaxe with your coins.\n\nEach level gives you a fixed coin budget — spend it wisely!"),
            ("MERGE PICKAXES", "Drag a pickaxe onto another of the same level to merge them into a stronger one.\n\nMerged pickaxes deal way more damage!"),
            ("START THE BATTLE", "When you're ready, tap the green START button.\n\nYour pickaxes will automatically attack the blocks above."),
            ("WIN THE LEVEL", "Destroy all blocks to win!\n\nThe more pickaxes you save, the more stars you get.\n\nGood luck!")
        };
        stepsProp.arraySize = stepsData.Count;
        for (int i = 0; i < stepsData.Count; i++)
        {
            SerializedProperty entry = stepsProp.GetArrayElementAtIndex(i);
            entry.FindPropertyRelative("title").stringValue = stepsData[i].title;
            entry.FindPropertyRelative("body").stringValue = stepsData[i].body;
        }
        so.ApplyModifiedProperties();
    }
}
#endif

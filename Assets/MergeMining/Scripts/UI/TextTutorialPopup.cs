using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextTutorialPopup : BasePopup
{
    [System.Serializable]
    public class TutorialStepData
    {
        public string title;
        [TextArea(2, 5)] public string body;
    }

    [System.Serializable]
    public class TutorialGroup
    {
        public int triggerLevel = 1;
        public string doneKey = "text_tutorial_done_1";
        public List<TutorialStepData> steps = new List<TutorialStepData>();
    }

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI stepIndicatorText;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextButtonLabel;
    [SerializeField] private List<TutorialGroup> groups;

    private TutorialGroup currentGroup;
    private int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        if (nextButton != null) nextButton.onClick.AddListener(OnNext);
    }

    public static bool IsDone()
    {
        return PlayerPrefs.GetInt("text_tutorial_done_1", 0) == 1
            && PlayerPrefs.GetInt("text_tutorial_done_2", 0) == 1
            && PlayerPrefs.GetInt("text_tutorial_done_3", 0) == 1;
    }

    public static bool IsGroupDone(string key)
    {
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public static void MarkDone(string key)
    {
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey("text_tutorial_done_1");
        PlayerPrefs.DeleteKey("text_tutorial_done_2");
        PlayerPrefs.DeleteKey("text_tutorial_done_3");
        PlayerPrefs.DeleteKey("text_tutorial_done");
        PlayerPrefs.Save();
    }

    public void TryShowTutorial()
    {
        if (LevelManager.Instance == null) return;
        int level = LevelManager.Instance.CurrentLevelNumber;
        TutorialGroup match = null;
        foreach (var g in groups)
        {
            if (g.triggerLevel == level && !IsGroupDone(g.doneKey)) { match = g; break; }
        }
        if (match == null) return;
        currentGroup = match;
        currentIndex = 0;
        ShowCurrentStep();
        Show();
    }

    public bool HasPendingTutorialForLevel(int level)
    {
        foreach (var g in groups)
        {
            if (g.triggerLevel == level && !IsGroupDone(g.doneKey)) return true;
        }
        return false;
    }

    private void ShowCurrentStep()
    {
        if (currentGroup == null || currentGroup.steps == null || currentGroup.steps.Count == 0) return;
        if (currentIndex < 0) currentIndex = 0;
        if (currentIndex >= currentGroup.steps.Count) currentIndex = currentGroup.steps.Count - 1;

        TutorialStepData step = currentGroup.steps[currentIndex];
        if (titleText != null) titleText.text = step.title;
        if (bodyText != null) bodyText.text = step.body;
        if (stepIndicatorText != null) stepIndicatorText.text = (currentIndex + 1) + " / " + currentGroup.steps.Count;
        if (nextButtonLabel != null) nextButtonLabel.text = (currentIndex == currentGroup.steps.Count - 1) ? "LET'S GO!" : "NEXT";
    }

    private void OnNext()
    {
        if (currentGroup == null) { Hide(); return; }
        if (currentIndex >= currentGroup.steps.Count - 1)
        {
            MarkDone(currentGroup.doneKey);
            Hide();
            return;
        }
        currentIndex++;
        ShowCurrentStep();
    }
}

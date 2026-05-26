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

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bodyText;
    [SerializeField] private TextMeshProUGUI stepIndicatorText;
    [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI nextButtonLabel;
    [SerializeField] private List<TutorialStepData> steps;

    private const string DONE_KEY = "text_tutorial_done";

    private int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        if (nextButton != null) nextButton.onClick.AddListener(OnNext);
    }

    public static bool IsDone()
    {
        return PlayerPrefs.GetInt(DONE_KEY, 0) == 1;
    }

    public static void MarkDone()
    {
        PlayerPrefs.SetInt(DONE_KEY, 1);
        PlayerPrefs.Save();
    }

    public static void ResetDone()
    {
        PlayerPrefs.DeleteKey(DONE_KEY);
        PlayerPrefs.Save();
    }

    public void TryShowTutorial()
    {
        if (IsDone()) return;
        if (LevelManager.Instance != null && LevelManager.Instance.CurrentLevelNumber != 1)
        {
            MarkDone();
            return;
        }
        currentIndex = 0;
        ShowCurrentStep();
        Show();
    }

    private void ShowCurrentStep()
    {
        if (steps == null || steps.Count == 0) return;
        if (currentIndex < 0) currentIndex = 0;
        if (currentIndex >= steps.Count) currentIndex = steps.Count - 1;

        TutorialStepData step = steps[currentIndex];
        if (titleText != null) titleText.text = step.title;
        if (bodyText != null) bodyText.text = step.body;
        if (stepIndicatorText != null) stepIndicatorText.text = (currentIndex + 1) + " / " + steps.Count;
        if (nextButtonLabel != null) nextButtonLabel.text = (currentIndex == steps.Count - 1) ? "LET'S GO!" : "NEXT";
    }

    private void OnNext()
    {
        if (currentIndex >= steps.Count - 1)
        {
            MarkDone();
            Hide();
            return;
        }
        currentIndex++;
        ShowCurrentStep();
    }
}

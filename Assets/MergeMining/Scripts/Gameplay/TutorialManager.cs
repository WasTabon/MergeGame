using System;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    private const string TUTORIAL_DONE_KEY = "tutorial_done";

    [SerializeField] private TutorialOverlay overlay;
    [SerializeField] private RectTransform shopButtonRef;
    [SerializeField] private RectTransform pickaxeGridRef;
    [SerializeField] private RectTransform blocksRowRef;

    private TutorialStep currentStep = TutorialStep.None;
    private int blockTapsSeen = 0;
    private bool tutorialDone;
    private bool startRequested = false;

    public event Action<TutorialStep> OnStepChanged;
    public bool IsActive => !tutorialDone && currentStep != TutorialStep.None && currentStep != TutorialStep.Done;
    public TutorialStep CurrentStep => currentStep;

    private void Awake()
    {
        Instance = this;
        tutorialDone = PlayerPrefs.GetInt(TUTORIAL_DONE_KEY, 0) == 1;
    }

    private void Start()
    {
        if (tutorialDone) return;
        TryStartIfReady();
    }

    private void OnEnable()
    {
        if (PickaxeGridManager.Instance != null)
        {
            PickaxeGridManager.Instance.OnMerged -= OnMerged;
            PickaxeGridManager.Instance.OnMerged += OnMerged;
        }
    }

    private void OnDisable()
    {
        if (PickaxeGridManager.Instance != null) PickaxeGridManager.Instance.OnMerged -= OnMerged;
    }

    public void NotifyOtherFlowEnded()
    {
        if (tutorialDone) return;
        if (startRequested) return;
        DG.Tweening.DOVirtual.DelayedCall(0.5f, () => TryStartIfReady(), true);
    }

    private void TryStartIfReady()
    {
        if (tutorialDone) return;
        if (startRequested) return;
        if (IsBlocked()) return;
        startRequested = true;
        GoToStep(TutorialStep.TapShop);
    }

    private bool IsBlocked()
    {
        if (DailyRewardManager.Instance != null && DailyRewardManager.Instance.CanClaimToday()) return true;
        return false;
    }

    public void NotifyShopPurchase()
    {
        if (currentStep == TutorialStep.TapShop)
        {
            GoToStep(TutorialStep.DragMerge);
        }
    }

    private void OnMerged(int newLevel)
    {
        if (currentStep == TutorialStep.DragMerge)
        {
            GoToStep(TutorialStep.TapBlock);
        }
    }

    public void NotifyBlockTapped()
    {
        if (currentStep == TutorialStep.TapBlock)
        {
            blockTapsSeen++;
            if (blockTapsSeen >= 3)
            {
                CompleteTutorial();
            }
        }
    }

    private void GoToStep(TutorialStep step)
    {
        currentStep = step;
        if (overlay != null) overlay.ShowStep(step, GetTargetForStep(step));
        OnStepChanged?.Invoke(step);
    }

    private RectTransform GetTargetForStep(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.TapShop: return shopButtonRef;
            case TutorialStep.DragMerge: return pickaxeGridRef;
            case TutorialStep.TapBlock: return blocksRowRef;
            default: return null;
        }
    }

    private void CompleteTutorial()
    {
        currentStep = TutorialStep.Done;
        tutorialDone = true;
        PlayerPrefs.SetInt(TUTORIAL_DONE_KEY, 1);
        if (overlay != null) overlay.HideAll();
        OnStepChanged?.Invoke(TutorialStep.Done);
    }

    public void SkipTutorial()
    {
        CompleteTutorial();
    }
}

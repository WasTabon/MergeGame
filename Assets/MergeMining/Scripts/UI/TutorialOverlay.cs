using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image dimmer;
    [SerializeField] private RectTransform highlightRing;
    [SerializeField] private RectTransform handPointer;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private Button skipButton;

    private Tweener handTween;
    private Tweener ringTween;
    private RectTransform myRoot;

    private void Awake()
    {
        myRoot = transform as RectTransform;
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        gameObject.SetActive(false);
        if (skipButton != null) skipButton.onClick.AddListener(OnSkip);
    }

    public void ShowStep(TutorialStep step, RectTransform target)
    {
        gameObject.SetActive(true);
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
        }

        string text = GetInstructionText(step);
        if (instructionText != null) instructionText.text = text;

        if (target == null)
        {
            HideAll();
            return;
        }

        Vector2 anchoredPos;
        Vector2 size;
        ConvertTargetToOverlaySpace(target, out anchoredPos, out size);

        float ringWidth = Mathf.Clamp(size.x + 40f, 180f, 700f);
        float ringHeight = Mathf.Clamp(size.y + 40f, 120f, 380f);

        highlightRing.anchoredPosition = anchoredPos;
        highlightRing.sizeDelta = new Vector2(ringWidth, ringHeight);
        PositionInstruction(step, anchoredPos, size);

        ringTween?.Kill();
        highlightRing.localScale = Vector3.one;
        ringTween = highlightRing.DOScale(1.08f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine).SetUpdate(true);

        handPointer.gameObject.SetActive(true);
        handTween?.Kill();
        AnimateHand(step, anchoredPos, size);
    }

    private void ConvertTargetToOverlaySpace(RectTransform target, out Vector2 anchoredPos, out Vector2 size)
    {
        Vector3[] corners = new Vector3[4];
        target.GetWorldCorners(corners);
        Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;

        Canvas overlayCanvas = myRoot.GetComponentInParent<Canvas>();
        Camera cam = overlayCanvas != null && overlayCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : overlayCanvas.worldCamera;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldCenter);
        Vector2 screenBL = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
        Vector2 screenTR = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(myRoot, screenPoint, cam, out anchoredPos);

        float w = Mathf.Abs(screenTR.x - screenBL.x);
        float h = Mathf.Abs(screenTR.y - screenBL.y);

        float scaleFactor = overlayCanvas != null ? overlayCanvas.scaleFactor : 1f;
        if (scaleFactor > 0.001f)
        {
            w /= scaleFactor;
            h /= scaleFactor;
        }
        size = new Vector2(w, h);
    }

    private void PositionInstruction(TutorialStep step, Vector2 ringPos, Vector2 ringSize)
    {
        if (instructionText == null) return;
        RectTransform irt = instructionText.rectTransform;

        float halfH = ringSize.y * 0.5f;
        Vector2 pos = ringPos;
        if (step == TutorialStep.TapShop)
        {
            pos.y += halfH + 140f;
        }
        else if (step == TutorialStep.DragMerge)
        {
            pos.y += halfH + 140f;
        }
        else
        {
            pos.y -= halfH + 140f;
        }
        irt.anchoredPosition = pos;
    }

    private void AnimateHand(TutorialStep step, Vector2 ringPos, Vector2 ringSize)
    {
        handPointer.sizeDelta = new Vector2(70f, 70f);

        if (step == TutorialStep.DragMerge)
        {
            float halfX = Mathf.Min(ringSize.x * 0.25f, 150f);
            Vector2 dragStart = ringPos + new Vector2(-halfX, 0f);
            Vector2 dragEnd = ringPos + new Vector2(halfX, 0f);
            handPointer.anchoredPosition = dragStart;
            handTween = handPointer.DOAnchorPos(dragEnd, 1.1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
        else
        {
            handPointer.anchoredPosition = ringPos;
            handTween = handPointer.DOScale(0.85f, 0.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
    }

    public void HideAll()
    {
        ringTween?.Kill();
        handTween?.Kill();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0f, 0.3f).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private string GetInstructionText(TutorialStep step)
    {
        switch (step)
        {
            case TutorialStep.TapShop: return "TAP TO BUY A PICKAXE";
            case TutorialStep.DragMerge: return "DRAG TWO PICKAXES TO MERGE";
            case TutorialStep.TapBlock: return "TAP THE BLOCK TO MINE";
            default: return "";
        }
    }

    private void OnSkip()
    {
        if (TutorialManager.Instance != null) TutorialManager.Instance.SkipTutorial();
    }
}

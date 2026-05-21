using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class AchievementToast : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform content;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private float showDuration = 3f;

    private Queue<AchievementDefinition> queue = new Queue<AchievementDefinition>();
    private bool isShowing;

    private void Awake()
    {
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void Show(AchievementDefinition a)
    {
        queue.Enqueue(a);
        if (!isShowing) ShowNext();
    }

    private void ShowNext()
    {
        if (queue.Count == 0) { isShowing = false; return; }
        isShowing = true;
        AchievementDefinition a = queue.Dequeue();

        gameObject.SetActive(true);
        if (titleText != null) titleText.text = "UNLOCKED: " + a.title.ToUpper();
        if (descriptionText != null) descriptionText.text = a.description;
        if (rewardText != null) rewardText.text = "+" + a.gemsReward;

        if (content != null)
        {
            content.anchoredPosition = new Vector2(0f, 200f);
            content.DOAnchorPosY(0f, 0.45f).SetEase(Ease.OutBack).SetUpdate(true);
        }
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
        }

        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.achievementUnlock);

        DOVirtual.DelayedCall(showDuration, FadeOut, false);
    }

    private void FadeOut()
    {
        if (canvasGroup != null) canvasGroup.DOFade(0f, 0.4f).SetUpdate(true);
        if (content != null)
        {
            content.DOAnchorPosY(200f, 0.4f).SetEase(Ease.InQuad).SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    ShowNext();
                });
        }
    }
}

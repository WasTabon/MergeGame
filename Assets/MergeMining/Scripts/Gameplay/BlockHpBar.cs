using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlockHpBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image bgImage;

    private Tweener fillTween;

    public void SetImmediate(float ratio)
    {
        fillTween?.Kill();
        ratio = Mathf.Clamp01(ratio);
        fillImage.fillAmount = ratio;
        UpdateColor(ratio);
    }

    public void AnimateTo(float ratio, float duration = 0.2f)
    {
        fillTween?.Kill();
        ratio = Mathf.Clamp01(ratio);
        fillTween = fillImage.DOFillAmount(ratio, duration).SetEase(Ease.OutQuad)
            .OnUpdate(() => UpdateColor(fillImage.fillAmount));
    }

    private void UpdateColor(float ratio)
    {
        Color c;
        if (ratio > 0.6f) c = new Color(0.4f, 0.85f, 0.4f);
        else if (ratio > 0.3f) c = new Color(0.95f, 0.78f, 0.25f);
        else c = new Color(0.95f, 0.35f, 0.35f);
        fillImage.color = c;
    }
}

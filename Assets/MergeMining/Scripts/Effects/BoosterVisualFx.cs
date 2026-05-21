using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoosterVisualFx : MonoBehaviour
{
    [SerializeField] private Image edgeGlowImage;

    private Tweener pulseTween;

    private void OnEnable()
    {
        if (BoosterManager.Instance != null)
        {
            BoosterManager.Instance.OnBoosterStarted -= OnStarted;
            BoosterManager.Instance.OnBoosterStarted += OnStarted;
            BoosterManager.Instance.OnBoosterEnded -= OnEnded;
            BoosterManager.Instance.OnBoosterEnded += OnEnded;
        }
        if (edgeGlowImage != null)
        {
            Color c = edgeGlowImage.color; c.a = 0f; edgeGlowImage.color = c;
        }
    }

    private void OnDisable()
    {
        if (BoosterManager.Instance != null)
        {
            BoosterManager.Instance.OnBoosterStarted -= OnStarted;
            BoosterManager.Instance.OnBoosterEnded -= OnEnded;
        }
    }

    private void OnStarted(BoosterType type, float duration, float remaining)
    {
        if (type != BoosterType.SpeedX2 && type != BoosterType.RewardsX2) return;
        if (edgeGlowImage == null) return;

        Color c = type == BoosterType.SpeedX2 ? new Color(0.4f, 0.85f, 0.95f) : new Color(1f, 0.85f, 0.4f);
        c.a = 0.3f;
        edgeGlowImage.color = c;

        pulseTween?.Kill();
        pulseTween = edgeGlowImage.DOFade(0.6f, 0.7f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    private void OnEnded(BoosterType type)
    {
        if (type != BoosterType.SpeedX2 && type != BoosterType.RewardsX2) return;
        if (BoosterManager.Instance != null &&
            (BoosterManager.Instance.IsActive(BoosterType.SpeedX2) || BoosterManager.Instance.IsActive(BoosterType.RewardsX2))) return;
        if (edgeGlowImage == null) return;

        pulseTween?.Kill();
        edgeGlowImage.DOFade(0f, 0.4f);
    }
}

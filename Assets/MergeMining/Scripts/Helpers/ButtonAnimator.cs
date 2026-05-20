using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ButtonAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private float pressedScale = 0.95f;
    [SerializeField] private float animDuration = 0.1f;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private bool playHaptic = true;

    private Vector3 originalScale;
    private Button button;
    private Tweener currentTween;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
        button.onClick.AddListener(OnClick);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * pressedScale, animDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, animDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, animDuration).SetEase(Ease.OutQuad);
    }

    private void OnClick()
    {
        if (SoundManager.Instance != null && clickClip != null)
        {
            SoundManager.Instance.PlaySFX(clickClip);
        }
        if (playHaptic && HapticManager.Instance != null)
        {
            HapticManager.Instance.Light();
        }
    }
}

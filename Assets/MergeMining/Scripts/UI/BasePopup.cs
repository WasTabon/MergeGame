using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BasePopup : MonoBehaviour
{
    [SerializeField] protected RectTransform content;
    [SerializeField] protected Image backdrop;
    [SerializeField] protected Button backdropButton;
    [SerializeField] protected bool closeOnBackdrop = true;
    [SerializeField] protected bool pausesGame = false;
    [SerializeField] protected float animDuration = 0.3f;
    [SerializeField] protected float backdropAlpha = 0.6f;

    protected virtual void Awake()
    {
        if (backdropButton != null && closeOnBackdrop)
        {
            backdropButton.onClick.AddListener(Hide);
        }
        gameObject.SetActive(false);
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (PopupManager.Instance != null) PopupManager.Instance.Register(this);

        if (pausesGame)
        {
            Time.timeScale = 0f;
        }

        content.localScale = Vector3.zero;
        Color c = backdrop.color;
        c.a = 0f;
        backdrop.color = c;

        backdrop.DOFade(backdropAlpha, animDuration).SetEase(Ease.OutQuad).SetUpdate(true);
        content.DOScale(1f, animDuration).SetEase(Ease.OutBack).SetUpdate(true);

        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.popupOpen, 0.6f);

        OnShow();
    }

    public virtual void Hide()
    {
        OnHide();

        if (PopupManager.Instance != null) PopupManager.Instance.Unregister(this);

        if (pausesGame)
        {
            Time.timeScale = 1f;
        }

        backdrop.DOFade(0f, animDuration).SetEase(Ease.InQuad).SetUpdate(true);
        content.DOScale(0f, animDuration).SetEase(Ease.InBack).SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));

        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.popupClose, 0.5f);
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}

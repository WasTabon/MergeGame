using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BasePopup : MonoBehaviour
{
    [SerializeField] protected RectTransform content;
    [SerializeField] protected Image backdrop;
    [SerializeField] protected Button backdropButton;
    [SerializeField] protected bool closeOnBackdrop = true;
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
        PopupManager.Instance.Register(this);

        content.localScale = Vector3.zero;
        Color c = backdrop.color;
        c.a = 0f;
        backdrop.color = c;

        backdrop.DOFade(backdropAlpha, animDuration).SetEase(Ease.OutQuad);
        content.DOScale(1f, animDuration).SetEase(Ease.OutBack);

        OnShow();
    }

    public virtual void Hide()
    {
        OnHide();

        PopupManager.Instance.Unregister(this);

        backdrop.DOFade(0f, animDuration).SetEase(Ease.InQuad);
        content.DOScale(0f, animDuration).SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
}

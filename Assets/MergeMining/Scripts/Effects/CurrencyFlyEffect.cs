using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum CurrencyType { Coins, Gems }

public class CurrencyFlyEffect : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private float duration = 0.6f;

    public RectTransform RectTransform => transform as RectTransform;

    public void Launch(Vector3 fromWorldPos, RectTransform toTarget, CurrencyType type, int amount)
    {
        if (toTarget == null) { Destroy(gameObject); return; }

        Color c = type == CurrencyType.Coins ? new Color(1f, 0.85f, 0.4f) : new Color(0.4f, 0.85f, 0.9f);
        if (iconImage != null) iconImage.color = c;

        RectTransform rt = RectTransform;
        rt.position = fromWorldPos;
        rt.localScale = Vector3.one * 0.5f;

        Vector3 mid = (fromWorldPos + toTarget.position) * 0.5f;
        mid.y += 100f;
        mid.x += Random.Range(-80f, 80f);

        Sequence s = DOTween.Sequence();
        s.Append(rt.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
        s.Append(rt.DOPath(new Vector3[] { mid, toTarget.position }, duration, PathType.CatmullRom).SetEase(Ease.InQuad));
        s.Join(rt.DOScale(0.7f, duration).SetEase(Ease.InQuad).SetDelay(0.1f));
        s.OnComplete(() =>
        {
            if (CurrencyManager.Instance != null)
            {
                if (type == CurrencyType.Coins) CurrencyManager.Instance.AddCoins(amount);
                else CurrencyManager.Instance.AddGems(amount);
            }
            if (HapticManager.Instance != null) HapticManager.Instance.Light();
            Destroy(gameObject);
        });
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Pickaxe : MonoBehaviour
{
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image levelBadge;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image glowImage;

    public int Level { get; private set; }
    public PickaxeSlot CurrentSlot { get; set; }
    public RectTransform RectTransform => transform as RectTransform;

    public void SetLevel(int level)
    {
        Level = level;
        ApplyVisuals();
    }

    private void ApplyVisuals()
    {
        PickaxeLevelData data = PickaxeConfigProvider.Config.GetLevel(Level);
        if (data == null) return;

        bodyImage.color = data.color;
        levelText.text = Level.ToString();

        if (glowImage != null)
        {
            Color glow = data.color;
            glow.a = 0.5f;
            glowImage.color = glow;
        }
    }

    public void PlaySpawnAnimation()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack);
    }

    public void PlayLevelUpAnimation()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one * 0.4f, 0.4f, 6, 0.5f);

        if (glowImage != null)
        {
            glowImage.transform.DOKill();
            glowImage.transform.localScale = Vector3.one;
            glowImage.DOFade(1f, 0.1f).OnComplete(() => glowImage.DOFade(0.3f, 0.4f));
            glowImage.transform.DOScale(1.8f, 0.4f).SetEase(Ease.OutQuad);
        }
    }
}

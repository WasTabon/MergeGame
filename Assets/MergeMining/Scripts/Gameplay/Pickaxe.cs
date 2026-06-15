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
    [SerializeField] private Image durabilityFill;

    public int Level { get; private set; }
    public PickaxeSlot CurrentSlot { get; set; }
    public RectTransform RectTransform => transform as RectTransform;
    public int RemainingDurability { get; private set; }
    public int MaxDurability { get; private set; }

    public void SetLevel(int level)
    {
        Level = level;
        ApplyVisuals();
        PickaxeLevelData data = PickaxeConfigProvider.Config.GetLevel(Level);
        if (data != null)
        {
            float durMult = 1f;
            if (LevelManager.Instance != null && LevelManager.Instance.ActiveModifier != null)
            {
                durMult = LevelManager.Instance.ActiveModifier.DurabilityMultiplier;
            }
            MaxDurability = Mathf.Max(1, Mathf.RoundToInt(data.durability * durMult));
            RemainingDurability = MaxDurability;
        }
        UpdateDurabilityBar();
    }

    public void ConsumeDurability()
    {
        if (RemainingDurability <= 0) return;
        RemainingDurability--;
        UpdateDurabilityBar();
        if (RemainingDurability <= 0)
        {
            BreakPickaxe();
        }
    }

    private void UpdateDurabilityBar()
    {
        if (durabilityFill == null) return;
        float ratio = MaxDurability > 0 ? (float)RemainingDurability / MaxDurability : 0f;
        durabilityFill.fillAmount = ratio;
        if (ratio < 0.3f) durabilityFill.color = new Color(0.95f, 0.3f, 0.3f);
        else if (ratio < 0.6f) durabilityFill.color = new Color(0.95f, 0.78f, 0.25f);
        else durabilityFill.color = new Color(0.4f, 0.78f, 0.4f);
    }

    private void BreakPickaxe()
    {
        PickaxeSlot slot = CurrentSlot;
        if (slot != null) slot.Clear();
        transform.DOKill();
        transform.DOScale(0f, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Destroy(gameObject);
                if (PickaxeGridManager.Instance != null) PickaxeGridManager.Instance.NotifyPickaxeBroken();
            });
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.blockExplode, 0.4f, 1.4f);
    }

    private void ApplyVisuals()
    {
        PickaxeLevelData data = PickaxeConfigProvider.Config.GetLevel(Level);
        if (data == null) return;

        if (data.spriteOverride != null)
        {
            bodyImage.sprite = data.spriteOverride;
            bodyImage.color = Color.white;
            bodyImage.preserveAspect = true;
        }
        else
        {
            bodyImage.color = data.color;
        }
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

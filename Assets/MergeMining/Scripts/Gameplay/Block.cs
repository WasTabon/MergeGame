using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class Block : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image bodyImage;
    [SerializeField] private Image highlightImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private BlockHpBar hpBar;

    public BlockTypeData TypeData { get; private set; }
    public float CurrentHP { get; private set; }
    public float MaxHP { get; private set; }
    public int RewardCoins { get; private set; }
    public bool IsAlive => CurrentHP > 0f && !isDying;
    public RectTransform RectTransform => transform as RectTransform;

    public event Action<Block> OnDestroyed;

    private bool isDying = false;
    private Vector3 originalScale;
    private float lastDamageTime;
    private float regenDelay = 1.5f;
    private float regenPerSec = 0f;
    private float descendSpeed = 0f;
    private float dangerY = float.NegativeInfinity;
    private bool reachedBottom = false;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetRegen(float delay, float perSec)
    {
        regenDelay = delay;
        regenPerSec = perSec;
    }

    public void SetDescend(float speed, float dangerWorldY)
    {
        descendSpeed = speed;
        dangerY = dangerWorldY;
    }

    private float lastHealTime;

    private void Update()
    {
        if (!IsAlive) return;
        if (LevelManager.Instance != null && LevelManager.Instance.Phase != LevelPhase.Battle) return;

        if (descendSpeed > 0f && !reachedBottom)
        {
            RectTransform rt = transform as RectTransform;
            Vector3 pos = rt.position;
            pos.y -= descendSpeed * Time.deltaTime;
            rt.position = pos;
            if (rt.position.y <= dangerY)
            {
                reachedBottom = true;
                if (LevelManager.Instance != null) LevelManager.Instance.NotifyBlockReachedBottom();
            }
        }

        if (regenPerSec > 0f && CurrentHP < MaxHP)
        {
            if (Time.time - lastDamageTime >= regenDelay)
            {
                CurrentHP += MaxHP * regenPerSec * Time.deltaTime;
                if (CurrentHP > MaxHP) CurrentHP = MaxHP;
                if (hpBar != null) hpBar.AnimateTo(CurrentHP / MaxHP, 0.1f);
            }
        }

        if (TypeData != null && TypeData.behavior == BlockBehavior.Healer && TypeData.healPerSecond > 0f)
        {
            if (Time.time - lastHealTime >= TypeData.healInterval)
            {
                lastHealTime = Time.time;
                HealNeighbors();
            }
        }
    }

    private void HealNeighbors()
    {
        if (BlocksRowManager.Instance == null) return;
        float healAmount = TypeData.healPerSecond * TypeData.healInterval;
        foreach (var b in BlocksRowManager.Instance.ActiveBlocks)
        {
            if (b == null || b == this || !b.IsAlive) continue;
            float dist = Vector3.Distance(b.transform.position, transform.position);
            if (dist > 250f) continue;
            b.HealedBy(this, healAmount);
        }
        ShowHealPulse();
    }

    public void HealedBy(Block source, float amount)
    {
        if (!IsAlive) return;
        if (CurrentHP >= MaxHP) return;
        CurrentHP = Mathf.Min(MaxHP, CurrentHP + amount);
        if (hpBar != null) hpBar.AnimateTo(CurrentHP / MaxHP, 0.2f);
        ShowHealFlash();
    }

    private void ShowHealPulse()
    {
        if (bodyImage == null) return;
        Color orig = TypeData.color;
        bodyImage.DOKill();
        bodyImage.color = new Color(0.5f, 1f, 0.5f);
        bodyImage.DOColor(orig, 0.4f);
    }

    private void ShowHealFlash()
    {
        if (bodyImage == null) return;
        bodyImage.DOKill();
        Color orig = TypeData != null ? TypeData.color : Color.white;
        bodyImage.color = new Color(0.7f, 1f, 0.7f);
        bodyImage.DOColor(orig, 0.3f);
    }

    public void Setup(BlockTypeData type, float maxHP, int reward)
    {
        TypeData = type;
        MaxHP = maxHP;
        CurrentHP = maxHP;
        RewardCoins = reward;
        isDying = false;

        bodyImage.color = type.color;
        if (highlightImage != null) highlightImage.color = new Color(1f, 1f, 1f, 0.15f);
        if (nameText != null) nameText.text = type.displayName.ToUpper();

        if (hpBar != null) hpBar.SetImmediate(1f);

        transform.localScale = Vector3.zero;
        transform.DOScale(originalScale, 0.35f).SetEase(Ease.OutBack);
    }

    public void TakeDamage(float dmg)
    {
        TakeDamage(dmg, 0);
    }

    public void TakeDamage(float dmg, int sourcePickaxeLevel)
    {
        if (!IsAlive) return;

        if (TypeData != null && TypeData.behavior == BlockBehavior.Iron && sourcePickaxeLevel < TypeData.minPickaxeLevel)
        {
            ShowImmuneFlash();
            return;
        }

        float finalDmg = dmg;
        if (TypeData != null) finalDmg *= TypeData.damageMultiplier;

        CurrentHP -= finalDmg;
        if (CurrentHP < 0f) CurrentHP = 0f;
        lastDamageTime = Time.time;

        if (hpBar != null) hpBar.AnimateTo(CurrentHP / MaxHP, 0.2f);

        transform.DOKill(true);
        transform.localScale = originalScale;
        transform.DOPunchScale(originalScale * 0.08f, 0.18f, 6, 0.5f);

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.DOKill();
            cam.transform.DOShakePosition(0.1f, 0.06f, 8, 90f);
        }

        if (CurrentHP <= 0f) Die();
    }

    private void ShowImmuneFlash()
    {
        if (bodyImage == null) return;
        bodyImage.DOKill();
        Color orig = TypeData.color;
        bodyImage.color = Color.white;
        bodyImage.DOColor(orig, 0.25f);
        transform.DOKill(true);
        transform.localScale = originalScale;
        transform.DOPunchPosition(new Vector3(8f, 0f, 0f), 0.18f, 8, 0.5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsAlive) return;
        if (PopupManager.Instance != null && PopupManager.Instance.IsAnyPopupOpen) return;

        TakeDamage(1f);

        if (HapticManager.Instance != null) HapticManager.Instance.Light();
        if (TutorialManager.Instance != null) TutorialManager.Instance.NotifyBlockTapped();

        transform.DOKill(true);
        transform.localScale = originalScale;
        transform.DOPunchScale(originalScale * 0.12f, 0.15f, 6, 0.5f);
    }

    private void Die()
    {
        if (isDying) return;
        isDying = true;

        if (TypeData != null && TypeData.behavior == BlockBehavior.Explosive)
        {
            TriggerExplosion();
        }

        OnDestroyed?.Invoke(this);

        transform.DOKill();
        transform.DOScale(0f, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }

    private void TriggerExplosion()
    {
        if (PickaxeGridManager.Instance == null) return;
        Vector3 myPos = transform.position;
        float radius = 400f;

        var pickaxes = UnityEngine.Object.FindObjectsOfType<Pickaxe>();
        int hitCount = 0;
        foreach (var p in pickaxes)
        {
            if (p == null) continue;
            float dist = Vector3.Distance(p.transform.position, myPos);
            if (dist > radius) continue;
            int dmg = Mathf.RoundToInt(TypeData.explosionDamage);
            for (int i = 0; i < dmg; i++) p.ConsumeDurability();
            if (hitCount++ >= 3) break;
        }

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.DOKill();
            cam.transform.DOShakePosition(0.4f, 0.5f, 12, 90f);
        }
        if (HapticManager.Instance != null) HapticManager.Instance.Heavy();
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.blockExplode, 1.4f, 0.7f);
    }
}

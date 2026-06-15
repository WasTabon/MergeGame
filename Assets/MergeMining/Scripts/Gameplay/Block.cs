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
    private Vector3 currentScale;
    private float lastDamageTime;
    private float regenDelay = 1.5f;
    private float regenPerSec = 0f;
    private float descendSpeed = 0f;
    private float dangerY = float.NegativeInfinity;
    private bool reachedBottom = false;
    private bool shellActive = false;
    private float shellHP = 0f;
    private float shellMaxHP = 0f;
    private float lastBossAttackTime = 0f;

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

        bool didShellRegen = false;
        if (TypeData != null && TypeData.innerHPMultiplier > 0f && !shellActive && CurrentHP < MaxHP)
        {
            if (Time.time - lastDamageTime >= 0.6f)
            {
                CurrentHP += MaxHP * TypeData.shellRegenPerSec * Time.deltaTime;
                if (CurrentHP > MaxHP) CurrentHP = MaxHP;
                if (hpBar != null) hpBar.AnimateTo(CurrentHP / MaxHP, 0.1f);
                didShellRegen = true;
            }
        }

        if (!didShellRegen && regenPerSec > 0f && CurrentHP < MaxHP && !shellActive)
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

        if (TypeData != null && TypeData.isBoss && TypeData.bossCounterAttackInterval > 0f)
        {
            if (Time.time - lastBossAttackTime >= TypeData.bossCounterAttackInterval)
            {
                lastBossAttackTime = Time.time;
                BossCounterAttack();
            }
        }
    }

    private void BossCounterAttack()
    {
        if (PickaxeGridManager.Instance == null) return;
        var pickaxes = UnityEngine.Object.FindObjectsOfType<Pickaxe>();
        if (pickaxes == null || pickaxes.Length == 0) return;

        Pickaxe weakest = null;
        int lowestLevel = int.MaxValue;
        foreach (var p in pickaxes)
        {
            if (p == null) continue;
            if (p.Level < lowestLevel)
            {
                lowestLevel = p.Level;
                weakest = p;
            }
        }
        if (weakest == null) return;

        if (bodyImage != null)
        {
            Color orig = GetBodyTint();
            bodyImage.DOKill();
            bodyImage.color = Color.red;
            bodyImage.DOColor(orig, 0.5f);
        }
        transform.DOKill(true);
        transform.localScale = currentScale;
        transform.DOPunchScale(currentScale * 0.2f, 0.4f, 8, 0.5f);

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.DOKill();
            cam.transform.DOShakePosition(0.4f, 0.3f, 10, 90f);
        }
        if (HapticManager.Instance != null) HapticManager.Instance.Heavy();
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.blockExplode, 1.2f, 0.65f);

        Pickaxe slotPickaxe = weakest;
        slotPickaxe.transform.DOKill();
        slotPickaxe.transform.DOScale(0f, 0.35f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                if (slotPickaxe == null) return;
                if (slotPickaxe.CurrentSlot != null) slotPickaxe.CurrentSlot.Clear();
                Destroy(slotPickaxe.gameObject);
                if (PickaxeGridManager.Instance != null) PickaxeGridManager.Instance.NotifyPickaxeBroken();
            });
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
        Color orig = GetBodyTint();
        bodyImage.DOKill();
        bodyImage.color = new Color(0.5f, 1f, 0.5f);
        bodyImage.DOColor(orig, 0.4f);
    }

    private void ShowHealFlash()
    {
        if (bodyImage == null) return;
        bodyImage.DOKill();
        Color orig = GetBodyTint();
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

        shellActive = false;
        shellHP = 0f;
        shellMaxHP = 0f;
        if (type != null && type.innerHPMultiplier > 0f)
        {
            shellActive = true;
            shellMaxHP = maxHP;
            shellHP = maxHP;
            MaxHP = maxHP * type.innerHPMultiplier;
            CurrentHP = MaxHP;
        }

        if (type.spriteOverride != null)
        {
            bodyImage.sprite = type.spriteOverride;
            bodyImage.color = Color.white;
            bodyImage.preserveAspect = false;
        }
        else
        {
            bodyImage.color = type.color;
        }
        if (highlightImage != null) highlightImage.color = new Color(1f, 1f, 1f, 0.15f);
        if (nameText != null) nameText.text = type.displayName.ToUpper();

        if (hpBar != null) hpBar.SetImmediate(1f);

        currentScale = originalScale;
        transform.localScale = Vector3.zero;
        transform.DOScale(currentScale, 0.45f).SetEase(Ease.OutBack);

        lastBossAttackTime = Time.time;
    }

    private Color GetBodyTint()
    {
        if (TypeData == null) return Color.white;
        if (TypeData.spriteOverride != null) return Color.white;
        return TypeData.color;
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

        if (TypeData != null && TypeData.armor > 0f)
        {
            finalDmg = Mathf.Max(0.5f, finalDmg - TypeData.armor);
        }

        if (shellActive)
        {
            shellHP -= finalDmg;
            if (shellHP < 0f) shellHP = 0f;
            lastDamageTime = Time.time;
            if (hpBar != null) hpBar.AnimateTo(shellHP / shellMaxHP, 0.2f);
            DoDamageVfx();
            if (shellHP <= 0f) BreakShell();
            return;
        }

        CurrentHP -= finalDmg;
        if (CurrentHP < 0f) CurrentHP = 0f;
        lastDamageTime = Time.time;

        if (hpBar != null) hpBar.AnimateTo(CurrentHP / MaxHP, 0.2f);
        DoDamageVfx();

        if (CurrentHP <= 0f) Die();
    }

    private void DoDamageVfx()
    {
        transform.DOKill(true);
        transform.localScale = currentScale;
        transform.DOPunchScale(currentScale * 0.08f, 0.18f, 6, 0.5f);

        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.transform.DOKill();
            cam.transform.DOShakePosition(0.1f, 0.06f, 8, 90f);
        }
    }

    private void BreakShell()
    {
        shellActive = false;
        if (bodyImage != null)
        {
            bodyImage.DOKill();
            Color target = TypeData != null && TypeData.spriteOverride != null
                ? new Color(0.7f, 0.7f, 0.7f, 1f)
                : TypeData.darkColor;
            bodyImage.DOColor(target, 0.3f);
        }
        if (hpBar != null) hpBar.SetImmediate(CurrentHP / MaxHP);
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.blockExplode, 0.7f, 1.2f);
    }

    private void ShowImmuneFlash()
    {
        if (bodyImage == null) return;
        bodyImage.DOKill();
        Color orig = GetBodyTint();
        bodyImage.color = Color.white;
        bodyImage.DOColor(orig, 0.25f);
        transform.DOKill(true);
        transform.localScale = currentScale;
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
        transform.localScale = currentScale;
        transform.DOPunchScale(currentScale * 0.12f, 0.15f, 6, 0.5f);
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

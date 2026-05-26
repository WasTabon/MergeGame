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
        if (!IsAlive) return;

        CurrentHP -= dmg;
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
        OnDestroyed?.Invoke(this);

        transform.DOKill();
        transform.DOScale(0f, 0.25f).SetEase(Ease.InBack)
            .OnComplete(() => Destroy(gameObject));
    }
}

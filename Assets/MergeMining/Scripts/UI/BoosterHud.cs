using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BoosterHud : MonoBehaviour
{
    [SerializeField] private List<BoosterButtonView> buttons;

    private void Start()
    {
        foreach (var b in buttons)
        {
            BoosterType t = b.type;
            b.button.onClick.AddListener(() => OnBoosterClicked(t));
            UpdateBoosterCost(b);
            SetCooldownState(b, false, 0f, 0f);
        }
    }

    private void OnEnable()
    {
        if (BoosterManager.Instance != null)
        {
            BoosterManager.Instance.OnBoosterStarted -= OnBoosterStarted;
            BoosterManager.Instance.OnBoosterStarted += OnBoosterStarted;
            BoosterManager.Instance.OnBoosterEnded -= OnBoosterEnded;
            BoosterManager.Instance.OnBoosterEnded += OnBoosterEnded;
            BoosterManager.Instance.OnBoosterTick -= OnBoosterTick;
            BoosterManager.Instance.OnBoosterTick += OnBoosterTick;
        }
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnGemsChanged -= OnGemsChanged;
            CurrencyManager.Instance.OnGemsChanged += OnGemsChanged;
        }
    }

    private void OnDisable()
    {
        if (BoosterManager.Instance != null)
        {
            BoosterManager.Instance.OnBoosterStarted -= OnBoosterStarted;
            BoosterManager.Instance.OnBoosterEnded -= OnBoosterEnded;
            BoosterManager.Instance.OnBoosterTick -= OnBoosterTick;
        }
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.OnGemsChanged -= OnGemsChanged;
    }

    private void UpdateBoosterCost(BoosterButtonView b)
    {
        BoosterInfo info = BoosterConfigProvider.Config.Get(b.type);
        if (info == null) return;
        if (b.costText != null) b.costText.text = info.gemsCost.ToString();
        if (b.nameText != null) b.nameText.text = info.displayName.ToUpper();
        if (b.iconImage != null) b.iconImage.color = info.color;
        RefreshInteractable(b);
    }

    private void RefreshInteractable(BoosterButtonView b)
    {
        if (b.button == null || CurrencyManager.Instance == null) return;
        BoosterInfo info = BoosterConfigProvider.Config.Get(b.type);
        if (info == null) return;
        bool canAfford = CurrencyManager.Instance.CanAffordGems(info.gemsCost);
        b.button.interactable = canAfford;
    }

    private void OnBoosterClicked(BoosterType type)
    {
        if (BoosterManager.Instance == null) return;
        BoosterManager.Instance.TryActivate(type);
    }

    private BoosterButtonView FindButton(BoosterType type)
    {
        foreach (var b in buttons) if (b.type == type) return b;
        return null;
    }

    private void OnBoosterStarted(BoosterType type, float duration, float remaining)
    {
        BoosterButtonView b = FindButton(type);
        if (b == null) return;
        SetCooldownState(b, true, remaining, duration);
    }

    private void OnBoosterTick(BoosterType type, float remaining)
    {
        BoosterButtonView b = FindButton(type);
        if (b == null) return;
        BoosterInfo info = BoosterConfigProvider.Config.Get(type);
        if (b.cooldownOverlay != null) b.cooldownOverlay.fillAmount = remaining / info.duration;
        if (b.remainingText != null) b.remainingText.text = Mathf.CeilToInt(remaining) + "s";
    }

    private void OnBoosterEnded(BoosterType type)
    {
        BoosterButtonView b = FindButton(type);
        if (b == null) return;
        SetCooldownState(b, false, 0f, 0f);
        RefreshInteractable(b);
    }

    private void SetCooldownState(BoosterButtonView b, bool active, float remaining, float duration)
    {
        if (b.cooldownOverlay != null)
        {
            b.cooldownOverlay.gameObject.SetActive(active);
            if (active) b.cooldownOverlay.fillAmount = duration > 0f ? remaining / duration : 1f;
        }
        if (b.remainingText != null)
        {
            b.remainingText.gameObject.SetActive(active);
            if (active) b.remainingText.text = Mathf.CeilToInt(remaining) + "s";
        }
        if (b.costRow != null) b.costRow.gameObject.SetActive(!active);
    }

    private void OnGemsChanged(int v)
    {
        foreach (var b in buttons) RefreshInteractable(b);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopController : MonoBehaviour
{
    [SerializeField] private Button shopButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private RectTransform priceIcon;

    private int pickaxesBought = 0;

    private void OnEnable()
    {
        SubscribeToEvents();
        UpdatePriceLabel();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Start()
    {
        shopButton.onClick.AddListener(OnShopClicked);
        UpdatePriceLabel();
        RefreshInteractable();
    }

    private void SubscribeToEvents()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
            CurrencyManager.Instance.OnCoinsChanged += OnCoinsChanged;
        }
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnPhaseChanged -= OnPhaseChanged;
            LevelManager.Instance.OnPhaseChanged += OnPhaseChanged;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
        if (LevelManager.Instance != null) LevelManager.Instance.OnPhaseChanged -= OnPhaseChanged;
    }

    private void OnCoinsChanged(int v) { RefreshInteractable(); }
    private void OnPhaseChanged(LevelPhase phase) { RefreshInteractable(); }

    private int GetCurrentPrice()
    {
        if (LevelManager.Instance != null) return LevelManager.Instance.GetPickaxeBaseCost();
        return LevelConfigProvider.Config != null ? LevelConfigProvider.Config.pickaxeBaseCost : 10;
    }

    private void UpdatePriceLabel()
    {
        if (priceText != null) priceText.text = GetCurrentPrice().ToString();
    }

    private void RefreshInteractable()
    {
        bool canAfford = CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(GetCurrentPrice());
        bool hasSlot = PickaxeGridManager.Instance != null && PickaxeGridManager.Instance.HasFreeSlot();
        bool isSetup = LevelManager.Instance == null || LevelManager.Instance.Phase == LevelPhase.Setup;
        shopButton.interactable = canAfford && hasSlot && isSetup;
    }

    private void OnShopClicked()
    {
        int price = GetCurrentPrice();
        if (CurrencyManager.Instance == null || !CurrencyManager.Instance.SpendCoins(price)) return;
        if (PickaxeGridManager.Instance == null || !PickaxeGridManager.Instance.HasFreeSlot()) return;

        PickaxeGridManager.Instance.AddPickaxe(1);
        pickaxesBought++;
        UpdatePriceLabel();

        if (priceIcon != null)
        {
            priceIcon.DOKill();
            priceIcon.localScale = Vector3.one;
            priceIcon.DOPunchScale(Vector3.one * 0.3f, 0.3f, 6, 0.5f);
        }

        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.shopBuy);
        if (LevelManager.Instance != null) LevelManager.Instance.NotifyCoinsSpent(price);

        RefreshInteractable();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopController : MonoBehaviour
{
    [SerializeField] private Button shopButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private RectTransform priceIcon;
    [SerializeField] private int basePrice = 10;
    [SerializeField] private float priceMultiplier = 1.15f;

    private const string PICKAXES_BOUGHT_KEY = "pickaxes_bought";

    private int pickaxesBought = 0;

    private void Awake()
    {
        pickaxesBought = PlayerPrefs.GetInt(PICKAXES_BOUGHT_KEY, 0);
    }

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
    }

    private void UnsubscribeFromEvents()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= OnCoinsChanged;
        }
    }

    private void OnCoinsChanged(int v)
    {
        RefreshInteractable();
    }

    private int GetCurrentPrice()
    {
        return Mathf.RoundToInt(basePrice * Mathf.Pow(priceMultiplier, pickaxesBought));
    }

    private void UpdatePriceLabel()
    {
        if (priceText != null) priceText.text = GetCurrentPrice().ToString();
    }

    private void RefreshInteractable()
    {
        bool canAfford = CurrencyManager.Instance != null && CurrencyManager.Instance.CanAfford(GetCurrentPrice());
        bool hasSlot = PickaxeGridManager.Instance != null && PickaxeGridManager.Instance.HasFreeSlot();
        shopButton.interactable = canAfford && hasSlot;
    }

    private void OnShopClicked()
    {
        int price = GetCurrentPrice();
        if (CurrencyManager.Instance == null || !CurrencyManager.Instance.SpendCoins(price)) return;
        if (PickaxeGridManager.Instance == null || !PickaxeGridManager.Instance.HasFreeSlot()) return;

        int level = GetRandomPurchaseLevel();
        PickaxeGridManager.Instance.AddPickaxe(level);

        pickaxesBought++;
        PlayerPrefs.SetInt(PICKAXES_BOUGHT_KEY, pickaxesBought);
        UpdatePriceLabel();

        if (priceIcon != null)
        {
            priceIcon.DOKill();
            priceIcon.localScale = Vector3.one;
            priceIcon.DOPunchScale(Vector3.one * 0.3f, 0.3f, 6, 0.5f);
        }

        RefreshInteractable();
    }

    private int GetRandomPurchaseLevel()
    {
        int highest = PickaxeGridManager.Instance.HighestEverReached;
        int cap = Mathf.Max(1, highest - 3);
        int min = 1;
        return Random.Range(min, cap + 1);
    }
}

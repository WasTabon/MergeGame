using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ChestHud : MonoBehaviour
{
    [SerializeField] private Button freeChestButton;
    [SerializeField] private Image freeChestIcon;
    [SerializeField] private TextMeshProUGUI freeChestTimer;
    [SerializeField] private GameObject freeChestReadyBadge;

    [SerializeField] private Button blockChestButton;
    [SerializeField] private Image blockChestIcon;
    [SerializeField] private TextMeshProUGUI blockChestProgress;
    [SerializeField] private GameObject blockChestReadyBadge;

    [SerializeField] private Button premiumChestButton;
    [SerializeField] private TextMeshProUGUI premiumChestCost;

    private void Start()
    {
        if (freeChestButton != null) freeChestButton.onClick.AddListener(OnFreeChest);
        if (blockChestButton != null) blockChestButton.onClick.AddListener(OnBlockChest);
        if (premiumChestButton != null) premiumChestButton.onClick.AddListener(OnPremiumChest);

        if (premiumChestCost != null)
        {
            premiumChestCost.text = ChestConfigProvider.Config.premiumChestGemsCost.ToString();
        }
    }

    private void Update()
    {
        if (ChestManager.Instance == null) return;
        RefreshFreeChest();
        RefreshBlockChest();
        RefreshPremiumChest();
    }

    private void RefreshFreeChest()
    {
        bool ready = ChestManager.Instance.IsFreeChestReady;
        if (freeChestButton != null) freeChestButton.interactable = ready;
        if (freeChestReadyBadge != null && freeChestReadyBadge.activeSelf != ready) freeChestReadyBadge.SetActive(ready);
        if (freeChestTimer != null)
        {
            if (ready) freeChestTimer.text = "READY";
            else
            {
                float secs = ChestManager.Instance.FreeChestSecondsRemaining;
                int m = Mathf.FloorToInt(secs / 60f);
                int s = Mathf.FloorToInt(secs % 60f);
                freeChestTimer.text = string.Format("{0:00}:{1:00}", m, s);
            }
        }
    }

    private void RefreshBlockChest()
    {
        bool ready = ChestManager.Instance.HasPendingBlockChest;
        if (blockChestButton != null) blockChestButton.interactable = ready;
        if (blockChestReadyBadge != null && blockChestReadyBadge.activeSelf != ready) blockChestReadyBadge.SetActive(ready);
        if (blockChestProgress != null)
        {
            if (ready) blockChestProgress.text = "READY";
            else blockChestProgress.text = ChestManager.Instance.BlocksSinceChest + "/" + ChestManager.Instance.BlocksRequiredForChest;
        }
    }

    private void RefreshPremiumChest()
    {
        if (CurrencyManager.Instance == null) return;
        int cost = ChestConfigProvider.Config.premiumChestGemsCost;
        if (premiumChestButton != null) premiumChestButton.interactable = CurrencyManager.Instance.CanAffordGems(cost);
    }

    private void OnFreeChest()
    {
        ChestManager.Instance.TryOpenFreeChest();
    }

    private void OnBlockChest()
    {
        ChestManager.Instance.TryOpenBlockChest();
    }

    private void OnPremiumChest()
    {
        ChestManager.Instance.TryOpenPremiumChest();
    }
}

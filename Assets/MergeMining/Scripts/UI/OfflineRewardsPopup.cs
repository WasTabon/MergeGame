using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class OfflineRewardsPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI timeAwayText;
    [SerializeField] private TextMeshProUGUI coinsAmountText;
    [SerializeField] private RectTransform coinsIcon;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button claimDoubleButton;

    private int pendingCoins;

    protected override void Awake()
    {
        base.Awake();
        claimButton.onClick.AddListener(OnClaim);
        if (claimDoubleButton != null) claimDoubleButton.onClick.AddListener(OnClaimDouble);
    }

    public void ShowReward(float seconds, int coins)
    {
        pendingCoins = coins;
        if (timeAwayText != null) timeAwayText.text = "AWAY FOR " + FormatTime(seconds);
        if (coinsAmountText != null) coinsAmountText.text = "+" + coins;
        Show();
    }

    protected override void OnShow()
    {
        if (coinsIcon != null)
        {
            coinsIcon.localScale = Vector3.zero;
            coinsIcon.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f).SetUpdate(true);
            coinsIcon.DOLocalRotate(new Vector3(0f, 360f, 0f), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart).SetUpdate(true).SetEase(Ease.Linear);
        }
    }

    private string FormatTime(float seconds)
    {
        int h = Mathf.FloorToInt(seconds / 3600f);
        int m = Mathf.FloorToInt((seconds % 3600f) / 60f);
        if (h > 0) return h + "h " + m + "m";
        if (m > 0) return m + "m";
        return Mathf.FloorToInt(seconds) + "s";
    }

    private void OnClaim()
    {
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddCoins(pendingCoins);
        if (coinsIcon != null) coinsIcon.DOKill();
        Hide();
    }

    private void OnClaimDouble()
    {
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddCoins(pendingCoins * 2);
        if (coinsIcon != null) coinsIcon.DOKill();
        Hide();
    }
}

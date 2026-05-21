using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DailyRewardPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private RectTransform rewardIcon;
    [SerializeField] private Image rewardIconImage;
    [SerializeField] private TextMeshProUGUI rewardAmountText;
    [SerializeField] private Button claimButton;

    private int day;
    private DailyRewardEntry currentEntry;

    protected override void Awake()
    {
        base.Awake();
        claimButton.onClick.AddListener(OnClaim);
    }

    public void ShowForDay(int dayNum)
    {
        day = dayNum;
        currentEntry = DailyRewardConfigProvider.Config.GetForDay(day);
        if (currentEntry == null) return;

        if (titleText != null) titleText.text = "DAILY REWARD";
        if (dayText != null) dayText.text = "DAY " + day;

        Color iconColor;
        string amountStr;
        if (currentEntry.kind == DailyRewardKind.Coins)
        {
            iconColor = new Color(1f, 0.85f, 0.4f);
            amountStr = "+" + currentEntry.amount;
        }
        else if (currentEntry.kind == DailyRewardKind.Gems)
        {
            iconColor = UIColors.GEM;
            amountStr = "+" + currentEntry.amount;
        }
        else
        {
            iconColor = new Color(0.9f, 0.6f, 1f);
            amountStr = "PICKAXE LV" + currentEntry.pickaxeLevel;
        }

        if (rewardIconImage != null) rewardIconImage.color = iconColor;
        if (rewardAmountText != null) rewardAmountText.text = amountStr;

        Show();
    }

    protected override void OnShow()
    {
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.dailyReward);

        if (rewardIcon != null)
        {
            rewardIcon.localScale = Vector3.zero;
            rewardIcon.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f).SetUpdate(true);
            rewardIcon.DOLocalRotate(new Vector3(0f, 360f, 0f), 2f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart).SetUpdate(true).SetEase(Ease.Linear);
        }
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    private void OnClaim()
    {
        if (rewardIcon != null) rewardIcon.DOKill();

        if (DailyRewardManager.Instance != null) DailyRewardManager.Instance.ClaimToday();

        Hide();
    }
}

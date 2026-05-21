using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ZoneCompletePopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private RectTransform gemIcon;
    [SerializeField] private Button continueButton;

    protected override void Awake()
    {
        base.Awake();
        continueButton.onClick.AddListener(OnContinue);
    }

    public void ShowReward(int gemsReward)
    {
        rewardText.text = "+" + gemsReward;
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddGems(gemsReward);
        Show();
    }

    protected override void OnShow()
    {
        if (gemIcon != null)
        {
            gemIcon.localScale = Vector3.zero;
            gemIcon.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f).SetUpdate(true);
            gemIcon.DOLocalRotate(new Vector3(0f, 360f, 0f), 1.5f, RotateMode.FastBeyond360)
                .SetLoops(-1, LoopType.Restart).SetUpdate(true).SetEase(Ease.Linear);
        }
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    private void OnContinue()
    {
        if (gemIcon != null) gemIcon.DOKill();
        Hide();
    }
}

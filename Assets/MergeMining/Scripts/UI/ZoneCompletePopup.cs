using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ZoneCompletePopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI completedZoneNameText;
    [SerializeField] private TextMeshProUGUI nextZoneNameText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private RectTransform gemIcon;
    [SerializeField] private Button continueButton;

    private int pendingGems;
    private bool gemsAlreadyAdded;

    protected override void Awake()
    {
        base.Awake();
        continueButton.onClick.AddListener(OnContinue);
    }

    public void ShowReward(int gemsReward)
    {
        ShowZoneCompleted("Zone", gemsReward, "Next");
    }

    public void ShowZoneCompleted(string completedName, int gemsReward, string nextName)
    {
        if (completedZoneNameText != null) completedZoneNameText.text = completedName.ToUpper() + " COMPLETED";
        if (nextZoneNameText != null) nextZoneNameText.text = "NEXT: " + nextName.ToUpper();
        if (rewardText != null) rewardText.text = "+" + gemsReward;
        pendingGems = gemsReward;
        gemsAlreadyAdded = false;
        Show();
    }

    protected override void OnShow()
    {
        if (SfxLibrary.Instance != null) SfxLibrary.Instance.Play(SfxLibrary.Instance.zoneComplete);

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

        if (!gemsAlreadyAdded && FlyEffectSpawner.Instance != null && gemIcon != null)
        {
            gemsAlreadyAdded = true;
            FlyEffectSpawner.Instance.FlyGems(gemIcon.position, pendingGems, 4);
        }
        else if (!gemsAlreadyAdded)
        {
            gemsAlreadyAdded = true;
            if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddGems(pendingGems);
        }

        Hide();
    }
}

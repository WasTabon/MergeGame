using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class LevelDefeatPopup : BasePopup
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;

    protected override void Awake()
    {
        base.Awake();
        if (retryButton != null) retryButton.onClick.AddListener(OnRetry);
        if (menuButton != null) menuButton.onClick.AddListener(OnMenu);
    }

    public void ShowDefeat(int level, LevelFailReason reason)
    {
        if (titleText != null) titleText.text = "DEFEAT";
        if (reasonText != null) reasonText.text = GetReasonText(reason);
        Show();
    }

    private string GetReasonText(LevelFailReason reason)
    {
        switch (reason)
        {
            case LevelFailReason.OutOfTime: return "RAN OUT OF TIME!";
            case LevelFailReason.BlockReachedBottom: return "BLOCKS REACHED THE PICKAXES!";
            case LevelFailReason.NoPickaxesLeft: return "ALL PICKAXES BROKEN!";
        }
        return "";
    }

    private void OnRetry()
    {
        Hide();
        if (LevelManager.Instance != null) LevelManager.Instance.RestartLevel();
    }

    private void OnMenu()
    {
        Hide();
        if (LevelManager.Instance != null) LevelManager.Instance.GoToLevelSelect();
        else if (TransitionManager.Instance != null) TransitionManager.Instance.LoadScene("LevelSelect");
    }
}

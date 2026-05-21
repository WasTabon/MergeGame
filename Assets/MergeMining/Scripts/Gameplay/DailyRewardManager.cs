using System;
using UnityEngine;

public class DailyRewardManager : MonoBehaviour
{
    public static DailyRewardManager Instance { get; private set; }

    [SerializeField] private DailyRewardPopup popup;

    private const string LAST_CLAIM_DATE_KEY = "daily_last_claim_date";
    private const string CURRENT_DAY_KEY = "daily_current_day";

    private void Awake() { Instance = this; }

    private void Start()
    {
        CheckDailyReward();
    }

    private string TodayDateString()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd");
    }

    public bool CanClaimToday()
    {
        string last = PlayerPrefs.GetString(LAST_CLAIM_DATE_KEY, "");
        return last != TodayDateString();
    }

    public int GetCurrentDay()
    {
        return Mathf.Max(1, PlayerPrefs.GetInt(CURRENT_DAY_KEY, 1));
    }

    private void CheckDailyReward()
    {
        if (!CanClaimToday()) return;
        if (popup != null) popup.ShowForDay(GetCurrentDay());
    }

    public void ClaimToday()
    {
        if (!CanClaimToday()) return;

        int day = GetCurrentDay();
        DailyRewardEntry entry = DailyRewardConfigProvider.Config.GetForDay(day);
        if (entry != null) GrantReward(entry);

        PlayerPrefs.SetString(LAST_CLAIM_DATE_KEY, TodayDateString());
        PlayerPrefs.SetInt(CURRENT_DAY_KEY, day + 1);

        if (TutorialManager.Instance != null) TutorialManager.Instance.NotifyOtherFlowEnded();
    }

    private void GrantReward(DailyRewardEntry e)
    {
        if (e.kind == DailyRewardKind.Coins && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(e.amount);
        }
        else if (e.kind == DailyRewardKind.Gems && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddGems(e.amount);
        }
        else if (e.kind == DailyRewardKind.PickaxeLevel && PickaxeGridManager.Instance != null)
        {
            if (PickaxeGridManager.Instance.HasFreeSlot())
            {
                PickaxeGridManager.Instance.AddPickaxe(e.pickaxeLevel);
            }
            else if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCoins(e.pickaxeLevel * 50);
            }
        }
    }
}

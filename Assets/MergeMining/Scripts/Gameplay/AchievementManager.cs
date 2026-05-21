using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [SerializeField] private AchievementToast toast;

    private const string CLAIMED_PREFIX = "achievement_claimed_";
    private const string CHESTS_OPENED_KEY = "stat_chests_opened";
    private const string BOOSTERS_USED_KEY = "stat_boosters_used";

    public event Action<AchievementDefinition> OnAchievementUnlocked;

    private void Awake() { Instance = this; }

    private void Start()
    {
        StartCoroutine(CheckLater());
    }

    private System.Collections.IEnumerator CheckLater()
    {
        yield return null;
        CheckAll();
    }

    public bool IsClaimed(string id)
    {
        return PlayerPrefs.GetInt(CLAIMED_PREFIX + id, 0) == 1;
    }

    public int GetProgress(AchievementCondition cond)
    {
        switch (cond)
        {
            case AchievementCondition.BlocksDestroyed:
                return PlayerPrefs.GetInt("total_blocks_destroyed", 0);
            case AchievementCondition.PickaxesPurchased:
                return PlayerPrefs.GetInt("pickaxes_bought", 0);
            case AchievementCondition.HighestPickaxeLevel:
                return PlayerPrefs.GetInt("highest_pickaxe_level", 1);
            case AchievementCondition.ZonesUnlocked:
                return PlayerPrefs.GetInt("unlocked_zones_count", 1);
            case AchievementCondition.ChestsOpened:
                return PlayerPrefs.GetInt(CHESTS_OPENED_KEY, 0);
            case AchievementCondition.BoostersUsed:
                return PlayerPrefs.GetInt(BOOSTERS_USED_KEY, 0);
        }
        return 0;
    }

    public void IncrementChestsOpened()
    {
        int v = PlayerPrefs.GetInt(CHESTS_OPENED_KEY, 0) + 1;
        PlayerPrefs.SetInt(CHESTS_OPENED_KEY, v);
        CheckAll();
    }

    public void IncrementBoostersUsed()
    {
        int v = PlayerPrefs.GetInt(BOOSTERS_USED_KEY, 0) + 1;
        PlayerPrefs.SetInt(BOOSTERS_USED_KEY, v);
        CheckAll();
    }

    public void CheckAll()
    {
        List<AchievementDefinition> all = AchievementConfigProvider.Config.achievements;
        foreach (var a in all)
        {
            if (IsClaimed(a.id)) continue;
            int progress = GetProgress(a.condition);
            if (progress >= a.target)
            {
                UnlockAndReward(a);
            }
        }
    }

    private void UnlockAndReward(AchievementDefinition a)
    {
        PlayerPrefs.SetInt(CLAIMED_PREFIX + a.id, 1);
        if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddGems(a.gemsReward);
        OnAchievementUnlocked?.Invoke(a);
        if (toast != null) toast.Show(a);
    }
}

using System;
using UnityEngine;

public class OfflineRewardsManager : MonoBehaviour
{
    public static OfflineRewardsManager Instance { get; private set; }

    [SerializeField] private OfflineRewardsPopup popup;
    [SerializeField] private float maxOfflineSeconds = 4f * 3600f;
    [SerializeField] private float minOfflineSecondsToShow = 60f;
    [SerializeField] private float coinsPerSecondPerLevel = 0.5f;

    private const string LAST_SEEN_KEY = "last_seen_epoch";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CheckOfflineRewardsOnStart();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) SaveLastSeen();
        else CheckOfflineRewardsOnStart();
    }

    private void OnApplicationQuit()
    {
        SaveLastSeen();
    }

    private void SaveLastSeen()
    {
        double now = GetEpoch();
        PlayerPrefs.SetString(LAST_SEEN_KEY, now.ToString("F0", System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.Save();
    }

    private void CheckOfflineRewardsOnStart()
    {
        string raw = PlayerPrefs.GetString(LAST_SEEN_KEY, "");
        if (string.IsNullOrEmpty(raw))
        {
            SaveLastSeen();
            return;
        }

        double last = 0;
        double.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out last);
        double now = GetEpoch();
        float secondsAway = (float)(now - last);

        SaveLastSeen();

        if (secondsAway < minOfflineSecondsToShow) return;

        float capped = Mathf.Min(secondsAway, maxOfflineSeconds);
        int coinsEarned = CalculateOfflineCoins(capped);
        if (coinsEarned <= 0) return;

        if (popup != null)
        {
            popup.ShowReward(capped, coinsEarned);
        }
        else
        {
            if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddCoins(coinsEarned);
        }
    }

    private int CalculateOfflineCoins(float seconds)
    {
        if (BlocksRowManager.Instance == null) return 0;

        float dpsTotal = 0f;
        int pickaxeCount = 0;
        foreach (var s in GetSlotsViaPublicApi())
        {
            if (s != null && !s.IsEmpty && s.CurrentPickaxe != null)
            {
                var data = PickaxeConfigProvider.Config.GetLevel(s.CurrentPickaxe.Level);
                if (data != null) dpsTotal += data.damage * data.miningSpeed;
                pickaxeCount++;
            }
        }

        if (dpsTotal <= 0f) return 0;

        int totalDestroyed = BlocksRowManager.Instance.TotalDestroyed;
        int avgReward = BlockConfigProvider.Config.CalcReward(totalDestroyed);
        float avgHP = BlockConfigProvider.Config.CalcHP(totalDestroyed);
        if (avgHP <= 0f) return 0;

        float blocksPerSec = dpsTotal / avgHP;
        float coinsPerSec = blocksPerSec * avgReward * 0.5f;
        return Mathf.RoundToInt(coinsPerSec * seconds);
    }

    private System.Collections.Generic.List<PickaxeSlot> GetSlotsViaPublicApi()
    {
        var list = new System.Collections.Generic.List<PickaxeSlot>();
        PickaxeSlot[] all = FindObjectsOfType<PickaxeSlot>(true);
        foreach (var s in all) list.Add(s);
        return list;
    }

    private double GetEpoch()
    {
        return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}

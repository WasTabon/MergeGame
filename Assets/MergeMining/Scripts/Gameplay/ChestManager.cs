using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    public static ChestManager Instance { get; private set; }

    [SerializeField] private ChestOpeningPopup chestOpeningPopup;

    private const string LAST_FREE_CHEST_KEY = "last_free_chest_time";
    private const string BLOCKS_SINCE_CHEST_KEY = "blocks_since_chest";
    private const string PENDING_BLOCK_CHEST_KEY = "pending_block_chest";

    public event Action OnFreeChestReady;
    public event Action OnBlockChestEarned;

    private double lastFreeChestTime;
    private int blocksSinceChest;
    private bool pendingBlockChest;

    public bool IsFreeChestReady
    {
        get
        {
            double now = GetEpoch();
            return (now - lastFreeChestTime) >= ChestConfigProvider.Config.freeChestCooldownSeconds;
        }
    }

    public float FreeChestSecondsRemaining
    {
        get
        {
            double now = GetEpoch();
            float left = ChestConfigProvider.Config.freeChestCooldownSeconds - (float)(now - lastFreeChestTime);
            return Mathf.Max(0f, left);
        }
    }

    public bool HasPendingBlockChest => pendingBlockChest;
    public int BlocksSinceChest => blocksSinceChest;
    public int BlocksRequiredForChest => ChestConfigProvider.Config.blocksPerChest;

    private void Awake()
    {
        Instance = this;
        LoadState();
    }

    private void LoadState()
    {
        string raw = PlayerPrefs.GetString(LAST_FREE_CHEST_KEY, "0");
        double.TryParse(raw, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out lastFreeChestTime);
        blocksSinceChest = PlayerPrefs.GetInt(BLOCKS_SINCE_CHEST_KEY, 0);
        pendingBlockChest = PlayerPrefs.GetInt(PENDING_BLOCK_CHEST_KEY, 0) == 1;
    }

    private double GetEpoch()
    {
        return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    public void RegisterBlockDestroyed()
    {
        blocksSinceChest++;
        PlayerPrefs.SetInt(BLOCKS_SINCE_CHEST_KEY, blocksSinceChest);

        int req = ChestConfigProvider.Config.blocksPerChest;
        if (blocksSinceChest >= req && !pendingBlockChest)
        {
            blocksSinceChest = 0;
            pendingBlockChest = true;
            PlayerPrefs.SetInt(BLOCKS_SINCE_CHEST_KEY, 0);
            PlayerPrefs.SetInt(PENDING_BLOCK_CHEST_KEY, 1);
            OnBlockChestEarned?.Invoke();
        }
    }

    public bool TryOpenFreeChest()
    {
        if (!IsFreeChestReady) return false;
        lastFreeChestTime = GetEpoch();
        PlayerPrefs.SetString(LAST_FREE_CHEST_KEY, lastFreeChestTime.ToString("F0", System.Globalization.CultureInfo.InvariantCulture));
        OpenChest(ChestType.Free);
        return true;
    }

    public bool TryOpenBlockChest()
    {
        if (!pendingBlockChest) return false;
        pendingBlockChest = false;
        PlayerPrefs.SetInt(PENDING_BLOCK_CHEST_KEY, 0);
        OpenChest(ChestType.Block);
        return true;
    }

    public bool TryOpenPremiumChest()
    {
        int cost = ChestConfigProvider.Config.premiumChestGemsCost;
        if (CurrencyManager.Instance == null) return false;
        if (!CurrencyManager.Instance.SpendGems(cost)) return false;
        OpenChest(ChestType.Premium);
        return true;
    }

    private void OpenChest(ChestType type)
    {
        ChestTypeData chest = ChestConfigProvider.Config.Get(type);
        if (chest == null) return;

        List<GeneratedChestReward> rewards = GenerateRewards(chest);
        ApplyRewards(rewards);

        if (chestOpeningPopup != null) chestOpeningPopup.ShowChest(chest, rewards);

        if (AchievementManager.Instance != null) AchievementManager.Instance.IncrementChestsOpened();
    }

    private List<GeneratedChestReward> GenerateRewards(ChestTypeData chest)
    {
        List<GeneratedChestReward> result = new List<GeneratedChestReward>();
        if (chest.possibleRewards.Count == 0) return result;

        float totalWeight = 0f;
        foreach (var r in chest.possibleRewards) totalWeight += r.weight;

        for (int i = 0; i < chest.rewardsCount; i++)
        {
            float roll = UnityEngine.Random.value * totalWeight;
            float acc = 0f;
            ChestReward chosen = chest.possibleRewards[0];
            foreach (var r in chest.possibleRewards)
            {
                acc += r.weight;
                if (roll <= acc) { chosen = r; break; }
            }

            GeneratedChestReward gen = new GeneratedChestReward
            {
                id = chosen.id,
                amount = UnityEngine.Random.Range(chosen.minAmount, chosen.maxAmount + 1),
                color = chosen.color
            };
            result.Add(gen);
        }
        return result;
    }

    private void ApplyRewards(List<GeneratedChestReward> rewards)
    {
        foreach (var r in rewards)
        {
            if (r.id == "coins" && CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddCoins(r.amount);
            }
            else if (r.id == "gems" && CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.AddGems(r.amount);
            }
            else if (r.id.StartsWith("pickaxe_lvl") && PickaxeGridManager.Instance != null)
            {
                int level = int.Parse(r.id.Substring("pickaxe_lvl".Length));
                if (PickaxeGridManager.Instance.HasFreeSlot())
                {
                    PickaxeGridManager.Instance.AddPickaxe(level);
                }
                else
                {
                    int compensation = level * 25;
                    if (CurrencyManager.Instance != null) CurrencyManager.Instance.AddCoins(compensation);
                }
            }
        }
    }
}
